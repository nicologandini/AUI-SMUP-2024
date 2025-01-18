using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace AI_Pipeline {
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


        private InputAction actionBinding;


        private bool canTalk = false;


        // Start is called before the first frame update
        void Start()
        {
            //startRecoButton.onClick.AddListener(() => StartSpeechPipeline());
            print($"Starting AI_Pipeline");
            canTalk = true;
            actionBinding = inputManager.actionAssets[0].FindActionMap("Main").FindAction("X Constraint");          //actionMaps[0].actions[18];
            print($"actionBinding: {actionBinding}");
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
            canTalk= false;
            print("talking");
            
            string text = await sst.SpeechToText(actionBinding, speechTimeOut);
            text = $"Utente1 dice: {text}";
            string response = await ai_Conversation.SubmitChat(text);
            await tts.TextToSpeech(response);

            canTalk = true;
            print("Can talk again");
        }

        async void Test() {
            print("inizio risposta");
            string response = await ai_Conversation.SubmitChat("ciao");
            print(response);
        }
    }
}