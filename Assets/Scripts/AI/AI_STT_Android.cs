using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace SMUP.AI {
    public class AI_STT_Android : MonoBehaviour {
        [SerializeField] private SpeechSettings_SO speechSettings_SO;
        [SerializeField] private AI_Pipeline pipeline;


        [Header("Recoding Values")]
        [SerializeField] private float maxRecordingDuration = 120; // Durata della registrazione in secondi
        [SerializeField] private int recordingFrequency = 44100; // Frequenza di campionamento (44100 Hz è standard)
        [SerializeField] private int silenceBeforeStopMilliseconds = 3000;

        [Header ("Debug")]
        [SerializeField] private AudioSource audioSource;


        private string _microphoneDevice;
        private AudioClip _recordedClip;
        private SpeechConfig _speechConfig;
        private bool _isRecognizing;
        private bool _stopRequested;
        private bool _skipFirstFrame;
        private UnityEngine.InputSystem.InputAction _actionBinding;


        void Start() {
            InitializeSpeechRecognizer();

            // Testing stuff
            // Invoke("SpeechToTextInvoke" ,3f);
        }

        private void SpeechToTextInvoke() {
            SpeechToText(null, 20f);
        }

        private void Update() {
            if (_isRecognizing) {
                if (_actionBinding == null) {return;}
                if (_skipFirstFrame) {
                    _skipFirstFrame = false;
                    return;
                }

                if (_actionBinding.ReadValue<float>() > 0 && _actionBinding.WasPressedThisFrame()) {
                    print("Talk button pressed");
                    _isRecognizing = false;
                    _stopRequested = true;
                }
            }
        }

        public void ResetRecognitionState() {
            _isRecognizing = false;
        }


        public async Task<string> SpeechToText(UnityEngine.InputSystem.InputAction actionBinding, float timeout = 30f) {
            if (!_isRecognizing) {
                _skipFirstFrame = true;
                _stopRequested = false;
                _actionBinding = actionBinding;
                _isRecognizing = true;

                StartAudioRec();
                await WaitUntilTimeoutOrStopRequested(math.min(timeout, maxRecordingDuration));

                StopAudioRecording();
                string text = await RecognizeAudioClip(_recordedClip);

                ResetRecognitionState();
                return text;
            } else {
                ResetRecognitionState();
                return "";
            }
        }

        private async Task WaitUntilTimeoutOrStopRequested(float timeout) {
            float elapsedTime = 0f;

            while (elapsedTime < timeout && !_stopRequested) {
                await Task.Delay(100); // Aspetta 100ms prima di controllare di nuovo
                elapsedTime += 0.1f; // Incrementa il tempo trascorso
            }

            _stopRequested = false;
        }


        void InitializeSpeechRecognizer()
        {
            _speechConfig = SpeechConfig.FromSubscription(speechSettings_SO.speechAPIKey, speechSettings_SO.region);
            _speechConfig.SpeechRecognitionLanguage = speechSettings_SO.recognitionLanguage;
            _speechConfig.SetProperty(PropertyId.Speech_SegmentationSilenceTimeoutMs, silenceBeforeStopMilliseconds.ToString());
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
            await Task.Delay(Mathf.CeilToInt(maxRecordingDuration) * 1000);      //1000 milliseconds => 1 seconds
            Debug.Log($"Stop waiting");
        }

        private void StartAudioRec() {
            _recordedClip = Microphone.Start(_microphoneDevice, true, Mathf.CeilToInt(maxRecordingDuration), recordingFrequency);
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

            byte[] audioData;

            int targetFrequency = 16000;
            if(clip.frequency != targetFrequency)
                audioData = ConvertToPCMWithResample(clip);
            else 
                audioData = ConvertToPCM(clip);

            //AudioDebug(audioData);

            // AudioClip testClip = CreateClipFromPCM("testClip", clip.samples , clip.channels);
            // PlayClipDebug(testClip);

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
            if (pipeline != null) {
                pipeline.SetAvatarCloud(CloudType.THINKING_CLOUD, true);
            }
            var recognitionResult = await recognizer.RecognizeOnceAsync();

            if (recognitionResult.Reason == ResultReason.RecognizedSpeech)
            {
                Debug.Log($"Recognition completata con questi info => \nDuarata: {recognitionResult.Duration} | Offset: {recognitionResult.OffsetInTicks}");
                return recognitionResult.Text;
            }
            else
            {
                Debug.LogError("Riconoscimento fallito: " + recognitionResult.Reason);
                Debug.LogError($"Con questi dettagli: \nDuarata: {recognitionResult.Duration} | Offset: {recognitionResult.OffsetInTicks}");
                return null;
            }
        }

        private static void AudioDebug(byte[] audioData)
        {
            byte[] testData = new byte[audioData.Length];
            Array.Copy(audioData, 40000, testData, 0, 20000);

            Debug.Log($"Primi byte: {string.Join(", ", testData.Take(10000))}");
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

        public byte[] ConvertToPCMWithResample(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogError("AudioClip is null!");
                return null;
            }

            // Parametri per il nuovo sample rate
            int originalSampleRate = clip.frequency; // Frequenza originale
            int targetSampleRate = 16000;            // Frequenza desiderata (16 kHz)

            // Ottieni i dati grezzi dell'audio
            int originalSamples = clip.samples * clip.channels;
            float[] audioData = new float[originalSamples];
            clip.GetData(audioData, 0);

            // Calcola il nuovo numero di campioni dopo il downsampling
            float resampleRatio = (float)targetSampleRate / originalSampleRate;
            int resampledSamples = Mathf.CeilToInt(originalSamples * resampleRatio);

            // Esegui il downsampling
            float[] resampledData = ResampleAudio(audioData, clip.channels, originalSampleRate, targetSampleRate);

            // Inizializza il buffer per i dati PCM (16 bit = 2 byte per campione)
            byte[] pcmData = new byte[resampledData.Length * 2];

            int pcmIndex = 0;
            for (int i = 0; i < resampledData.Length; i++)
            {
                // Scala il valore del campione da float (-1.0f a 1.0f) a short (-32768 a 32767)
                float gain = 1f;
                short sample = (short)Mathf.Clamp(resampledData[i] * 32767f * gain, short.MinValue, short.MaxValue);

                // Scrivi il campione nel buffer PCM (Little Endian: byte meno significativo prima)
                pcmData[pcmIndex++] = (byte)(sample & 0xFF);            // Byte meno significativo
                pcmData[pcmIndex++] = (byte)((sample >> 8) & 0xFF);     // Byte più significativo
            }

            return pcmData;
        }

        public byte[] ConvertToPCM(AudioClip clip)
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

                // if(i % 100000 == 0) {
                //     print("-----------------------------");
                //     print($"{i}th sample has value {audioData[i]} => {sample}");
                //     print($"with pcmData values: {pcmData[--pcmIndex]} {pcmData[--pcmIndex]}");
                //     print("-----------------------------");

                //     pcmIndex += 2;
                // }
            }

            //PlayClipDebug(CreateClipFromPCM("testClip", clip.samples, clip.channels, ConvertPCM16ToFloat(pcmData)));

            return pcmData;
        }

        private float[] ResampleAudio(float[] audioData, int channels, int originalSampleRate, int targetSampleRate)
        {
            float resampleRatio = (float)targetSampleRate / originalSampleRate;
            int resampledLength = Mathf.CeilToInt(audioData.Length * resampleRatio / channels) * channels;
        try {
            float[] resampledData = new float[resampledLength];

            for (int i = 0; i < resampledLength / channels; i++)
            {
                // Calcola l'indice del campione originale corrispondente
                float originalIndex = i / resampleRatio;
                int originalIndexFloor = Mathf.FloorToInt(originalIndex);
                int originalIndexCeil = Mathf.Min(originalIndexFloor + 1, (audioData.Length / channels) - 1);

                float t = originalIndex - originalIndexFloor;

                // Interpolazione lineare per ogni canale
                for (int channel = 0; channel < channels; channel++)
                {
                    float sample1 = audioData[originalIndexFloor * channels + channel];
                    float sample2 = audioData[originalIndexCeil * channels + channel];
                    resampledData[i * channels + channel] = Mathf.Lerp(sample1, sample2, t);
                }
            }

            return resampledData;
        } catch (IndexOutOfRangeException e) {
            print($"Lengths => original: {audioData.Length} | Resampled: {resampledLength} | channels: {channels}");
            Debug.LogError(e.Message);
            Debug.LogError(e.StackTrace);
            return null;
        }
        }

        float[] ConvertPCM16ToFloat(byte[] byteArray)
        {
            int sampleCount = byteArray.Length / 2; // 2 byte per campione
            float[] floatArray = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                int byteIndex = i * 2;
                short sample = (short)(byteArray[byteIndex] | (byteArray[byteIndex + 1] << 8)); // Unisci i due byte in un short
                floatArray[i] = sample / 32768f; // Normalizza in un range tra -1.0f e 1.0f
            }

            return floatArray;
        }

        private AudioClip CreateClipFromPCM(string clipName, int samples, int channels, float[] data)
        {
            AudioClip testClip = AudioClip.Create(clipName, samples, channels, recordingFrequency, false);
            testClip.SetData(data, 0);

            return testClip;
        }

        private void PlayClipDebug(AudioClip clip) {
            if (audioSource == null) {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.loop = true;
            audioSource.clip = clip;
            audioSource.Play();
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
}