using JetBrains.Annotations;
using Meta.WitAi;
using Meta.WitAi.CallbackHandlers;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace SMUP.AI {
    public class AIAvatar_Manager : MonoBehaviour
    {
        [SerializeField] private GameObject thinkingCloud;
        [SerializeField] private GameObject okCloud;
        [SerializeField] private GameObject negativeCloud;
        [SerializeField] private GameObject listeningCloud;


        private void Start() {
            SetThinkingCloud(false);
            SetNegativeCloud(false);
            SetListeningCloud(false);
            SetOkCloud(false);
        }


        public void SetAllCloud(bool value) {
            SetNegativeCloud(value);
            SetThinkingCloud(value);
            SetListeningCloud(value);
            SetOkCloud(value);
        }

        public void SetCloud(CloudType cloudType, bool value) {
            switch(cloudType) {
                case CloudType.OK_CLOUD:
                    SetOkCloud(value);
                    break;
                
                case CloudType.NEGATIVE_CLOUD:
                    SetNegativeCloud(value);
                    break;

                case CloudType.THINKING_CLOUD:
                    SetThinkingCloud(value);
                    break;

                case CloudType.LISTENING_CLOUD:
                    SetListeningCloud(value);
                    break;

                default:
                    SetAllCloud(false);
                    break;
            }
        }


        private void SetThinkingCloud(bool value) {
            if(thinkingCloud == null) { return; }

            thinkingCloud.SetActive(value);
        }
        private void SetOkCloud(bool value) {
            if(okCloud == null) { return; }

            okCloud.SetActive(value);
        }
        private void SetNegativeCloud(bool value) {
            if(negativeCloud == null) { return; }

            negativeCloud.SetActive(value);
        }
        private void SetListeningCloud(bool value) {
            if(listeningCloud == null) { return; }

            listeningCloud.SetActive(value);
        }
    }
}

public enum CloudType {
    NONE, OK_CLOUD, NEGATIVE_CLOUD, THINKING_CLOUD, LISTENING_CLOUD
}
