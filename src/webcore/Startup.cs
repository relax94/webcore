using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using webcore.Services;
using webcore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using tusdotnet.Middleware;
using tusdotnet.Models;
using AutoMapper;

namespace webcore
{
    public class Startup
    {
        private IHostingEnvironment _env;
        private IConfigurationRoot _config;

        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)
                .AddJsonFile("appsettings.json");
            _config = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            

            services.AddLogging();
            services.AddIdentity<WebCoreUser, IdentityRole>(config =>
            {
                config.User.RequireUniqueEmail = true;
                config.Password.RequiredLength = 8;
                //config.Cookies.ApplicationCookie.LoginPath = "/Auth/Login";
            }).AddEntityFrameworkStores<WebCoreContext>();

            if (_env.IsDevelopment())
            {
                services.AddScoped<IMailService, DebugMailService>();

            }

            services.AddSingleton(_config);

            services.AddScoped<IWebCoreRepository, WebCoreRepository>();
            services.AddDbContext<WebCoreContext>();
            services.AddMvc().AddJsonOptions(option =>
            {
                option.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddConsole(LogLevel.Information);
            }
            else
            {
                loggerFactory.AddConsole(LogLevel.Error);
            }

            
            //app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseIdentity();

            Mapper.Initialize(config =>
            {
                config.CreateMap<VideoViewModel, Video>();
            });

            app.UseMiddleware<UploadMiddleware>(new DefaultTusConfiguration
            {
                Store = new TusDiskStore(@"E:\tusfiles\"),
                UrlPath = "/files"
            });
            

            app.UseMvc(config =>
            {
                config.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "App", action = "Index" }
                    );
            });

            
        }
    }
}
