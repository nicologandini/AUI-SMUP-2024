using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Game : MonoBehaviour
{
    [SerializeField]
    GameObject[] stations = null;  // Inputs in Unity

    [SerializeField]
    GameObject[] balloons_L = null;

    [SerializeField]
    GameObject[] stations_R = null; // Inputs in Unity

    [SerializeField]
    GameObject[] balloons_R = null;

    private int[] deliveredBalloons_L = new int[6];

    private int[] deliveredBalloons_R = new int[6];

    private string plateAv = "Balloon_station_plate_av";
    private string plateOk = "Balloon_station_plate_ok";
    private string plateOcc = "Balloon_station_plate_occ";
    
    public enum colors { blue, green, lightblue, orange, pink, purple };

    public static Game GameInstance;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameInstance = this;
        Debug.Log(deliveredBalloons_L.Length);
        for (int i = 0; i < 6; i++)
        {
            deliveredBalloons_L[i] = 11; // 11 = libero
            deliveredBalloons_R[i] = 11; //
        }

        // per rendere i due bottoni "vecchi" invisibili
        GameObject button_old = GameObject.Find("PushButton_old");
        GameObject button_old1 = GameObject.Find("PushButton (1)_old");
        button_old.SetActive(false);
        button_old1.SetActive(false);

        foreach (GameObject station in stations)
        {
            Transform childAv = station.transform.Find(plateAv);
            Transform childOk = station.transform.Find(plateOk);
            Transform childOcc = station.transform.Find(plateOcc);
            Debug.Log("La posizione del plate e': " + childAv.position);
            childAv.gameObject.SetActive(true);
            childOk.gameObject.SetActive(false);
            childOcc.gameObject.SetActive(false);
            // Debug.Log(childAv); Output is name 
        }
        
        //GameObject s1 = GameObject.Find("Balloon_station_1");
        //Transform childTransform = s1.transform.Find("Balloon_station_plate");
        //Debug.Log("La posizione del plate e': " + childTransform.position);
        //childTransform.gameObject.SetActive(false);
    }

    public void addBalloon(bool position, Material mat, GameObject station) // true = right, false = left
    {
        Debug.Log(mat.name);
        int stationIndex = returnStationIndex(station);
        int color = returnColor(mat);

        
        if (position) {
            if (deliveredBalloons_R[stationIndex] == 11) // se la stazione non e' gia' occupata entra nell'if
            {
                //Debug.Log(mat.name);
                deliveredBalloons_R[stationIndex] = color;
                Debug.Log("aggiunto palloncino di colore " + mat.name + " in posizione " + (stationIndex + 1));
            }
        } else {
            if (deliveredBalloons_L[stationIndex] == 11) // se la stazione non e' gia' occupata entra nell'if
            {
                //Debug.Log(deliveredBalloons_L.Length);
                deliveredBalloons_L[stationIndex] = color;//mat.name;
                Debug.Log("aggiunto palloncino di colore " + mat.name + " in posizione " + (stationIndex + 1));
            }
        }
        
    }

    public void removeBalloon(bool position, Material mat, GameObject station)
    {
        int stationIndex = returnStationIndex(station);
        int color = returnColor(mat);

        if (position) {
            deliveredBalloons_R[color] = 11; // ora libero
            Debug.Log("rimosso palloncino di colore " + mat.name + " in posizione " + (stationIndex + 1));
        }
        else {
            deliveredBalloons_L[color] = 11; // ora libero
            Debug.Log("rimosso palloncino di colore " + mat.name + " in posizione " + (stationIndex + 1));
            Debug.Log(deliveredBalloons_L[color]);
        }

    }

    public void performMatch()
    {
        // Itera per tutti gli elementi degli array deliveredBalloons, che in ogni posizione (stazione) contengono gli indici relativi ai colori
        for (int i = 0; i < 6; i++)
        {
            if (deliveredBalloons_L[i] != deliveredBalloons_R[i] || deliveredBalloons_L[i] == 11 || deliveredBalloons_R[i] == 11) // valori devono essere uguali e nessuna delle due stazioni vuota
            {
                Debug.Log("not a match");

                // Da implementare il ritorno dei palloncini ai posti di partenza

                return;
            }
        }

        Debug.Log("match!");
        return;
    }

    private int returnStationIndex(GameObject station)
    {
        if (station.name == "Balloon_station_1" || station.name == "Balloon_station_1_R")
        {
            return 0;
        } else if (station.name == "Balloon_station_2" || station.name == "Balloon_station_2_R")
        {
            return 1;
        }
        else if (station.name == "Balloon_station_3" || station.name == "Balloon_station_3_R")
        {
            return 2;
        }
        else if (station.name == "Balloon_station_4" || station.name == "Balloon_station_4_R")
        {
            return 3;
        }
        else if (station.name == "Balloon_station_5" || station.name == "Balloon_station_5_R")
        {
            return 4;
        }
        else if (station.name == "Balloon_station_6" || station.name == "Balloon_station_6_R")
        {
            return 5;
        } else
        {
            return 0;
        }

    }

    private int returnColor(Material mat)
    {
        switch(mat.name)
        {
            case "blue":
                return (int)colors.blue;
            case "green":
                return (int)colors.green;
            case "lightblue":
                return (int)colors.lightblue;
            case "orange":
                return (int)colors.orange;
            case "pink":
                return (int)colors.pink;
            case "purple (Instance)":
                return (int)colors.purple;
            default:
                Debug.Log("error: no color");
                return 40; // costante simbolica

        }

    }
}
