using UnityEngine;
using System.Collections.Generic; 

public class Game : MonoBehaviour
{
    [SerializeField]
    GameObject[] stations = null;  // Inputs in Unity

    private string plateAv = "Balloon_station_plate_av";
    private string plateOk = "Balloon_station_plate_ok";
    private string plateOcc = "Balloon_station_plate_occ";
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject station in stations)
        {
            Transform childAv = station.transform.Find(plateAv);
            Transform childOk = station.transform.Find(plateOk);
            Transform childOcc = station.transform.Find(plateOcc);
            Debug.Log("La posizione del plate e': " + childAv.position);
            childAv.gameObject.SetActive(true);
            childOk.gameObject.SetActive(false);
            childOcc.gameObject.SetActive(false);
        }
        
        //GameObject s1 = GameObject.Find("Balloon_station_1");
        //Transform childTransform = s1.transform.Find("Balloon_station_plate");
        //Debug.Log("La posizione del plate e': " + childTransform.position);
        //childTransform.gameObject.SetActive(false);
    }
}
