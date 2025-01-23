using System.Collections;
using System.Collections.Generic;
using Microsoft.CognitiveServices.Speech;
using Photon.Pun;
using TMPro;
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

        [Header ("Parameters")]
        [SerializeField] private InputActionManager inputManager;
        //[SerializeField] private InputAction actionBinding;
        [SerializeField] private float speechTimeOut = 60f;

        [Header("Multiplayer")]
        [SerializeField] private PhotonView photonView;

        [Header("AI Avatar")]
        [SerializeField] private AIAvatar_Manager avatarManager;

        [Header("Debug")]
        [SerializeField] private TextMeshProUGUI infoText;
        



        private InputAction actionBinding;


        private bool canTalk = false;


        // Start is called before the first frame update
        void Start()
        {
            //startRecoButton.onClick.AddListener(() => StartSpeechPipeline());
            print($"Starting AI_Pipeline");
            ShowInfoText("Starting AI");
            canTalk = true;
            actionBinding = inputManager.actionAssets[0].FindActionMap("Main").FindAction("X Constraint");          //actionMaps[0].actions[18];

            print($"actionBinding: {actionBinding}");
            AppendInfoText($"actionBinding: {actionBinding}");

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

        async void StartSpeechPipeline()
        {
            if (!canTalk) { return; }
            if (photonView != null) {
                photonView.RPC("SetTalkLock", RpcTarget.Others, false);  
            }   
            canTalk= false;
            print("talking");
            AppendInfoText("talking");
            SetAvatarCloud(CloudType.THINKING_CLOUD, true);
            
            string text = await sst.SpeechToText(actionBinding, speechTimeOut);
            if(text ==  null || text == "") {
                Debug.Log("NO valid text found!");
                SetAvatarCloud(CloudType.NONE, false);
                return;
            }

            text = $"Utente1 dice: {text}";
            string response = await ai_Conversation.SubmitChat(text);

            SetAvatarCloud(CloudType.OK_CLOUD, true);
            await tts.TextToSpeech(response);


            canTalk = true;
            if (photonView != null) {
                photonView.RPC("SetTalkLock", RpcTarget.Others, true);  
            }   
            print("Can talk again");
            AppendInfoText("Can taalk again");
            SetAvatarCloud(CloudType.NONE, false);
        }

        async void Test() {
            print("inizio risposta");
            string response = await ai_Conversation.SubmitChat("ciao");
            print(response);
        }

        [PunRPC] 
        private void SetTalkLock(bool value) {
            canTalk = value;

            if(!canTalk) {
                SetAvatarCloud(CloudType.NEGATIVE_CLOUD, true);
            } else {
                SetAvatarCloud(CloudType.NONE, false);
            }

            print($"Lock AI set to {value} by other");
        }

        private void SetAvatarCloud(CloudType cloudType, bool value) {
            if (avatarManager == null) { return; }

            avatarManager.SetAllCloud(false);
            avatarManager.SetCloud(cloudType, value);
        }
        

        private void ShowInfoText(string text) {
        if(infoText == null) { return; }

        infoText.text = text;
        }

        private void AppendInfoText(string text) {
            if(infoText == null) { return; }

            infoText.text += "\n" + text;
        }
    }
}