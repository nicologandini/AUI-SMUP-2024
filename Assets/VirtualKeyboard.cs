using TMPro;
using UnityEngine;

public class VirtualKeyboard : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;  // Riferimento al campo di testo
    [SerializeField] private Canvas canvas;  // Riferimento al campo di testo


    public void AddCharacter(string character)
    {
        if (inputField != null)
        {
            if(inputField.text.Length >= 20) {return;}
            inputField.text += character;
        }
    }

    public void DeleteCharacter()
    {
        if (inputField != null && inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        }
    }

    public void SetCanvas(bool value) {
        canvas.enabled = value;
    }
}
