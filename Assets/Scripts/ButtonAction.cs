using UnityEngine;
using static GameMultiplayer;

public class ButtonAction : MonoBehaviour
{
    public void OnButtonAction()
    {
        print("Ho premuto il pulsante");
        
        /* OK code
        GameInstance.RequestMatch();            //GameInstance.performMatch();
        */
    }

    public void PassthroughAction()
    {
        /*GameObject camera = GameObject.Find("MR Interaction Setup");
        GameObject xrorigin = camera.transform.GetChild(3).gameObject;
        camera = xrorigin.transform.GetChild(0).gameObject;
        GameObject passthrough = camera.transform.GetChild(0).gameObject;
        passthrough.GetComponent<OVRPassthroughLayer>().enabled = false;
        */

        /* Ok code
        GameInstance.passthroughAction();
        */
    }
}
