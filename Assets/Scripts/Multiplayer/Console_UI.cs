using TMPro;
using UnityEngine;

public class Console_UI : MonoBehaviour
{
    private static Console_UI _instance;
    public static Console_UI Instance {
        get {
            if (_instance != null) {
                return _instance;
            } else {
                GameObject console_GO = new GameObject("ConsoleUIAuto");
                _instance = console_GO.AddComponent<Console_UI>();
                return _instance;
            }
        }
        private set {
            _instance = value;
        }
    }

    [SerializeField] private TextMeshProUGUI consoleText;
    [SerializeField] private bool isDebugOn = false;


    private void Awake() {
        if(_instance != null) {
            if(_instance == this) {DontDestroyOnLoad(this.gameObject); return;}

            Destroy(this.gameObject);
            return;
        } else {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start() {
        if(consoleText == null) {return;}

        consoleText.text = "";
        if(!isDebugOn) {consoleText.enabled = false;}
    }


    public void ClearLog()  {
        if(!isDebugOn) {return;}
        if(consoleText == null) {return;}
        consoleText.text = "";
    }

    public void ConsolePrint(string text, int textSize = 20) {
        Debug.Log(text);

        if(!isDebugOn) {return;}
        if(consoleText == null) {return;}
        consoleText.fontSize = textSize;
        consoleText.text += (text+"\n");
    }
}
