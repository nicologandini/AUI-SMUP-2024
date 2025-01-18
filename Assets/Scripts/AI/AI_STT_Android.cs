using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class AI_STT_Android : MonoBehaviour {
    [SerializeField] private SpeechSettings_SO speechSettings_SO;


    [Header("Recoding Values")]
    [SerializeField] private int maxRecordingDuration = 120; // Durata della registrazione in secondi
    [SerializeField] private int recordingFrequency = 44100; // Frequenza di campionamento (44100 Hz è standard)


    private string _microphoneDevice;
    private AudioClip _recordedClip;
    private SpeechConfig _speechConfig;
    private bool _isRecognizing;


    void Start() {
        InitializeSpeechRecognizer();
    }


    public async Task<string> SpeechToText(UnityEngine.InputSystem.InputAction actionBinding, float timeout = 30f) {
        if (!_isRecognizing) {
            StartAudioRec();
            await WaitForRecording();
            StopAudioRecording();
            string text = await RecognizeAudioClip(_recordedClip);

            return text;
        } else {
            return "";
        }
    }


    void InitializeSpeechRecognizer()
    {
        _speechConfig = SpeechConfig.FromSubscription(speechSettings_SO.speechAPIKey, speechSettings_SO.region);
        _speechConfig.SpeechRecognitionLanguage = speechSettings_SO.recognitionLanguage;
        FindMicrophone();
    }

    private void FindMicrophone()
    {
        var devices = Microphone.devices;
        if (devices.Length > 0)
        {
            Debug.Log("Microfoni disponibili:");
            for (int i = 0; i < devices.Length; i++)
            {
                Debug.Log($"[{i}] {devices[i]}");
            }

            // Usa il primo microfono (o sostituisci con un indice specifico)
            _microphoneDevice = devices[0]; // Cambia l'indice per scegliere un microfono specifico
            Debug.Log("Microfono selezionato: " + _microphoneDevice);
        }
        else
        {
            Debug.LogError("Nessun microfono trovato!");
            return;
        }
    }

    private async Task WaitForRecording()
    {
        // Aspetta che la registrazione finisca
        Debug.Log($"Start waiting for {maxRecordingDuration} seconds");
        await Task.Delay(maxRecordingDuration * 1000);      //1000 milliseconds => 1 seconds
        Debug.Log($"Stop waiting");
    }

    private void StartAudioRec() {
        _recordedClip = Microphone.Start(_microphoneDevice, true, maxRecordingDuration, recordingFrequency);
        Debug.Log("Registrazione avviata per " + maxRecordingDuration + " secondi.");
        //AppendInfoText("Registrazione avviata per " + maxRecordingDuration + " secondi.");
    }

    private void StopAudioRecording() {
        if (Microphone.IsRecording(_microphoneDevice))
        {
            Microphone.End(_microphoneDevice); // Ferma il microfono
            Debug.Log("Rec completed.");
        }
        else
        {
            Debug.LogWarning("Rec has been interupted.");
        }
    }

    private async Task<string> RecognizeAudioClip(AudioClip clip)
    {
        if(_recordedClip == null) {
            Debug.LogError("No recorded audio to reproduce.");
            return "";
        }

        Debug.Log("Starting recognition...");
        string result = await ConvertAudioClipToText(clip);
        Debug.Log("Recognized text: " + result);

        return result;
    }

    private async Task<string> ConvertAudioClipToText(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("AudioClip is null!");
            return null;
        }

        // Converte l'AudioClip in byte PCM
        clip = ConvertToMono(clip);
        DebugRecording(clip);

        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        // Controlla se i campioni non sono tutti a zero
        bool hasData = samples.Any(sample => Mathf.Abs(sample) > 0.0001f);
        Debug.Log(hasData ? "L'audio contiene dati validi." : "L'audio è vuoto o contiene solo zeri.");

        byte[] audioData = ConvertToPCM(clip);

        byte[] testData = new byte[audioData.Length];
        Array.Copy(audioData, 40000, testData, 0, 20000);

        Debug.Log($"Primi byte: {string.Join(", ", testData.Take(10000))}");

        if (audioData == null)
        {
            Debug.LogError("Error converting audio.");
            return null;
        }

        // Crea uno stream di input audio
        using var audioInputStream = AudioInputStream.CreatePushStream();
        using var audioConfig = AudioConfig.FromStreamInput(audioInputStream);
        using var recognizer = new SpeechRecognizer(_speechConfig, audioConfig);

        // Alimenta lo stream con i dati audio
        audioInputStream.Write(audioData);
        audioInputStream.Close();

        // Avvia il riconoscimento
        var recognitionResult = await recognizer.RecognizeOnceAsync();

        if (recognitionResult.Reason == ResultReason.RecognizedSpeech)
        {
            return recognitionResult.Text;
        }
        else
        {
            Debug.LogError("Riconoscimento fallito: " + recognitionResult.Reason);
            Debug.LogError($"Con questi dettagli: \nDuarata: {recognitionResult.Duration} | Offset: {recognitionResult.OffsetInTicks}");
            return null;
        }
    }

    private AudioClip ConvertToMono(AudioClip clip)
    {
        float[] data = new float[clip.samples * clip.channels];
        clip.GetData(data, 0);

        float[] monoData = new float[clip.samples];
        for (int i = 0; i < clip.samples; i++)
        {
            monoData[i] = 0;
            for (int c = 0; c < clip.channels; c++)
            {
                monoData[i] += data[i * clip.channels + c];
            }
            monoData[i] /= clip.channels; // Media tra i canali
        }

        AudioClip monoClip = AudioClip.Create(clip.name + "_mono", clip.samples, 1, clip.frequency, false);
        monoClip.SetData(monoData, 0);
        return monoClip;
    }

    public static byte[] ConvertToPCM(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("AudioClip is null!");
            return null;
        }

        // Ottieni i dati grezzi dell'audio
        int samples = clip.samples * clip.channels;
        float[] audioData = new float[samples];
        clip.GetData(audioData, 0);

        // Inizializza il buffer per i dati PCM (16 bit = 2 byte per campione)
        byte[] pcmData = new byte[samples * 2];

        int pcmIndex = 0;
        for (int i = 0; i < samples; i++)
        {
            // Scala il valore del campione da float (-1.0f a 1.0f) a short (-32768 a 32767)
            float gain = 1f;
            short sample = (short)Mathf.Clamp(audioData[i] * 32767f * gain, short.MinValue, short.MaxValue);

            // Scrivi il campione nel buffer PCM (Little Endian: byte meno significativo prima)
            pcmData[pcmIndex++] = (byte)(sample & 0xFF);            // Byte meno significativo
            pcmData[pcmIndex++] = (byte)((sample >> 8) & 0xFF);     // Byte più significativo
        }


        // byte[] testData = new byte[pcmData.Length];
        // Array.Copy(pcmData, 40000, testData, 0, 20000);

        // Debug.Log($"Primi byte in method: {string.Join(", ", testData.Take(10000))}");

        return pcmData;
    }



    void DebugRecording(AudioClip clip)
    {
        Debug.Log($"Clip Samples: {clip.samples}");
        Debug.Log($"Clip Channels: {clip.channels}");
        Debug.Log($"Clip Frequency: {clip.frequency}");

        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 40000);
        Debug.Log($"Primi campioni: {string.Join(", ", samples.Take(1000))}");
    }
}