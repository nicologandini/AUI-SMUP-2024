using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using static GameMultiplayer;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;       //Changed to multiplayer

public class ColliderDetection : MonoBehaviour
{
    [SerializeField] private HapticImpulsePlayer m_HapticImpulsePlayer;
    public float intensity = 0.5f; // Intensità della vibrazione (da 0 a 1)
    public float duration = 0.2f;  // Durata della vibrazione in secondi
    public float frequency = 0.0f;  // 0.0 per usare valore di default


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
				GameInstance.changeOtherStation(this.transform.parent.gameObject, 1);
                //SingletonScript.Instance.stationColour(GameInstance.getOtherStation(this.transform.parent.gameObject), "grey");
            }
            
            GameInstance.addBalloon(collidingObject, this.transform.parent.gameObject);
        }


        HapticImpulsePlayer interactor = c.GetComponent<HapticImpulsePlayer>();
        DebugDialogue.Instance.AppendInfoText("Interactor is null?:" + (interactor == null));

        if (interactor != null)
        {
            DebugDialogue.Instance.AppendInfoText("Interactor is " + interactor.gameObject.name);
            interactor.SendHapticImpulse(intensity, duration, frequency);
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

