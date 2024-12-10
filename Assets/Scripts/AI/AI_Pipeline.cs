using UnityEngine;
using UnityEngine.InputSystem;


namespace SMUP_AI {
    public class AI_Pipeline : MonoBehaviour
    {
        public event System.Action<bool> OnTalkingChanged;

        [Header("Components")]
        [SerializeField] private AI_STT_continuous sst;
        [SerializeField] private AI_TTS tts;
        [SerializeField] private AI_Conversation ai_Conversation;

        [Header ("Parameters")]
        [SerializeField] private float speechTimeOut = 60f;

        [Header ("Interaction")]
        [SerializeField] private InputActionReference actionRef;


        private InputAction actionBinding;


        private bool canTalk = false;


        // Start is called before the first frame update
        void Start()
        {
            canTalk = true;
            OnTalkingChanged?.Invoke(!canTalk);

            actionBinding = actionRef.action;
        }

        void Update() {
            if(actionRef.action.ReadValue<float>() > 0) {
                //Test();
                StartSpeechPipeline();
            }
        }

        async void StartSpeechPipeline()
        {
            if (!canTalk) { return; }
            canTalk= false;
            OnTalkingChanged?.Invoke(!canTalk);
            print("talking");
            
            string text = await sst.SpeechToText(actionBinding, speechTimeOut);
            OnTalkingChanged?.Invoke(false);
            text = $"Utente1 dice: {text}";
            string response = await ai_Conversation.SubmitChat(text);
            await tts.TextToSpeech(response);

            canTalk = true;
            //OnTalkingChanged?.Invoke(!canTalk);
            print("Can talk again");
        }

        async void Test() {
            print("inizio risposta");
            string response = await ai_Conversation.SubmitChat("ciao");
            print(response);
        }
    }
}