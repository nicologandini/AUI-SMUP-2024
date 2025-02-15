using UnityEngine;

namespace SMUP.Audio {
    [CreateAssetMenu(fileName = "SoundBank", menuName = "Scriptable Objects/SoundBank")]
    public class SoundBankSO : ScriptableObject
    {
        public AudioClipData[] buttonSFX;
        public AudioClipData[] wrongMatchSFX;
        public AudioClipData[] rightMatchSFX;
        public AudioClipData[] leaveBalloonSFX;
        public AudioClipData[] pickupBalloonSFX;



        public AudioClipData GetButtonSFX() {
            if(buttonSFX.Length == 0) {return null;}

            // Get a random SFX in the array
            return buttonSFX[UnityEngine.Random.Range(0, buttonSFX.Length)];    
        }
        public AudioClipData GetWrongMatchSFX() {
            if(wrongMatchSFX.Length == 0) {return null;}

            // Get a random SFX in the array
            return wrongMatchSFX[UnityEngine.Random.Range(0, wrongMatchSFX.Length)];    
        }
        public AudioClipData GetRightMatchSFX() {
            if(rightMatchSFX.Length == 0) {return null;}

            // Get a random SFX in the array
            return rightMatchSFX[UnityEngine.Random.Range(0, rightMatchSFX.Length)];    
        }
        public AudioClipData GetLeaveBalloonSFX() {
            if(leaveBalloonSFX.Length == 0) {return null;}

            // Get a random SFX in the array
            return leaveBalloonSFX[UnityEngine.Random.Range(0, leaveBalloonSFX.Length)];    
        }
        public AudioClipData GetPickupBalloonSFX() {
            if(pickupBalloonSFX.Length == 0) {return null;}

            // Get a random SFX in the array
            return pickupBalloonSFX[UnityEngine.Random.Range(0, pickupBalloonSFX.Length)];    
        }
    }
}