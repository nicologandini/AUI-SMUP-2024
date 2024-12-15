using UnityEngine;
using static Game;

public class ButtonAction : MonoBehaviour
{
    public void OnButtonAction()
    {
        print("Ho premuto il pulsante");
        if (GameInstance.canMatch())
        {
            GameInstance.performMatch();
        } else
        {
            Debug.Log("There are still empty stations!");
        }
    }

}
