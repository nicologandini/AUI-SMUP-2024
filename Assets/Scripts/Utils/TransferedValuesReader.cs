using UnityEngine;
using static GameMultiplayer;

public class TransferedValuesReader : MonoBehaviour
{
    [SerializeField] private GameObject[] assistantGameObjects;   
    //[SerializeField] private ;

    void Awake()
    {
        ValueSceneTranferer transferer = ValueSceneTranferer.Instance;

        SetAssistantValues(transferer);
    }

    void Start()
    {
        ValueSceneTranferer transferer = ValueSceneTranferer.Instance;

        SetARValues(transferer);    //In Start se non non puo prendere riferimento a GameInstance
    }   

    private static void SetARValues(ValueSceneTranferer transferer)
    {
        System.Object ARvalue = transferer.GetValue("startInAR");
        if (ARvalue == null)
        {
            Debug.LogWarning("key used resulted in null value from the transferer dictionary");
            return;
        }

        bool startInAR = (bool)ARvalue;
        if (startInAR)
        {
            if(GameInstance == null) {Debug.LogWarning("GameInstance is null"); return;}
            GameInstance.StartInAR = true;
            GameInstance.passthroughAction();
        }
    }

    private void SetAssistantValues(ValueSceneTranferer transferer)
    {
        System.Object assistantValue = transferer.GetValue("isAssistantActive");
        if (assistantValue == null)
        {
            Debug.LogWarning("key used resulted in null value from the transferer dictionary");
            return;
        }

        bool isAssistantActive = (bool)assistantValue;
        foreach (var elem in assistantGameObjects)
        {
            elem.SetActive(isAssistantActive);
        }
    }
}
