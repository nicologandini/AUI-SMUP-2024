using UnityEngine;
using System.Collections.Generic; 

public class Game : MonoBehaviour
{
    //private Transform childObj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // childObj = transform.find("Balloon_station_plate");
        // childObj.active = false;
        GameObject s0 = GameObject.Find("Balloon_station_0");
        // GameObject myResults = s0.GetComponentInChildren<GameObject>();
        // Debug.Log(myResults);
        s0.SetActive(false);
    }

    // // Update is called once per frame
    // void Update()
    // {
    //     
    // }
}
