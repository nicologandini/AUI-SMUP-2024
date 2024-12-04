using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class AI_STT_continuous : MonoBehaviour
{
    [SerializeField] private SpeechSettings_SO speechSettings_SO;


    private SpeechRecognizer recognizer;
    private bool isRecognizing = false; // Stato del riconoscimento
    private bool stopRequested = false; // Flag per interrompere il riconoscimento
    private bool canReturn = false;
    private UnityEngine.InputSystem.InputAction actionBinding;


    private object threadLocker = new object();
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
        micPermissionGranted = true;
    #endif

        InitializeSpeechRecognizer();
    }


    public async Task<string> SpeechToText(UnityEngine.InputSystem.InputAction actionBinding, float timeout = 30f) {
        if(!isRecognizing) {
            InitializeSpeechRecognizer();
            SetStopCallback(actionBinding);
            string text = await StartSpeechRecognition(timeout);
            ReleaseElements();

            return text;
        } else {
            return "";
        }
    }


    private void InitializeSpeechRecognizer()
    {
        SpeechConfig config = SpeechConfig.FromSubscription(speechSettings_SO.speechAPIKey, speechSettings_SO.region);
        config.SpeechRecognitionLanguage = speechSettings_SO.recognitionLanguage;
        config.SetProperty(PropertyId.Speech_SegmentationSilenceTimeoutMs, "1000");         //ms di delay prima di dividere le frasi
        recognizer = new SpeechRecognizer(config);

        // Evens:
        recognizer.Recognizing += (s, e) =>
        {
            //Debug.Log($"Parola rilevata: {e.Result.Text}");
        };

        recognizer.Recognized += (s, e) =>
        {
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                lock(threadLocker) {
                    //Debug.Log($"Frase completa riconosciuta: {e.Result.Text}");
                    message += e.Result.Text;
                }
            }
        };

        recognizer.SessionStopped += (s,e) => {
            lock(threadLocker) {
                canReturn = true;
            }
        }; 

        recognizer.Canceled += (s, e) =>
        {
            //Debug.LogError($"Errore nel riconoscimento: {e.ErrorDetails}");
            canReturn = true;
        };
    }

    public void SetStopCallback(UnityEngine.InputSystem.InputAction actionBinding) {
        this.actionBinding = actionBinding;
        actionBinding.performed += OnStopPressed;
    }

    public void RemoveStopCallback() {
        if(this.actionBinding != null) {
            this.actionBinding.performed -= OnStopPressed;
            this.actionBinding = null;
        }
    }

    private void OnStopPressed(InputAction.CallbackContext context)
    {
        StopSpeechRecognition();
        RemoveStopCallback();
    }


    private async Task<string> StartSpeechRecognition(float timeout = 30f)
    {
        isRecognizing = true;
        stopRequested = false;
        canReturn = false;
        message = "";

        //Debug.Log("Riconoscimento vocale avviato...");
        await recognizer.StartContinuousRecognitionAsync();
        await WaitUntilTimeoutOrStopRequested(timeout);

        StopSpeechRecognition();
        await WaitToReturn();

        lock (threadLocker) {
            //print($"Message: {message}");
            return message;
        }
    }

    private async void StopSpeechRecognition()
    {
        if (!isRecognizing) return;

        //Debug.Log("Riconoscimento vocale interrotto...");
        stopRequested = true;
        await recognizer.StopContinuousRecognitionAsync();

        isRecognizing = false;
    }


    private async Task WaitUntilTimeoutOrStopRequested(float timeout)
    {
        float elapsedTime = 0f;

        while (elapsedTime < timeout && !stopRequested)
        {
            await Task.Delay(100); // Aspetta 100ms prima di controllare di nuovo
            elapsedTime += 0.1f; // Incrementa il tempo trascorso
        }
    }

    private async Task WaitToReturn() {
        while(!canReturn) {
            await Task.Delay(100); // Aspetta 100ms prima di controllare di nuovo
        }
    }


    private void OnDestroy()
    {
        ReleaseElements();
    }

    private void ReleaseElements() {
        if (recognizer != null) { recognizer.Dispose(); }
    }
}