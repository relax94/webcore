using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

        public AppController(IMailService mailService, IConfigurationRoot config, IWebCoreRepository webCoreRepository, ILogger<AppController> logger)
        {
            _mailService = mailService;
            _config = config;
            _logger = logger;
            _webCoreRepository = webCoreRepository;


        }

        public async Task<IActionResult> Index()
        {


            var trips = await _webCoreRepository.GetAllTrips<Trip>();

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
