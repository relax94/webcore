﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tusdotnet.Constants;
using tusdotnet.Interfaces;
using tusdotnet.Models;

namespace tusdotnet.Middleware
{

    public class UploadMiddlewareOptions
    {

    }

    public class UploadMiddleware
    {
        private readonly RequestDelegate Next;
        private readonly UploadMiddlewareOptions options;
        private static readonly Dictionary<string, SemaphoreSlim> FileLocks = new Dictionary<string, SemaphoreSlim>();
        private ITusConfiguration _config;

        public UploadMiddleware(RequestDelegate nextFunc, ITusConfiguration config)
        {
            this._config = config;
            this.Next = nextFunc;
        }

        public Task Invoke(HttpContext context)
        {
            ValidateConfig();

            if (!ShouldHandleRequest(context.Request))
            {
                return Next.Invoke(context);
            }

            var method = context.Request.Method.ToLower();

            var tusResumable = context.Request.Headers[HeaderConstants.TusResumable];

            if (method != "options" && tusResumable != HeaderConstants.TusResumableValue)
            {
                context.Response.Headers[HeaderConstants.TusResumable] = HeaderConstants.TusResumableValue;
                context.Response.Headers[HeaderConstants.TusVersion] = HeaderConstants.TusResumableValue;
                return RespondAsync(context,
                    HttpStatusCode.PreconditionFailed,
                    $"Tus version {tusResumable} is not supported. Supported versions: {HeaderConstants.TusResumableValue}");
            }

            switch (method)
            {
                case "post":
                    return HandlePostRequest(context);
                case "head":
                    return HandleHeadRequest(context);
                case "patch":
                    return HandlePatchRequest(context);
                case "options":
                    return HandleOptionsRequest(context);
                default:
                    throw new NotImplementedException($"HTTP method {context.Request.Method.ToLower()} is not implemented.");
            }
        }

        private async Task HandlePostRequest(HttpContext context)
        {
            // TODO: Add support for metadata if the store supports it
            // TODO: Add support for Defer-Upload-Length if the store supports it

            /*
			 * The Client MUST send a POST request against a known upload creation URL to request a new upload resource. 
			 * The request MUST include one of the following headers:
			 * a) Upload-Length to indicate the size of an entire upload in bytes.
			 * b) Upload-Defer-Length: 1 if upload size is not known at the time. 
			 * Once it is known the Client MUST set the Upload-Length header in the next PATCH request. 
			 * Once set the length MUST NOT be changed. As long as the length of the upload is not known, t
			 * he Server MUST set Upload-Defer-Length: 1 in all responses to HEAD requests.
			 * If the Server supports deferring length, it MUST add creation-defer-length to the Tus-Extension header.
			 * The Client MAY supply the Upload-Metadata header to add additional metadata to the upload creation request. 
			 * The Server MAY decide to ignore or use this information to further process the request or to reject it. 
			 * If an upload contains additional metadata, responses to HEAD requests MUST include the Upload-Metadata 
			 * header and its value as specified by the Client during the creation.
			 * If the length of the upload exceeds the maximum, which MAY be specified using the Tus-Max-Size header, 
			 * the Server MUST respond with the 413 Request Entity Too Large status.
			 * The Server MUST acknowledge a successful upload creation with the 201 Created status. 
			 * The Server MUST set the Location header to the URL of the created resource. This URL MAY be absolute or relative.
			 * The Client MUST perform the actual upload using the core protocol.
			 * */

            var tusCreationStore = _config.Store as ITusCreationStore;
            if (tusCreationStore == null)
            {
                await Next.Invoke(context);
                return;
            }

            if (!context.Request.Headers.ContainsKey(HeaderConstants.UploadLength))
            {
                await RespondAsync(context, HttpStatusCode.BadRequest, $"Missing {HeaderConstants.UploadLength} header");
                return;
            }

            long uploadLength;
            if (!long.TryParse(context.Request.Headers[HeaderConstants.UploadLength], out uploadLength))
            {
                await RespondAsync(context, HttpStatusCode.BadRequest, $"Could not parse {HeaderConstants.UploadLength}");
                return;
            }

            if (uploadLength < 0)
            {
                await RespondAsync(context,
                    HttpStatusCode.BadRequest,
                    $"Header {HeaderConstants.UploadLength} must be a positive number");
                return;
            }

            var fileName = await tusCreationStore.CreateFileAsync(uploadLength, context.RequestAborted);

            context.Response.Headers[HeaderConstants.TusResumable] = HeaderConstants.TusResumableValue;
            context.Response.Headers[HeaderConstants.Location] = _config.UrlPath.TrimEnd('/') + "/" + fileName;
            context.Response.StatusCode = (int)HttpStatusCode.Created;
        }

        private Task HandleOptionsRequest(HttpContext context)
        {
            /*
			 * An OPTIONS request MAY be used to gather information about the Server’s current configuration. 
			 * A successful response indicated by the 204 No Content status MUST contain the Tus-Version header. 
			 * It MAY include the Tus-Extension and Tus-Max-Size headers.
			 * The Client SHOULD NOT include the Tus-Resumable header in the request and the Server MUST discard it.
			 * */

            context.Response.Headers[HeaderConstants.TusResumable] = HeaderConstants.TusResumableValue;
            context.Response.Headers[HeaderConstants.TusVersion] = HeaderConstants.TusResumableValue;

            var extensions = DetectExtensions();
            if (extensions.Any())
            {
                context.Response.Headers[HeaderConstants.TusExtension] = string.Join(",", extensions);
            }

            context.Response.StatusCode = (int)HttpStatusCode.NoContent;
            return Task.FromResult(true);
        }

        private async Task HandlePatchRequest(HttpContext context)
        {
            /*
			 * The Server SHOULD accept PATCH requests against any upload URL and apply the bytes 
			 * contained in the message at the given offset specified by the Upload-Offset header. 
			 * All PATCH requests MUST use Content-Type: application/offset+octet-stream.
			 * The Upload-Offset header’s value MUST be equal to the current offset of the resource. 
			 * In order to achieve parallel upload the Concatenation extension MAY be used. 
			 * If the offsets do not match, the Server MUST respond with the 409 Conflict status without modifying the upload resource.
			 * The Client SHOULD send all the remaining bytes of an upload in a single PATCH request, 
			 * but MAY also use multiple small requests successively for scenarios where 
			 * this is desirable, for example, if the Checksum extension is used.
			 * The Server MUST acknowledge successful PATCH requests with the 204 No Content status. 
			 * It MUST include the Upload-Offset header containing the new offset. 
			 * The new offset MUST be the sum of the offset before the PATCH request and the number of bytes received and 
			 * processed or stored during the current PATCH request.
			 * Both, Client and Server, SHOULD attempt to detect and handle network errors predictably. 
			 * They MAY do so by checking for read/write socket errors, as well as setting read/write timeouts. 
			 * A timeout SHOULD be handled by closing the underlying connection.
			 * The Server SHOULD always attempt to store as much of the received data as possible.
			 * */

            var fileName = GetFileName(context.Request);
            var cancellationToken = context.RequestAborted;

            SemaphoreSlim semaphore;

            lock (FileLocks)
            {
                if (!FileLocks.ContainsKey(fileName))
                {
                    FileLocks[fileName] = new SemaphoreSlim(1);
                }

                semaphore = FileLocks[fileName];
            }

            var hasLock = await semaphore.WaitAsync(TimeSpan.Zero, cancellationToken);

            if (!hasLock)
            {
                await
                    RespondAsync(context, HttpStatusCode.Conflict,
                        $"File {fileName} is currently being updated. Please try again later");
            }

            try
            {
                if (context.Request.ContentType == null ||
                    !context.Request.ContentType.Equals("application/offset+octet-stream",
                        StringComparison.OrdinalIgnoreCase))
                {
                    await RespondAsync(context,
                        HttpStatusCode.BadRequest,
                        $"Content-Type {context.Request.ContentType} is invalid. Must be application/offset+octet-stream");
                    return;
                }

                if (!context.Request.Headers.ContainsKey(HeaderConstants.UploadOffset))
                {
                    await RespondAsync(context, HttpStatusCode.BadRequest, $"Missing {HeaderConstants.UploadOffset} header");
                    return;
                }

                long requestOffset;

                if (!long.TryParse(context.Request.Headers[HeaderConstants.UploadOffset], out requestOffset))
                {
                    await RespondAsync(context, HttpStatusCode.BadRequest, $"Could not parse {HeaderConstants.UploadOffset} header");
                    return;
                }

                if (requestOffset < 0)
                {
                    await RespondAsync(context, HttpStatusCode.BadRequest,
                        $"Header {HeaderConstants.UploadOffset} must be a positive number");
                    return;
                }

                var exists = await _config.Store.FileExistAsync(fileName, cancellationToken);
                if (!exists)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    context.Response.Headers[HeaderConstants.TusResumable] = HeaderConstants.TusResumableValue;
                    context.Response.Headers[HeaderConstants.CacheControl] = HeaderConstants.NoStore;
                    return;
                }

                var fileOffset = await _config.Store.GetUploadOffsetAsync(fileName, cancellationToken);

                if (requestOffset != fileOffset)
                {
                    await RespondAsync(context,
                        HttpStatusCode.Conflict,
                        $"Offset does not match file. File offset: {fileOffset}. Request offset: {requestOffset}");
                    return;
                }

                var fileUploadLength = await _config.Store.GetUploadLengthAsync(fileName, cancellationToken);

                if (fileUploadLength != null && fileOffset == fileUploadLength.Value)
                {
                    await RespondAsync(context, HttpStatusCode.BadRequest, "Upload is already complete.");
                    return;
                }

                long bytesWritten;
                try
                {
                    bytesWritten = await _config.Store.AppendDataAsync(fileName, context.Request.Body, cancellationToken);
                }
                catch (IOException ioException)
                {
                    // Indicates that the client disconnected.
                    if (!(ioException.InnerException is Exception))
                    {
                        throw;
                    }

                    // Client disconnected so no need to return a response.
                    return;
                }
                catch (TusStoreException storeException)
                {
                    await RespondAsync(context, HttpStatusCode.BadRequest, storeException.Message);
                    throw;
                }

                context.Response.StatusCode = (int)HttpStatusCode.NoContent;
                context.Response.Headers[HeaderConstants.TusResumable] = HeaderConstants.TusResumableValue;
                context.Response.Headers[HeaderConstants.UploadOffset] = (fileOffset + bytesWritten).ToString();
            }
            finally
            {
                semaphore.Release();
                lock (FileLocks)
                {
                    FileLocks.Remove(fileName);
                }
            }
        }

        private async Task HandleHeadRequest(HttpContext context)
        {
            /*
			 * The Server MUST always include the Upload-Offset header in the response for a HEAD request, 
			 * even if the offset is 0, or the upload is already considered completed. If the size of the upload is known, 
			 * the Server MUST include the Upload-Length header in the response. 
			 * If the resource is not found, the Server SHOULD return either the 404 Not Found, 410 Gone or 403 Forbidden 
			 * status without the Upload-Offset header.
			 * The Server MUST prevent the client and/or proxies from caching the response by adding the 
			 * Cache-Control: no-store header to the response.
			 * */

            var fileName = GetFileName(context.Request);
            var cancellationToken = context.RequestAborted;

            var exists = await _config.Store.FileExistAsync(fileName, cancellationToken);
            if (!exists)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.Headers[HeaderConstants.CacheControl] = HeaderConstants.NoStore;
                return;
            }

            var uploadLength = await _config.Store.GetUploadLengthAsync(fileName, cancellationToken);
            if (uploadLength != null)
            {
                context.Response.Headers[HeaderConstants.UploadLength] = uploadLength.Value.ToString();
            }

            var uploadOffset = await _config.Store.GetUploadOffsetAsync(fileName, cancellationToken);
            context.Response.Headers[HeaderConstants.TusResumable] = HeaderConstants.TusResumableValue;
            context.Response.Headers[HeaderConstants.UploadOffset] = uploadOffset.ToString();
            context.Response.Headers[HeaderConstants.CacheControl] = HeaderConstants.NoStore;
        }

        private bool ShouldHandleRequest(HttpRequest request)
        {
            var method = request.Method.ToLower();

            if (!request.Headers.ContainsKey(HeaderConstants.TusResumable) && method != "options")
            {
                return false;
            }

            switch (method)
            {
                case "post":
                case "options":
                    return IsExactUrlMatch(request);
                case "head":
                case "patch":
                    return !IsExactUrlMatch(request) &&
                           GetUri(request).LocalPath.StartsWith(_config.UrlPath, StringComparison.OrdinalIgnoreCase);
                default:
                    return false;
            }
        }

        private static Uri GetUri(HttpRequest request)
        {
            var hostComponents = request.Host.ToUriComponent().Split(':');

            var builder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = hostComponents[0],
                Path = request.Path,
                Query = request.QueryString.ToUriComponent()
            };

            if (hostComponents.Length == 2)
            {
                builder.Port = Convert.ToInt32(hostComponents[1]);
            }

            return builder.Uri;
        }

        private string GetFileName(HttpRequest request)
        {
            var startIndex = GetUri(request)
                                 .LocalPath
                                 .IndexOf(_config.UrlPath, StringComparison.OrdinalIgnoreCase) + _config.UrlPath.Length;
            return GetUri(request)
                .LocalPath
                .Substring(startIndex)
                .Trim('/');
        }


        private bool IsExactUrlMatch(HttpRequest request)
        {
            return GetUri(request).LocalPath.TrimEnd('/') == _config.UrlPath.TrimEnd('/');
        }

        private List<string> DetectExtensions()
        {
            var extensions = new List<string>();
            if (_config.Store is ITusCreationStore)
            {
                extensions.Add(ExtensionConstants.Creation);
            }

            return extensions;
        }

        private static Task RespondAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "text/plain";
            context.Response.Headers[HeaderConstants.TusResumable] = HeaderConstants.TusResumableValue;
            return context.Response.WriteAsync(message);
        }

        private void ValidateConfig()
        {
            if (_config.Store == null)
            {
                throw new TusConfigurationException($"{nameof(_config.Store)} cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(_config.UrlPath))
            {
                throw new TusConfigurationException($"{nameof(_config.UrlPath)} cannot be empty.");
            }
        }
    }

}
