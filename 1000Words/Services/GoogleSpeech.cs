using Google.Cloud.Speech.V1;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;

namespace _1000Words.Services
{
    public class GoogleSpeech
    {
        public static List<string> Keywords { get; set; } = new List<string>();

        public static void UploadAudio(IFormFile audio)
        {
            // Reference to Google Cloud Speech-to_text Credentials
            string credential_path = @"C:\Users\Billy\workspace\capstones\1000Words\1000Words\words-247918-f13fa4057b4a.json";
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credential_path);

            using (Stream stream = audio.OpenReadStream())
            {
                RecognitionAudio recognitionAudio = RecognitionAudio.FromStream(stream);

                var speech = SpeechClient.Create();
                var response = speech.Recognize(new RecognitionConfig()
                {
                    Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                    LanguageCode = "en",
                }, recognitionAudio);

                Keywords.Clear();

                foreach (var result in response.Results)
                {
                    foreach (var alternative in result.Alternatives)
                    {
                        // Add transcript to list of keywords to be returned
                        Keywords.Add(alternative.Transcript);
                    }
                }
            }
        }
    }
}
