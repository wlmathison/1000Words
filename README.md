# 1000Words
1000Words is an interactive photo album which allows users to tag their photos with keywords by using speech-to-text. 1000Words allows users to search through all of their photos quickly using only keywords or dates, significantly decreasing the amount of time needed to find photos. I built the application using C#, ASP.Net MVC, Entity Framework, Identity Framework, Recorder.js, JavaScript, and Google Cloud Speech-to-Text API.

## Installation (using speech-to-text functionality)
If you would like to use the speech-to-text you will need a Google account and to either sign up for a free trial or use an existing Google Cloud Platform account. 
1. Clone this repo: `git@github.com:wlmathison/1000Words.git`
1. Sign in to your Google account
1. Open GCP Console at console.cloud.google.com
1. If you were not using Google Cloud Platform before, then sign up for a free trial
1. Set up a new Google Cloud Platform Console Project
1. Enable the Google Speech-to-Text API for that project
1. Create a service account
1. Download a private key as JSON and paste into words-247918-f13fa4057b4a.json file, replacing existing text
1. To install all libraries and their dependencies, run `npm install`
1. Open NuGet Package Manager Console
1. In the console, run the command `Update-Database`
1. Run _1000Words

## Installation (without speech-to-text)
1. Clone this repo: `git@github.com:wlmathison/1000Words.git`
1. To install all libraries and their dependencies, run `npm install`
1. Open NuGet Package Manager Console
1. In the console, run the command `Update-Database`
1. Run _1000Words

### Technologies
Project created using </br>
[C#](https://docs.microsoft.com/en-us/dotnet/csharp/) </br>
[ASP.Net MVC](https://docs.microsoft.com/en-us/aspnet/mvc/) </br>
[Entity Framework](https://docs.microsoft.com/en-us/ef/) </br>
[Identity Framework](https://docs.microsoft.com/en-us/aspnet/identity/overview/) </br>
[Recorder.js](https://github.com/mattdiamond/Recorderjs) </br>
[JavaScript](https://developer.mozilla.org/en-US/docs/Web/JavaScript) </br>
[Google Cloud Speech-to-Text](https://cloud.google.com/speech-to-text/) </br>

### Author
Billy Mathison