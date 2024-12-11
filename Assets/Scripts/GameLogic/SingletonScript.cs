using UnityEngine;
using System.Collections.Generic;
using static Game;

public class SingletonScript : MonoBehaviour
{
    private string plateAv = "Balloon_station_plate_av";
    private string plateOk = "Balloon_station_plate_ok";
    private string plateOcc = "Balloon_station_plate_occ";
    
    public static SingletonScript Instance;
    
    void Awake()
    {
        Instance = this;
    }
    
    public void stationAv(GameObject station)
    {
        Transform childAv = station.transform.Find(plateAv);
        Transform childOk = station.transform.Find(plateOk);
        Transform childOcc = station.transform.Find(plateOcc);
        childAv.gameObject.SetActive(true);
        childOk.gameObject.SetActive(false);
        childOcc.gameObject.SetActive(false);
    }
    
    public void stationOk(GameObject station)
    {
        Transform childAv = station.transform.Find(plateAv);
        Transform childOk = station.transform.Find(plateOk);
        Transform childOcc = station.transform.Find(plateOcc);
        childAv.gameObject.SetActive(false);
        childOk.gameObject.SetActive(true);
        childOcc.gameObject.SetActive(false);
    }
    
    public void stationOcc(GameObject station)
    {
        Transform childAv = station.transform.Find(plateAv);
        Transform childOk = station.transform.Find(plateOk);
        Transform childOcc = station.transform.Find(plateOcc);
        childAv.gameObject.SetActive(false);
        childOk.gameObject.SetActive(false);
        childOcc.gameObject.SetActive(true);
    }

}
