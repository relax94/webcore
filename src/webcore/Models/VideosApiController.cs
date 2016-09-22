using AutoMapper;
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

        [HttpPost("/api/videos/insert")]
        public async Task<IActionResult> AddVideo([FromBody] VideoViewModel videoVM)
        {
            if (ModelState.IsValid)
            {
                Video video = Mapper.Map<Video>(videoVM);
                var insertResponse = await this._repository.InsertRecordTaskAsync<Video>(video);
                return Ok(new { response = insertResponse == 1 ? true : false });
            }
            else
                return BadRequest();
        }

        [HttpGet("/api/videos/get")]
        public async Task<IActionResult> GetVideos()
        {
            return Ok(await this._repository.GetRecordsTaskAsync<Video>());
        }
    }
}
