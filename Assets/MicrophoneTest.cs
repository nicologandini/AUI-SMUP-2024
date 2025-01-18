using System.Collections;
using TMPro;
using UnityEngine;

public class MicrophoneTest : MonoBehaviour
{
    [SerializeField] private int maxRecordingDuration = 10;      // Durata della registrazione in secondi
    [SerializeField] private int recordingFrequency = 44100;    // Frequenza di campionamento
    [SerializeField] private TextMeshProUGUI infoText;


    private AudioSource _audioSource;
    private string _microphoneDevice;
    private AudioClip _recordedClip;


    void Start()
    {
        ShowInfoText("");

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
            _microphoneDevice = devices[0]; // Cambia l'indice per scegliere un microfono specifico
            Debug.Log("Microfono selezionato: " + _microphoneDevice);
            AppendInfoText("Microfono selezionato: " + _microphoneDevice);
        }
        else
        {
            Debug.LogError("Nessun microfono trovato!");
            AppendInfoText("Nessun microfono trovato!");
            return;
        }

        Invoke("StartAudioTest", 3f);
    }

    private void StartAudioTest() {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.loop = true;

        _recordedClip = Microphone.Start(_microphoneDevice, true, maxRecordingDuration, recordingFrequency);
        Debug.Log("Registrazione avviata per " + maxRecordingDuration + " secondi.");
        AppendInfoText("Registrazione avviata per " + maxRecordingDuration + " secondi.");

        // while (Microphone.GetPosition(_microphoneDevice) <= 0) { } // Aspetta che il microfono inizi
        // _audioSource.Play();

        StartCoroutine(WaitForRecording());
    }

    private IEnumerator WaitForRecording()
    {
        // Aspetta che la registrazione finisca
        yield return new WaitForSeconds(maxRecordingDuration);

        // Controlla se il microfono sta ancora registrando
        if (Microphone.IsRecording(_microphoneDevice))
        {
            Microphone.End(_microphoneDevice); // Ferma il microfono
            Debug.Log("Registrazione completata.");
            AppendInfoText("Registrazione completata.");
        }
        else
        {
            Debug.LogWarning("Il microfono ha interrotto la registrazione prima del termine.");
            AppendInfoText("Il microfono ha interrotto la registrazione prima del termine.");
        }

        // Riproduci il clip registrato
        PlayRecordedAudio();
    }

    private void PlayRecordedAudio()
    {
        if (_recordedClip == null)
        {
            Debug.LogError("Nessun audio registrato da riprodurre.");
            AppendInfoText("<color=red>Nessun audio registrato da riprodurre.</color>");
            return;
        }

        // Imposta il clip registrato come sorgente audio
        _audioSource.clip = _recordedClip;

        // Riproduci l'audio
        _audioSource.Play();
        Debug.Log("Riproduzione audio avviata.");
        AppendInfoText("Riproduzione audio avviata.");
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