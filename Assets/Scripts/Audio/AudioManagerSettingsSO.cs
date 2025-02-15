using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMUP.Audio  {
    [CreateAssetMenu(menuName = "AudioManager/AudioManagerSettings")]
    public class AudioManagerSettingsSO : ScriptableObject
    {
        [Range(-80f,20f)]
        public float SfxMaxDb = 0f;

        [Range(-80f,20f)]
        public float MusicMaxDb = -10f;

        [Range(-80f,20f)]
        public float MasterMaxDb = 10f;
    }
}