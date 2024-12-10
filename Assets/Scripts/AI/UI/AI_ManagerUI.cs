using SMUP_AI;
using TMPro;
using UnityEngine;
using Utilities.Extensions;

public class AI_ManagerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI talkingText;
    [SerializeField] private AI_Pipeline aiPipeline;



    private void Start() {
        aiPipeline.OnTalkingChanged += ToggleTalkingText; 
    }

    private void OnDestroy() {
        aiPipeline.OnTalkingChanged -= ToggleTalkingText; 
    }


    private void ToggleTalkingText(bool toggle) {
        if (toggle) {
            ShowTalkingText();
        } else {
            HideTalkingText();
        }
    }

    private void ShowTalkingText() {
        if (talkingText == null) { return; }

        talkingText.gameObject.SetActive(true);
        talkingText.text = "Talking...";
    }

    private void HideTalkingText() {
        if (talkingText == null) { return; }

        talkingText.gameObject.SetActive(false);
        talkingText.text = "";
    }
}
