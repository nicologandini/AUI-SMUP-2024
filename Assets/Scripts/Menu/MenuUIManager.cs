using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class MenuUIManager : MonoBehaviour {
    [Header ("Menu Elements")]
    [SerializeField] private Image menuBG;
    [SerializeField] private ButtonsChangeStateManager menuButtonsManager;

    [Header ("Main Menu")]
    [SerializeField] private Sprite mainMenuBG;
    [SerializeField] private GameObject mainMenuAssets;

    [Header ("Settings")]
    [SerializeField] private Sprite settingsMenuBG;
    [SerializeField] private GameObject settingsMenuAssets;
    [SerializeField] private VirtualKeyboard virtualKeyboard;



    private MenuState currState = MenuState.NONE;


    private void Start()
    {
        ChangeState(MenuState.Main);
    }


    public void ChangeState(MenuState nextState) {
        if(nextState == currState) {return;}

        DeactivateCurrMenu();
        switch (nextState) {
            case MenuState.Main:
                ActivateMainMenuUI();
                currState = MenuState.Main;
                return;

            case MenuState.Settings:
                ActivateMenuSettingsUI();
                currState = MenuState.Settings;
                return;

            default:
                return;
        }
    }


    private void DeactivateCurrMenu() {
        switch (currState) {
            case MenuState.Main:
                DeactivateMainMenuUI();
                return;

            case MenuState.Settings:
                DeactivateMenuSettingsUI();
                return;

            default:
                return;
        }
    }

    private void ActivateMainMenuUI() {
        menuBG.sprite = mainMenuBG;
        menuButtonsManager.ButtonPressed(0);
        mainMenuAssets.SetActive(true);
    }
    private void DeactivateMainMenuUI() {
        mainMenuAssets.SetActive(false);
    }

    private void ActivateMenuSettingsUI() {
        menuBG.sprite = settingsMenuBG;
        menuButtonsManager.ButtonPressed(1);
        settingsMenuAssets.SetActive(true);
    } 
    private void DeactivateMenuSettingsUI() {
        if(virtualKeyboard != null) {virtualKeyboard.SetCanvas(false);}

        settingsMenuAssets.SetActive(false);
    }


    // button state methods
    public void SetSettingNextState() {
        ChangeState(MenuState.Settings);
    }
    public void SetMainNextState() {
        ChangeState(MenuState.Main);
    }
}

public enum MenuState {
    NONE = -1, Main = 0, Settings
}