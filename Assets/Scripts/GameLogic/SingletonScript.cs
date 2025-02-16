using UnityEngine;
using System.Collections.Generic;
using static Game;

public class SingletonScript : MonoBehaviour
{
    // private string plateAv = "Balloon_station_plate_av";
    // private string plateOk = "Balloon_station_plate_ok";
    // private string plateOcc = "Balloon_station_plate_occ";
    private string plate = "Balloon_station_plate";

    public static SingletonScript Instance;

    void Awake()
    {
        Instance = this;
    }

    public void stationColour(GameObject station, string mat)
    {
        Transform child = station.transform.Find(plate);
        child.gameObject.GetComponent<Renderer>().material = Resources.Load(mat, typeof(Material)) as Material;
    }

    public string getStationColor(GameObject station)
    {

        Transform child = station.transform.Find(plate);
        return child.gameObject.GetComponent<Renderer>().material.name;
    }

    // public void stationAv(GameObject station)
    // {
    //     Transform childAv = station.transform.Find(plateAv);
    //     Transform childOk = station.transform.Find(plateOk);
    //     Transform childOcc = station.transform.Find(plateOcc);
    //     childAv.gameObject.SetActive(true);
    //     childOk.gameObject.SetActive(false);
    //     childOcc.gameObject.SetActive(false);
    // }
    //
    // public void stationOk(GameObject station)
    // {
    //     Transform childAv = station.transform.Find(plateAv);
    //     Transform childOk = station.transform.Find(plateOk);
    //     Transform childOcc = station.transform.Find(plateOcc);
    //     childAv.gameObject.SetActive(false);
    //     childOk.gameObject.SetActive(true);
    //     childOcc.gameObject.SetActive(false);
    // }
    //
    // public void stationOcc(GameObject station)
    // {
    //     Transform childAv = station.transform.Find(plateAv);
    //     Transform childOk = station.transform.Find(plateOk);
    //     Transform childOcc = station.transform.Find(plateOcc);
    //     childAv.gameObject.SetActive(false);
    //     childOk.gameObject.SetActive(false);
    //     childOcc.gameObject.SetActive(true);
    // }

}
