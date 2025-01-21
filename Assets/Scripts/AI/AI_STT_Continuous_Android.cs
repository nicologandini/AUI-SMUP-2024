using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.Android;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Linq;
using TMPro;

public class AI_STT_Continuous_Android : MonoBehaviour {
    [SerializeField] private SpeechSettings_SO speechSettings_SO;

    [SerializeField] private TextMeshProUGUI infoText;


    private SpeechRecognizer recognizer;
    private bool isRecognizing = false; // Stato del riconoscimento
    private bool stopRequested = false; // Flag per interrompere il riconoscimento
    private bool canReturn = false;
    private UnityEngine.InputSystem.InputAction actionBinding;

    private object threadLocker = new object();
    private string message = "";
    private bool micPermissionGranted = false;

    void Start() {
        ShowInfoText("");   
        RequestMicrophonePermission();
        InitializeSpeechRecognizer();
    }

    private void RequestMicrophonePermission() {
        // Richiede il permesso per il microfono su Android
        if (Application.platform == RuntimePlatform.Android && !Permission.HasUserAuthorizedPermission(Permission.Microphone)) {
            Permission.RequestUserPermission(Permission.Microphone);
        }

        micPermissionGranted = Permission.HasUserAuthorizedPermission(Permission.Microphone);

        if (!micPermissionGranted) {
            Debug.LogError("Permesso per il microfono non concesso. L'app non può eseguire il riconoscimento vocale.");
        }
    }

    public async Task<string> SpeechToText(UnityEngine.InputSystem.InputAction actionBinding, float timeout = 30f) {
        if (!isRecognizing && micPermissionGranted) {
            InitializeSpeechRecognizer();
            SetStopCallback(actionBinding);
            string text = await StartSpeechRecognition(timeout);
            ReleaseElements();

            return text;
        } else {
            Debug.LogError("Riconoscimento già in corso o permesso microfono non concesso.");
            return "";
        }
    }

    private void InitializeSpeechRecognizer() {
        SpeechConfig speechConfig = SpeechConfig.FromSubscription(speechSettings_SO.speechAPIKey, speechSettings_SO.region);
        speechConfig.SpeechRecognitionLanguage = speechSettings_SO.recognitionLanguage;
        speechConfig.SetProperty(PropertyId.Speech_SegmentationSilenceTimeoutMs, "1000"); // ms di delay prima di dividere le frasi
        AudioConfig audioConfig = AudioConfig.FromMicrophoneInput(FindMicrophoneDevice());

        if(audioConfig == null) {
            Debug.LogWarning("None input device found!");
            AppendInfoText("Nessun microfono trovato!");

            audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        }
        //audioConfig = AudioConfig.FromDefaultMicrophoneInput();

        //recognizer = new SpeechRecognizer(config);
        try {
        recognizer = new SpeechRecognizer(speechConfig, audioConfig);
        } catch (ApplicationException e) {
            ShowInfoText (e.Message);
            recognizer = new SpeechRecognizer(speechConfig, AudioConfig.FromDefaultMicrophoneInput());
        }

        // Eventi:
        recognizer.Recognizing += (s, e) => {
            Debug.Log($"Parola rilevata: {e.Result.Text}");
            AppendInfoText($"Parola rilevata: {e.Result.Text}");

        };

        recognizer.Recognized += (s, e) => {
            if (e.Result.Reason == ResultReason.RecognizedSpeech) {
                lock (threadLocker) {
                    Debug.Log($"Frase completa riconosciuta: {e.Result.Text}");
                    AppendInfoText($"Frase completa riconosciuta: {e.Result.Text}");
                    
                    message += e.Result.Text;
                }
            }
        };

        recognizer.SessionStopped += (s, e) => {
            lock (threadLocker) {
                Debug.Log("Sessione terminata.");
                AppendInfoText("Session stopped! ");

                canReturn = true;
            }
        };

        recognizer.Canceled += (s, e) => {
            Debug.LogError($"Errore nel riconoscimento: {e.ErrorDetails}");
            AppendInfoText($"Errore nel riconoscimento: {e.ErrorDetails}");

            canReturn = true;
        };
    }

    public void SetStopCallback(UnityEngine.InputSystem.InputAction actionBinding) {
        this.actionBinding = actionBinding;
        actionBinding.performed += OnStopPressed;
    }

    public void RemoveStopCallback() {
        if (this.actionBinding != null) {
            this.actionBinding.performed -= OnStopPressed;
            this.actionBinding = null;
        }
    }

    private void OnStopPressed(InputAction.CallbackContext context) {
        StopSpeechRecognition();
        RemoveStopCallback();
    }

    private async Task<string> StartSpeechRecognition(float timeout = 30f) {
        isRecognizing = true;
        stopRequested = false;
        canReturn = false;
        message = "";

        Debug.Log("Riconoscimento vocale avviato...");
        AppendInfoText("Riconoscimento vocale avviato...");

        await recognizer.StartContinuousRecognitionAsync();
        await WaitUntilTimeoutOrStopRequested(timeout);

        StopSpeechRecognition();
        await WaitToReturn();

        lock (threadLocker) {
            return message;
        }
    }

    private async void StopSpeechRecognition() {
        if (!isRecognizing) return;

        Debug.Log("Riconoscimento vocale interrotto...");
        AppendInfoText("Riconoscimento vocale interrotto...");

        stopRequested = true;
        await recognizer.StopContinuousRecognitionAsync();

        isRecognizing = false;
    }

    private async Task WaitUntilTimeoutOrStopRequested(float timeout) {
        float elapsedTime = 0f;

        while (elapsedTime < timeout && !stopRequested) {
            await Task.Delay(100); // Aspetta 100ms prima di controllare di nuovo
            elapsedTime += 0.1f; // Incrementa il tempo trascorso
        }
    }

    private async Task WaitToReturn() {
        while (!canReturn) {
            await Task.Delay(100); // Aspetta 100ms prima di controllare di nuovo
        }
    }

    private void OnDestroy() {
        ReleaseElements();
    }

    private void ReleaseElements() {
        if (recognizer != null) { recognizer.Dispose(); }
    }

    private AudioConfig GetAudioConfig(string deviceName)
    {
        // Ottieni i dispositivi disponibili (funzione placeholder, usa NAudio o configurazioni manuali)
        var availableDevices = Microphone.devices;
        if (availableDevices.Contains(deviceName))
        {
            Debug.Log($"Selezionato dispositivo: {deviceName}");
            return AudioConfig.FromMicrophoneInput(deviceName);
        }

        Debug.LogWarning($"Il dispositivo '{deviceName}' non è stato trovato. Usando il microfono di default.");
        AppendInfoText($"Il dispositivo '{deviceName}' non è stato trovato. Usando il microfono di default.");

        return AudioConfig.FromDefaultMicrophoneInput();
    }

    private string FindMicrophoneDevice() {
        // Ottieni l'elenco dei microfoni disponibili
        var devices = Microphone.devices;
        if (devices.Length > 0)
        {
            Debug.Log("Microfoni disponibili:");
            for (int i = 0; i < devices.Length; i++)
            {
                Debug.Log($"[{i}] {devices[i]}");
            }

            // Usa il primo microfono (o sostituisci con un indice specifico)
            string microphoneDevice = devices[0]; // Cambia l'indice per scegliere un microfono specifico
            Debug.Log("Microfono selezionato: " + microphoneDevice);
            AppendInfoText("Microfono selezionato: " + microphoneDevice);


            return microphoneDevice;
        }
        else
        {
            Debug.LogError("Nessun microfono trovato!");
            AppendInfoText("Nessun microfono trovato!");
            return "";
        }
    }


    private void ShowInfoText(string text) {
        if(infoText == null) { return; }

        infoText.text = text;
    }

    private void AppendInfoText(string text) {
        if(infoText == null) { return; }

        infoText.text += "\n" + text;
    }
}