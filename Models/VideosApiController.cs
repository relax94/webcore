using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webcore.Models
{
    public class VideosApiController : Controller
    {
        private IWebCoreRepository _repository;

        public VideosApiController(IWebCoreRepository repository)
        {
            _repository = repository;
        }
    }
}
