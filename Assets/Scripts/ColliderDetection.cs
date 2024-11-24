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
			Debug.Log("Collision on the delivery point by " + c.gameObject.GetInstanceID());
            collidingObject = c.gameObject;
        }
    }
    
    private void OnTriggerExit(Collider c)
    {
        Debug.Log("Exit event");
        if (c.tag == "SortingObject")
        {
			Debug.Log("Free");
			collidingObject = null;
        }
    }
}

