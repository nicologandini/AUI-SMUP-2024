using UnityEngine;

namespace SMUP.Audio {
    [CreateAssetMenu(fileName = "AudioClipData", menuName = "Scriptable Objects/AudioClipData")]
    public class AudioClipData : ScriptableObject {
        public AudioClip audioClip;
        [Range(0,1)] public float audioVolume = 1;
        [Range(0,2)] public float pitchOffset = 1;
    }
}