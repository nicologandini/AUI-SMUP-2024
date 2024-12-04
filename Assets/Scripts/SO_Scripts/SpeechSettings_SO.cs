using UnityEngine;

[CreateAssetMenu(fileName = "SpeechSettings_SO", menuName = "AI/SpeechSettings_SO", order = 0)]
public class SpeechSettings_SO : ScriptableObject {
    public string speechAPIKey = "";
    public string region = "";
    public string recognitionLanguage = "it-IT";
    public string voiceName = "en-US-AvaMultilingualNeural";
}
