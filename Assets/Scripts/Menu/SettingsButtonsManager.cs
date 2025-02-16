using System.Runtime.ExceptionServices;
using UnityEngine;

public class SettingsButtonsManager : MonoBehaviour
{
    [Header ("BUtton Managers")]
    [SerializeField] private ButtonsChangeStateManager buddy_ButtonsManager;
    [SerializeField] private ButtonsChangeStateManager ARVR_ButtonsManager;

    
    [Header("Values Transferer")]
    [SerializeField] private ValueSceneTranferer transferer;


    private bool _isBuddyActive = true;
    private bool _isAROnStart = false;


    void Start()
    {
        if(transferer == null) {
            transferer = FindFirstObjectByType<ValueSceneTranferer>();
        }

        if(transferer ==  null) {Debug.LogWarning("Could not find any ValueSceneTransferer!!");}
    }


    public void BuddyOnButtonPress() {
        if(_isBuddyActive) {return;}

        transferer.AddOrUpdateValue("isAssistantActive", true);
        _isBuddyActive = true;
        buddy_ButtonsManager.ButtonPressed(0);
    }
    public void BuddyOffButtonPress() {
        if(!_isBuddyActive) {return;}

        transferer.AddOrUpdateValue("isAssistantActive", false);
        _isBuddyActive = false;
        buddy_ButtonsManager.ButtonPressed(1);
    }

    public void StartWithVRButtonPress() {
        if(!_isAROnStart) {return;}

        transferer.AddOrUpdateValue("startInAR", false);
        _isAROnStart = false;
        ARVR_ButtonsManager.ButtonPressed(0);
    }
    public void StartWithARButtonPress() {
        if(_isAROnStart) {return;}

        transferer.AddOrUpdateValue("startInAR", true);
        _isAROnStart = true;
        ARVR_ButtonsManager.ButtonPressed(1);
    }
}
