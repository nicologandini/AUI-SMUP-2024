using UnityEngine;

[CreateAssetMenu(fileName = "SpeechBank_SO", menuName = "Scriptable Objects/SpeechBank_SO")]
public class SpeechBank_SO : ScriptableObject
{
    [SerializeField] [TextArea(3,10)] private string[] welcomeStrings;
    [SerializeField] [TextArea(3,10)] private string[] wrongMatchStrings;
    [SerializeField] [TextArea(3,10)] private string[] correctMatchStrings;


    public string GetSpeech(SpeechType speechType) {
        return speechType switch
        {
            SpeechType.StartingSpeech => GetRandomString(welcomeStrings),
            SpeechType.WrongMatchSpeech => GetRandomString(wrongMatchStrings),
            SpeechType.CorrectMatchSpeech => GetRandomString(correctMatchStrings),
            _ => null,
        };
    }

    private string GetRandomString(string[] refStrings) {
        if(refStrings == null || refStrings.Length == 0) {return null;}

        return refStrings[UnityEngine.Random.Range(0, refStrings.Length)];
    }
}

public enum SpeechType {
    NONE = -1, StartingSpeech, CorrectMatchSpeech, WrongMatchSpeech
} 