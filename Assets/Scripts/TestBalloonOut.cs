using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TestBalloonOut : MonoBehaviour
{
    private void OnTriggerEnter(Collider c)
    {
        Debug.Log("I'm the trigger, someone has entered");
        if (c.tag == "SortingObject")  // If the tag of the object is equal to "SortingObject"
        {
			Debug.Log("Collision on the delivery point by " + c.gameObject.GetInstanceID());
            Transform tPos = c.gameObject.transform;
            Vector3 pos = new Vector3(tPos.position.x, tPos.position.y, tPos.position.z);
            Debug.Log(pos);

            //GameObject.Find("blue_balloon").GetComponent<TestBal>().enabled = true;
        }
    }
    
    private void OnTriggerExit(Collider c)
    {
        //GameObject.Find("blue_balloon").GetComponent("TestBal").enabled = false;
    }
}

