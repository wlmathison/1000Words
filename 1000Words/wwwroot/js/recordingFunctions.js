// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// webkitURL is deprecated but nevertheless
URL = window.URL || window.webkitURL;

var gumStream; 						//stream from getUserMedia()
var rec; 							//Recorder.js object
var input; 							//MediaStreamAudioSourceNode we'll be recording

// shim for AudioContext when it's not avb. 
var AudioContext = window.AudioContext || window.webkitAudioContext;
var audioContext; //audio context to help us record

var recordButton = document.getElementById("recordButton");
var loadButton = document.getElementById("load-button");
var loadRecordButton = document.getElementById("load-recording");

var recording = false;

var errorDiv = document.getElementById("edit-error-message");

var switches = document.getElementById("switches");

//add events to those 2 buttons
recordButton.addEventListener("mousedown", startRecording);
recordButton.addEventListener("mouseup", stopRecording);

function startRecording() {
    console.log("recordButton clicked");
    errorDiv.textContent = "";

	/*
		Simple constraints object, for more advanced audio features see
		https://addpipe.com/blog/audio-constraints-getusermedia/
	*/

    var constraints = { audio: true, video: false }

	/*
    	We're using the standard promise based getUserMedia() 
    	https://developer.mozilla.org/en-US/docs/Web/API/MediaDevices/getUserMedia
	*/

    navigator.mediaDevices.getUserMedia(constraints).then(function (stream) {
        //console.log("getUserMedia() success, stream created, initializing Recorder.js ...");

		/*
			create an audio context after getUserMedia is called
			sampleRate might change after getUserMedia is called, like it does on macOS when recording through AirPods
			the sampleRate defaults to the one set in your OS for your playback device
		*/
        audioContext = new AudioContext();

        //update the format 
        //document.getElementById("formats").innerHTML = "Format: 1 channel pcm @ " + audioContext.sampleRate / 1000 + "kHz"

        /*  assign to gumStream for later use  */
        gumStream = stream;

        /* use the stream */
        input = audioContext.createMediaStreamSource(stream);

		/* 
			Create the Recorder object and configure to record mono sound (1 channel)
			Recording 2 channels  will double the file size
		*/
        rec = new Recorder(input, { numChannels: 1 })

        //start the recording process
        rec.record()
        recording = true;
        loadRecordButton.style.display = "block"

        console.log("Recording started");

    }).catch(function (err) {
        //enable the record button if getUserMedia() fails
        recordButton.disabled = false;
    });
}

function stopRecording() {
    console.log("stopButton clicked");

    if (recording) {
        //tell the recorder to stop the recording
        rec.stop();
        recording = false;

        //stop microphone access
        gumStream.getAudioTracks()[0].stop();

        //create the wav blob and pass it on to createDownloadLink
        rec.exportWAV(CreateToggleSwitches);

        // Display load spinner while waiting for results
        loadRecordButton.style.display = "none"
        loadButton.style.display = "block";
    }
    else {
        setTimeout(stopRecording, 20);
    }



}


//Instantiating i to be used when creating custom switches in CreateToggleSwitches method.
let i = 0;

function CreateToggleSwitches(blob) {

    //create formData to include wav blob during fetch call
    var formData = new FormData();
    formData.append("audio", blob);

    var results;

    //List of english stop words to remove from results
    var stopWords = ["a", "able", "about", "across", "after", "all", "almost", "also", "am", "among", "an", "and", "any", "are", "as", "at", "be", "because", "been", "but", "by", "can", "cannot", "could", "dear", "did", "do", "does", "either", "else", "ever", "every", "for", "from", "get", "got", "had", "has", "have", "he", "her", "hers", "him", "his", "how", "however", "i", "if", "in", "into", "is", "it", "its", "just", "least", "let", "like", "likely", "looks", "may", "might", "most", "must", "my", "neither", "no", "nor", "not", "of", "off", "often", "on", "only", "or", "other", "our", "own", "rather", "said", "say", "says", "she", "should", "since", "so", "some", "something", "than", "that", "the", "their", "them", "then", "there", "these", "they", "this", "tis", "to", "too", "twas", "us", "wants", "was", "we", "were", "what", "when", "where", "which", "while", "who", "whom", "why", "will", "with", "would", "yet", "you", "your", "ain't", "aren't", "can't", "could've", "couldn't", "didn't", "doesn't", "don't", "hasn't", "he'd", "he'll", "he's", "how'd", "how'll", "how's", "i'd", "i'll", "i'm", "i've", "isn't", "it's", "might've", "mightn't", "must've", "mustn't", "shan't", "she'd", "she'll", "she's", "should've", "shouldn't", "that'll", "that's", "there's", "they'd", "they'll", "they're", "they've", "wasn't", "we'd", "we'll", "we're", "weren't", "what'd", "what's", "when'd", "when'll", "when's", "where'd", "where'll", "where's", "who'd", "who'll", "who's", "why'd", "why'll", "why's", "won't", "would've", "wouldn't", "you'd", "you'll", "you're", "you've", "picture", "photo"];

    //POST fetch call to GoogleSpeech WebApi
    fetch('http://localhost:5000/api/GoogleSpeech/', { method: 'POST', body: formData })
        .then(function (response) {
            return response.json();
        }).then(function (myJson) {
            results = myJson.join();
        }).then(() => {

            if (results != "") {
                //NodeList of checked keywords in photo edit view
                var checkedKeywords = document.getElementsByName("CheckedKeywords");
                var existingKeywords = [];

                //pushing the value of each item in the nodeList into a new array
                for (j = 0; j < checkedKeywords.length; j++) {
                    existingKeywords.push(checkedKeywords[j].value)
                }

                //Splitting results into individual words
                var resultsArray = results.split(" ");
                //Filtering out only words not in list of stop words or existing keyword on the photo
                var filteredResults = resultsArray.filter(r => !stopWords.includes(r.toLowerCase()) && !existingKeywords.includes(r.toLowerCase()));
                //Filtering out any duplicate words
                var nonDuplicateFilteredResults = filteredResults.filter((item, index) => filteredResults.indexOf(item) === index);

                //Creating a switch toggle for each filtered word andd appending it in photo edit view
                nonDuplicateFilteredResults.forEach(r => {
                    i++;
                    var div = document.createElement('div');
                    div.className = "custom-control custom-switch";

                    var input = document.createElement('input');
                    input.type = 'checkbox';
                    input.className = 'custom-control-input';
                    input.id = 'RecordingCustomSwitch-' + i;
                    input.name = 'CheckedKeywords';
                    input.value = r;
                    input.checked = true;

                    var label = document.createElement('label');
                    label.className = 'custom-control-label';
                    label.textContent = r;
                    label.htmlFor = 'RecordingCustomSwitch-' + i;

                    div.appendChild(input);
                    div.appendChild(label);

                    loadButton.style.display = "none";

                    switches.appendChild(div);
                });
            } else {
                loadButton.style.display = "none";
                errorDiv.textContent = "Error - Please try again."
            }
        })
}
