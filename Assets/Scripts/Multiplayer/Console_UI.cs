using TMPro;
using UnityEngine;

public class Console_UI : MonoBehaviour
{
    public static Console_UI Instance;

    [SerializeField] private TextMeshProUGUI consoleText;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(this);
        }
    }

    private void Start() {
        consoleText.text = "";
    }


    public void ClearLog()  {
        consoleText.text = "";
    }

    public void ConsolePrint(string text, int textSize = 20) {
        Debug.Log(text);

        consoleText.fontSize = textSize;
        consoleText.text += (text+"\n");
    }
}
