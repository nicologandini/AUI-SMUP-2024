using UnityEngine;
using static Game;

public class ButtonAction : MonoBehaviour
{
    public void OnButtonAction()
    {
        print("Ho premuto il pulsante");
        GameInstance.performMatch();
    }
}
