using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using UnityEngine;

public class AI_TTS : MonoBehaviour
{
    [SerializeField] private SpeechSettings_SO speechSettings_SO;


    public async Task TextToSpeech(string text)
    {
        SpeechConfig config = SpeechConfig.FromSubscription(speechSettings_SO.speechAPIKey, speechSettings_SO.region);
        config.SpeechSynthesisVoiceName = speechSettings_SO.voiceName; 


        using (var speechSynthesizer = new SpeechSynthesizer(config))
        {
            SpeechSynthesisResult speechSynthesisResult = await speechSynthesizer.SpeakTextAsync(text);
            OutputSpeechSynthesisResult(speechSynthesisResult, text);
        }

        return;
    }

     void OutputSpeechSynthesisResult(SpeechSynthesisResult speechSynthesisResult, string text)
    {
        switch (speechSynthesisResult.Reason)
        {
            case ResultReason.SynthesizingAudioCompleted:
                print($"Speech synthesized for text: [{text}]");
                break;
            case ResultReason.Canceled:
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechSynthesisResult);
                print($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    print($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    print($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                    print($"CANCELED: Did you set the speech resource key and region values?");
                }
                break;
            default:
                break;
        }
    }
}
