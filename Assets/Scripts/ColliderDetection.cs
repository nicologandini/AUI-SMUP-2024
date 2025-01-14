using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using static GameMultiplayer;       //Changed to multiplayer

public class ColliderDetection : MonoBehaviour
{
	private GameObject collidingObject = null;
    
    private void OnTriggerEnter(Collider c)
    {
        Debug.Log("I'm the trigger, someone has entered");
        if (c.tag == "SortingObject")  // If the tag of the object is equal to "SortingObject"
        {
            collidingObject = c.gameObject;
            Debug.Log("Collision on " + this.transform.parent.gameObject + " by " + collidingObject);  //.getInstanceID()
            // Change the plate colour
            if (SingletonScript.Instance.getStationColor(this.transform.parent.gameObject) == "yellow (Instance)")
            {
                SingletonScript.Instance.stationColour(this.transform.parent.gameObject, "grey");
				GameInstance.changeOtherStation(this.transform.parent.gameObject, false);
            }
            
            GameInstance.addBalloon(collidingObject, this.transform.parent.gameObject);
        }
    }
    
    private void OnTriggerExit(Collider c)
    {
        Debug.Log("Exit event from the trigger");
        if (c.tag == "SortingObject")
        {
            collidingObject = c.gameObject;
            Debug.Log("Free Young Thug");

            GameInstance.removeBalloon(collidingObject, this.transform.parent.gameObject);
            collidingObject = null;
        }
    }
}

