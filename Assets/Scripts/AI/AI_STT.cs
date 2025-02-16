using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace SMUP.AI {
    public class AI_STT : MonoBehaviour
    {
        [SerializeField] private SpeechSettings_SO speechSettings_SO;

        private object threadLocker = new object();
        private bool waitingForReco;
        private string message = "";
        private bool micPermissionGranted = false;
    #if PLATFORM_ANDROID || PLATFORM_IOS
        private Microphone mic;
    #endif


        void Start()
        {
        #if PLATFORM_ANDROID
            message = "Waiting for mic permission";
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
        #else
            // Continue with normal initialization, Text and Button objects are present.
            micPermissionGranted = true;
        #endif
        }
        

        public async Task<string> SpeechToText()
        {
            SpeechConfig config = SpeechConfig.FromSubscription(speechSettings_SO.speechAPIKey, speechSettings_SO.region);
            config.SpeechRecognitionLanguage = speechSettings_SO.recognitionLanguage;
            message = "";

            using (var recognizer = new SpeechRecognizer(config))
            {
                lock (threadLocker)
                {
                    waitingForReco = true;
                }

                SpeechRecognitionResult result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

                // Checks result.
                string newMessage = string.Empty;
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    newMessage = result.Text;
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    newMessage = "NOMATCH: Speech could not be recognized.";
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    newMessage = $"CANCELED: Reason={cancellation.Reason} ErrorDetails={cancellation.ErrorDetails}";
                }

                lock (threadLocker)
                {
                    message = newMessage;
                    waitingForReco = false;
                }

                return message;
            }
        }
    }   
}