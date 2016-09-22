using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webcore.Models;
using webcore.Services;
using webcore.ViewModels;

namespace webcore.Controllers.Web
{
  
    public class AppController : Controller
    {
        private IMailService _mailService;
        private IConfigurationRoot _config;
        private ILogger<AppController> _logger;
        private IWebCoreRepository _webCoreRepository;
        private IHostingEnvironment _env;

        public AppController(IHostingEnvironment env,IMailService mailService, IConfigurationRoot config, IWebCoreRepository webCoreRepository, ILogger<AppController> logger)
        {
            _mailService = mailService;
            _config = config;
            _logger = logger;
            _env = env;
            _webCoreRepository = webCoreRepository;
        }

       // [RequestFormSizeLimit(valueCountLimit: 2147483648)]
        [HttpPost]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            if(file != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                using(var fileStream = new FileStream(Path.Combine(uploadsFolder, file.FileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }

            return Ok();
        }

        public async Task<IActionResult> Index()
        {

            //var insertResponse = await this._webCoreRepository.InsertRecordTaskAsync<Video>(new Video {FileName = "test" });
            //var all = await this._webCoreRepository.GetRecordsTaskAsync<Video>();
            

            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel contact)
        {
            _mailService.SendMail("", contact.Email, "", contact.Message);

            return View();
        }
    }
}
