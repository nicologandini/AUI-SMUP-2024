using System.Collections;
using System.Collections.Generic;
using it.polimi.smup2;
using Microsoft.CognitiveServices.Speech;
using Photon.Pun;
using TMPro;
//using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace SMUP.AI {
    public class AI_Pipeline : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private AI_STT_Android sst;
        [SerializeField] private AI_TTS tts;
        [SerializeField] private AI_Conversation ai_Conversation;

        [SerializeField] private DirectSpeechManager directSpeech;

        [Header ("Parameters")]
        [SerializeField] private InputActionManager inputManager;
        //[SerializeField] private InputAction actionBinding;
        [SerializeField] private float speechTimeOut = 60f;

        [Header("Multiplayer")]
        [SerializeField] private PhotonView photonView;

        [Header("AI Avatar")]
        [SerializeField] private AIAvatar_Manager avatarManager;

        [Header("Debug")]
        [SerializeField] private bool isDebug;
        

        public AI_TTS TTS => tts;

        private InputAction actionBinding;


        private bool canTalk = false;
        private bool isAIPerforming = false;    //usato per controllare se directspeech puo eseguire

        [HideInInspector] public string PlayerName = "";


        // Start is called before the first frame update
        void Start()
        {
            //startRecoButton.onClick.AddListener(() => StartSpeechPipeline());
            print($"Starting AI_Pipeline");
            if (isDebug) {DebugDialogue.Instance.ShowInfoText("Starting AI");}
            canTalk = true;
            actionBinding = inputManager.actionAssets[0].FindActionMap("Main").FindAction("X Constraint");          //actionMaps[0].actions[18];

            print($"actionBinding: {actionBinding}");
            if (isDebug) {DebugDialogue.Instance.AppendInfoText($"actionBinding: {actionBinding}");}

            if(photonView == null) {
                photonView = PhotonView.Get(this);
            }

            if (avatarManager != null) {
                avatarManager.SetAllCloud(false);
            }
        }

        void Update() {
            if(actionBinding.ReadValue<float>() > 0) {
                //Test();
                StartSpeechPipeline();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if AI sucessfully blocked. False if AI is already talking</returns>
        public bool SetPipelineStatus(bool value) {
            if(isAIPerforming) {return false;}

            if(value) {
                SetTalkState(true);
                SetAvatarCloud(CloudType.NONE, false);
            } else {
                SetTalkState(false);
                SetAvatarCloud(CloudType.NEGATIVE_CLOUD, true);
            }

            return true;
        }


        async void StartSpeechPipeline()
        {
            if (!canTalk) { return; }
            isAIPerforming = true;
            SetTalkState(false);
            SetAvatarCloud(CloudType.LISTENING_CLOUD, true);
            
            string text = await sst.SpeechToText(actionBinding, speechTimeOut);
            if(text ==  null || text == "") {
                Debug.Log("NO valid text found!");
                SetTalkState(true);
                SetAvatarCloud(CloudType.NONE, false);
                isAIPerforming = false;
                return;
            }

            SetAvatarCloud(CloudType.THINKING_CLOUD, true);

            if(PlayerName != null && PlayerName != "") {
                text = $"{PlayerName} dice: {text}";
            }
            string response = await ai_Conversation.SubmitChat(text);

            SetAvatarCloud(CloudType.OK_CLOUD, true);
            await tts.TextToSpeech(response);

            SetTalkState(true);
            SetAvatarCloud(CloudType.NONE, false);
            isAIPerforming = false;
        }

        async void Test() {
            print("inizio risposta");
            string response = await ai_Conversation.SubmitChat("ciao");
            print(response);
        }


        [PunRPC] 
        private void SetTalkLock(bool value) {
            canTalk = value;

            if(directSpeech != null) {
                directSpeech.IsTalking = !value;
            }

            if(!canTalk) {
                SetAvatarCloud(CloudType.NEGATIVE_CLOUD, true);
            } else {
                SetAvatarCloud(CloudType.NONE, false);
            }

            print($"Lock AI set to {value} by other");
        }

        public void SetAvatarCloud(CloudType cloudType, bool value) {
            if (avatarManager == null) { return; }

            avatarManager.SetAllCloud(false);
            avatarManager.SetCloud(cloudType, value);
        }

        private void SetTalkState(bool state) {
            canTalk = state;

            if (photonView != null) {
                photonView.RPC("SetTalkLock", RpcTarget.Others, state);  
            }   
            if(directSpeech != null) {
                directSpeech.IsTalking = !state;
            }
            
            print(state?"Can talk again":"Talking to AI");
            if (isDebug) {DebugDialogue.Instance.AppendInfoText("talking");}
        }
    }
}