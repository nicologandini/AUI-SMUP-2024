using UnityEngine;

namespace SMUP.AI {
    [CreateAssetMenu(fileName = "TextTTS_SO", menuName = "Scriptable Objects/TextTTS_SO")]
    public class TextTTS_SO : ScriptableObject
    {
        [TextArea(3, 10)] public string text;       
    }
}