using UnityEngine;
using System.Collections.Generic; 

public class Game : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject s1 = GameObject.Find("Balloon_station_1");
        Transform childTransform = s1.transform.Find("Balloon_station_plate");
        childTransform.gameObject.SetActive(false);
    }
}
