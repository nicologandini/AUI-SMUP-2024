using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DebugDialogue : MonoBehaviour
{
    private static DebugDialogue _instance;
    public static DebugDialogue Instance {
        get {
            if (_instance != null) {
                return _instance;
            } else {
                GameObject debugDialogue_GO = new GameObject("DebugDialogueAuto");
                _instance = debugDialogue_GO.AddComponent<DebugDialogue>();
                return _instance;
            }
        }
        private set {
            _instance = value;
        }
    }


    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private bool isDebugOn = false;


    private void Awake() {
        if(_instance != null) {
            if(_instance == this) {return;}

            Destroy(this.gameObject);
            return;
        } else {
            _instance = this;
        }
    }


    public void ShowInfoText(string text) {
        if(!isDebugOn) {return;}
        if(infoText == null) { return; }

        infoText.text = text;
    }

    public void AppendInfoText(string text) {
        if(!isDebugOn) {return;}
        if(infoText == null) { return; }

        infoText.text += "\n" + text;
    }

        public void AppendInLine(string text) {
        if(!isDebugOn) {return;}
        if(infoText == null) { return; }

        infoText.text += "|" + text;
    }
}
