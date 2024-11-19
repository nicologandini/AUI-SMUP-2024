using UnityEngine;
using System.Collection.Generic;
using System.Collections

public class ColliderDetection : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject enemy;
    public GameObject signObject;
    private bool activated = false;
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("im the trigger, someone has entered");
        if (other.tag == "Balloon")
        {
            signObject.SetActive(true);
            if (!activated)
            {
                activated = true;
                audioSource.enabled = true;
                enemy.SetActive(true);
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit event");
        if (other.tag == "Balloon")
        {
            signObject.SetActive(false);
        }
    }
}

