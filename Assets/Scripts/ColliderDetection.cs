using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ColliderDetection : MonoBehaviour
{
	private GameObject collidingObject = null;
    
    private void OnTriggerEnter(Collider c)
    {
        Debug.Log("I'm the trigger, someone has entered");
        if (c.tag == "SortingObject")  // If the tag of the object is equal to "SortingObject"
        {
            collidingObject = c.gameObject;
            Debug.Log("Collision on the delivery point by " + collidingObject.GetInstanceID());
            Debug.Log(this.transform.parent.gameObject);
            SingletonScript.Instance.stationOcc(this.transform.parent.gameObject);
        }
    }
    
    private void OnTriggerExit(Collider c)
    {
        Debug.Log("Exit event");
        if (c.tag == "SortingObject")
        {
			Debug.Log("Free");
			collidingObject = null;
            SingletonScript.Instance.stationAv(this.transform.parent.gameObject);
        }
    }
}

