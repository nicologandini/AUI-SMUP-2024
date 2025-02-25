using System.Threading.Tasks;
using SMUP.AI;
using UnityEngine;

public class DirectSpeechManager : MonoBehaviour
{
    private static DirectSpeechManager _instance;
    public static DirectSpeechManager Instance {
        get {
            return _instance;
        }
        private set {
            _instance = value;
        }
    }


    [SerializeField] private AI_Pipeline pipeline;
    [SerializeField] private AI_TTS tts;
    [SerializeField] private SpeechBank_SO speechBank;
    [SerializeField] private bool useInBetweenSpeech = false;

    public bool IsTalking = false;


    void Awake()
    {
        if(_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (pipeline == null) {
            Debug.LogWarning("No ai pipeline for DirectSpeechManager, trying to get an existing one");
            pipeline = FindFirstObjectByType<AI_Pipeline>();

            if (pipeline == null) {return;}
        }

        if (tts == null) {
            tts = pipeline.TTS;
        }
    }


    public async Task<bool> StartSpeech(TextTTS_SO textSO, float startDelaySecond = 0f) {
        if(!useInBetweenSpeech) {return false;}
        if(IsTalking) {return false;}

        if(textSO == null || textSO.text == "") {
            Debug.LogWarning("Tried to start a speech but the Text is null or empty!");
            return false;
        }
        if(!pipeline.SetPipelineStatus(false)) {return false;}
        await Task.Delay((int) (startDelaySecond * 1000));      //1000ms => 1s

        IsTalking = true;
        await tts.TextToSpeech(textSO.text);
        pipeline.SetPipelineStatus(true);
        IsTalking = false;

        return true;
    }

    public async Task<bool> StartSpeech(SpeechType speechType, float startDelaySecond = 0f) {
        if(!useInBetweenSpeech) {return false;}
        if(IsTalking) {return false;}

        if(speechBank == null) {
            Debug.LogWarning("Tried to start a speech with wrong settings!");
            return false;
        }

        string speechText = speechBank.GetSpeech(speechType);
        if(speechText == null || speechText == "") {            
            Debug.LogWarning("Tried to start a speech but the text is null or empty!");
            return false;
        }

        if(!pipeline.SetPipelineStatus(false)) {return false;}
        await Task.Delay((int) (startDelaySecond * 1000));      //1000ms => 1s

        IsTalking = true;
        print("Starting direct speech:" + speechText);
        await tts.TextToSpeech(speechText);
        pipeline.SetPipelineStatus(true);
        IsTalking = false;

        return true;
    }
}
