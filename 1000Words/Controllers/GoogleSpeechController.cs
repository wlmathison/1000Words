using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _1000Words.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _1000Words.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleSpeechController : ControllerBase
    {
        [HttpPost]
        public IActionResult PostWav(IFormFile audio)
        {
            // Uploading audio file to cloud Speech-to-Text API
            GoogleSpeech.UploadAudio(audio);

            // Returning keywords from API
            return Ok(GoogleSpeech.Keywords);
        }
    }
}