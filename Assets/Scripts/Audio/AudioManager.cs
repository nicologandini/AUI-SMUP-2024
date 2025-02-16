using System.Collections;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.Audio;

namespace SMUP.Audio {
    public class AudioManager : MonoBehaviour {
        private static AudioManager _instance;
        public static AudioManager Instance {
            get {
                if (_instance != null) {
                    return _instance;
                } else {
                    GameObject speechManager_GO = new GameObject("audioManagerAuto");
                    _instance = speechManager_GO.AddComponent<AudioManager>();
                    return _instance;
                }
            }
            private set {
                _instance = value;
            }
        }

        [SerializeField] private AudioMixer audioMixer; 
        [SerializeField] private AudioSource musicSource1;
        [SerializeField] private AudioSource musicSource2;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private SoundBankSO soundBankSo;
        [SerializeField] private AudioManagerSettingsSO audioManagerSettingsSo;
        [SerializeField] private bool useMixer = true;

        [SerializeField] private float pitchRandomRange = 0.1f;
        


        private bool _isEnterSfxPlayed; // If an sfx has an enter sound and an exit sound this will help understand if it has already played the enter sound

        public static AudioSource[] aud = new AudioSource[2];
        //Determine which audio source is the current one
        private bool activeMusicSource;
        //We will store the transition as a Coroutine so that we have the ability to stop it halfway if necessary
        IEnumerator musicTransition;

        private Coroutine musicLoopCoroutine;


            
        void Awake()
        {
            if(_instance == null) {
                _instance = this;
            } else {
                Destroy(this);
            }
        }



        public void SetMasterVolume(float volume)
        {
            if (useMixer)
            {
                // volume for AudioSource is between 0 and 1, 
                // it is from -80 to -20 in the mixer and must be normalized
                float mixerVolume = AudioManager.SliderToDB(volume, audioManagerSettingsSo.MasterMaxDb);
                audioMixer.SetFloat ("MasterVolume", mixerVolume);
            }
            else
            {
                // there is no master volume without mixer
            }
        }

        public void SetMusicVolume(float volume)
        {
            if (useMixer)
            {
                float mixerVolume = AudioManager.SliderToDB(volume, audioManagerSettingsSo.MusicMaxDb);
                audioMixer.SetFloat ("MusicVolume", mixerVolume);
            }
            else
            {
                volume = Mathf.Clamp(volume, 0f, 1f);
                aud[activeMusicSource ? 0 : 1].volume = volume;
            }
        }

        public void SetSfxVolume(float volume)
        {
            if (useMixer)
            {
                float mixerVolume = AudioManager.SliderToDB(volume, audioManagerSettingsSo.SfxMaxDb);
                audioMixer.SetFloat ("SFXVolume", mixerVolume);
            }
            else
            {
                volume = Mathf.Clamp(volume, 0f, 1f);
                sfxSource.volume = volume;
            }
        }

        public void PlayAudioEnum(AudioClipEnum clipEnum) {
            if(sfxSource == null) {
                Debug.LogWarning("Trying to play a SFX but no Source is assign");
                return;
            }

            AudioClipData audioClipData = GetAudioClip(clipEnum);
            if(audioClipData == null) {
                Debug.LogWarning("No AudioClip Found");
                return;
            }

            sfxSource.clip = audioClipData.audioClip;
            sfxSource.pitch = audioClipData.pitchOffset + Random.Range(-pitchRandomRange, pitchRandomRange);
            sfxSource.volume = audioClipData.audioVolume;

            sfxSource.Play();
        }

        public void PlayAudioClip(AudioClip audioClip) {
            if(sfxSource == null) {
                Debug.LogWarning("Trying to play a SFX but no Source is assign");
                return;
            }

            if(audioClip == null) {
                Debug.LogWarning("No AudioClip Found");
                return;
            }

            sfxSource.Stop();
            sfxSource.clip = audioClip;
            sfxSource.pitch = 1 + Random.Range(-pitchRandomRange, pitchRandomRange);
            //sfxSource.volume = 

            sfxSource.Play();
        }

        public static float SliderToDB(float volume, float maxDB=-10, float minDB=-80)
        {
            float dbRange = maxDB - minDB;
            return minDB + volume * dbRange;
        }


        private AudioClipData GetAudioClip(AudioClipEnum clipEnum) {
            return clipEnum switch
            {
                AudioClipEnum.Button => soundBankSo.GetButtonSFX(),
                AudioClipEnum.WrongMatch => soundBankSo.GetWrongMatchSFX(),
                AudioClipEnum.RightMatch => soundBankSo.GetRightMatchSFX(),
                AudioClipEnum.BalloonPickup => soundBankSo.GetPickupBalloonSFX(),
                AudioClipEnum.BalloonDrop => soundBankSo.GetLeaveBalloonSFX(),
                _ => null,
            };
        }

        // Custom play methods
        public void PlayBalloonPickupSFX()
        {
            PlayAudioEnum(AudioClipEnum.BalloonPickup);
        }
        public void PlayBalloonDropSFX()
        {
            PlayAudioEnum(AudioClipEnum.BalloonDrop);
        }
        public void PlayButtonPressSFX() {
            PlayAudioEnum(AudioClipEnum.Button);
        }
    }


    public enum AudioClipEnum{
        NONE = -1, Button = 0, WrongMatch, RightMatch, BalloonPickup, BalloonDrop, 
    }
}
