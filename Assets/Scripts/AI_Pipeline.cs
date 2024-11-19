using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace AI_Pipeline {
    public class AI_Pipeline : MonoBehaviour
    {
        [SerializeField] private AI_STT sst;
        [SerializeField] private AI_TTS tts;
        [SerializeField] private AI_Conversation ai_Conversation;

        [SerializeField] private InputActionManager inputManager;
        [SerializeField] private Button startRecoButton;


        private bool canTalk = false;


        // Start is called before the first frame update
        void Start()
        {
            //startRecoButton.onClick.AddListener(() => StartSpeechPipeline());
            canTalk = true;
            
        }

        void Update() {
            if(inputManager.actionAssets[0].actionMaps[0].actions[18].ReadValue<float>() > 0) {
                //Test();
                StartSpeechPipeline();
            }
        }

        async void StartSpeechPipeline()
        {
            if (!canTalk) { return; }
            
            print("talking");
            canTalk= false;
            string text = await sst.SpeechToText();
            string response = await ai_Conversation.SubmitChat(text);
            await tts.TextToSpeech(response);

            print("Can talk again");
            canTalk = true;
        }

        async void Test() {
            print("inizio risposta");
            string response = await ai_Conversation.SubmitChat("ciao");
            print(response);
        }
    }
}