using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;


public class KeyPressTest : MonoBehaviour
{
    [SerializeField] private InputActionManager inputManager;
    [SerializeField] private TextMeshProUGUI infoText;


    private InputAction _actionBinding;


    private void Start() {
        _actionBinding = inputManager.actionAssets[0].FindActionMap("Main").FindAction("X Constraint");          //actionMaps[0].actions[18];
        print($"actionBinding: {_actionBinding}");
        ShowInfoText($"actionBinding: {_actionBinding}");

        _actionBinding.performed += _ => {ShowInfoText($"<color=green>Pressed {_actionBinding}</color>");};
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
