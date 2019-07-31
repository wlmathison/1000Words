using Google.Cloud.Speech.V1;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace _1000Words.Services
{
    public class GoogleSpeech
    {
        public static List<string> Keywords { get; set; } = new List<string>();

        public static void UploadAudio(IFormFile audio)
        {
            string credential_path = @"C:\Users\Billy\workspace\capstones\1000Words\1000Words\words-247918-f13fa4057b4a.json";
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credential_path);

            using (Stream stream = audio.OpenReadStream())
            {
                RecognitionAudio recognitionAudio = RecognitionAudio.FromStream(stream);

                var speech = SpeechClient.Create();
                var response = speech.Recognize(new RecognitionConfig()
                {
                    Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                    SampleRateHertz = 48000,
                    LanguageCode = "en",
                }, recognitionAudio);
                foreach (var result in response.Results)
                {
                    foreach (var alternative in result.Alternatives)
                    {
                        Keywords.Add(alternative.Transcript);
                    }
                }
            }
        }
    }
}
