using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using static Game;

public class ColliderDetection : MonoBehaviour
{
	private GameObject collidingObject = null;
    private GameObject cObj = null;
    private string regex = @"[1]"; // il nome del palloncino deve contenere almeno un '1'
    
    private void OnTriggerEnter(Collider c)
    {
        Debug.Log("I'm the trigger, someone has entered");
        if (c.tag == "SortingObject")  // If the tag of the object is equal to "SortingObject"
        {
            collidingObject = c.gameObject;
            Debug.Log("Collision on the delivery point by " + collidingObject.GetInstanceID());
            Debug.Log(this.transform.parent.gameObject);
            SingletonScript.Instance.stationOcc(this.transform.parent.gameObject);

            // RegEx per sapere a che lato appartiene il palloncino
            Regex r = new Regex(regex, RegexOptions.IgnoreCase);
            Match m = r.Match(collidingObject.name);
            
            // entra nell'if se il palloncino � sul tavolo _R
            if (m.Success)
            {
                Debug.Log("match");
                GameInstance.addBalloon(true, collidingObject.GetComponent<Renderer>().material, this.transform.parent.gameObject);

            } else
            {
                Debug.Log("match");
                GameInstance.addBalloon(false, collidingObject.GetComponent<Renderer>().material, this.transform.parent.gameObject);
            }

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

            cObj = c.gameObject;

            // RegEx per sapere a che lato appartiene il palloncino
            Regex r = new Regex(regex, RegexOptions.IgnoreCase);
            Match m = r.Match(cObj.name);

            // entra nell'if se il palloncino e' sul tavolo _R
            if (m.Success)
            {
                GameInstance.removeBalloon(true, cObj.GetComponent<Renderer>().material, this.transform.parent.gameObject);

            }
            else
            {
                GameInstance.removeBalloon(false, cObj.GetComponent<Renderer>().material, this.transform.parent.gameObject);
            }
        }
    }
}

