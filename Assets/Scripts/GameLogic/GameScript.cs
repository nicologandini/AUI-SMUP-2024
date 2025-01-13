using UnityEngine;
using System.Collections.Generic;
using SMUP.GameLogic;

public class Game : MonoBehaviour
{
    List<Player> players = null;

    [SerializeField]
    List <GameObject> stationsPlayer1 = null;  // Inputs in Unity

    [SerializeField]
    List <GameObject> balloonsPlayer1 = null; // Inputs in Unity

    [SerializeField]
    List <GameObject> stationsPlayer2 = null; // Inputs in Unity

    [SerializeField]
    List <GameObject> balloonsPlayer2 = null; // Inputs in Unity

    int balloons_counter = 0;

    public static Game GameInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameInstance = this;
        
        if((balloonsPlayer1.Count != balloonsPlayer2.Count) || (stationsPlayer1.Count != stationsPlayer1.Count)){
            Debug.Log("Wrong number of balloons or statios!");
        } else {
            balloons_counter = balloonsPlayer1.Count;
        }
        // deliveredBalloons_L = new List<int> (new int[balloons_counter]);
        // deliveredBalloons_R = new List<int> (new int[balloons_counter]);

        players = new List<Player>();
        players.Add(new Player(balloonsPlayer1, stationsPlayer1));
        players.Add(new Player(balloonsPlayer2, stationsPlayer2));

        foreach(Player p in players){
            for(int i=0; i<p.getStations().Count; i++) {
                Debug.Log("Stazione " + p.getStations()[i] + " " + p.getStations()[i].GetInstanceID() + " - " + p.getBalloons()[i] + " " + p.getBalloons()[i].GetInstanceID());
            }
        }

        // for (int i = 0; i < balloons_counter; i++)
        // {
        //     deliveredBalloons_L[i] = 11; // 11 = libero
        //     deliveredBalloons_R[i] = 11; //
        // }

		// // Solo per test, poi si può eliminare
        // foreach (GameObject station in stations_L)
        // {
        //     Transform childAv = station.transform.Find(plateAv);
        //     Transform childOk = station.transform.Find(plateOk);
        //     Transform childOcc = station.transform.Find(plateOcc);
        //     Debug.Log("La posizione del plate e': " + childAv.position);
        //     childAv.gameObject.SetActive(true);
        //     childOk.gameObject.SetActive(false);
        //     childOcc.gameObject.SetActive(false);
        //     // Debug.Log(childAv); Output is name 
        // }
        
        //GameObject s1 = GameObject.Find("Balloon_station_1");
        //Transform childTransform = s1.transform.Find("Balloon_station_plate");
        //Debug.Log("La posizione del plate e': " + childTransform.position);
        //childTransform.gameObject.SetActive(false);
    }

    public void addBalloon(GameObject balloon, GameObject station){
        //foreach(Player p in players){
        Player p = getPlayer(balloon);
            Debug.Log("Sto cercando il palloncino " + balloon + balloon.GetInstanceID() + " e la stazione " + station + station.GetInstanceID() + " nel player ");
            Debug.Log(p.getStations().Contains(station));
            Debug.Log(p.getBalloons().Contains(balloon));
            Debug.Log(p.containsBalloon(station));
            if(p.getStations().Contains(station) && p.getBalloons().Contains(balloon) && !p.containsBalloon(station)){
                Debug.Log("Entering addBalloon if");
                p.addDeliveredBalloon(balloon, station);
                Debug.Log("Balloon " + p.getBalloon(station) + " added on the station " + station);
                return;
            }
        //}
        Debug.Log("Balloon positioned on the wrong station");
        Debug.Log("Situation on " + station + ": " + p.getBalloon(station));
    }

    public void removeBalloon(GameObject balloon, GameObject station){
        //foreach(Player p in players){
        Player p = getPlayer(balloon);
            if(p.getStations().Contains(station) && p.getBalloons().Contains(balloon) && p.removeBalloon(balloon, station))
            {
                Debug.Log("entering removeBalloon if");
                p.removeDeliveredBalloon(station);
                Debug.Log("Balloon " + balloon + " removed from the station " + station);
                Debug.Log("Situation on " + station + ": " + p.getBalloon(station));
                return;
            }
        //}
        Debug.Log("Error during removing balloon" + balloon + " from the station " + station);
        Debug.Log("Situation on " + station + ": " + p.getBalloon(station));
    }

    public void performMatch(){
        List<List<GameObject>> matrix = new List<List<GameObject>>();
        if(players is null){
            Debug.Log("Players do not exist");
            return;
        }

        if (!getLists())
        {
            Debug.Log("not a match");
            return;
        }

        /*
        foreach(Player p in players){
            if(p.getDeliveredBalloons() is null){
                Debug.Log("Not all the stations are occupied by a balloon! Match not possible");
                return;
            }
            matrix.Add(p.getDeliveredBalloons());
        }

        for(int i=0; i<matrix.Count-1; i++){  // Iterate thru columns
            for(int j=0; j<matrix[i].Count; j++){  // Iterate thru rows
                Material mat1 = matrix[j][i].GetComponent<Renderer>().material;
                Material mat2 = matrix[j][i+1].GetComponent<Renderer>().material;
                Debug.Log(mat1 + " - " + mat2);
                if(mat1 != mat2){  // Material of the balloons must be the same.
                    Debug.Log("Not a match!");
                    return;
                }
            }
        }*/
        Debug.Log("Match!");
    }

    public Player getPlayer(GameObject balloon)
    {
        foreach(Player p in players)
        {
            if (p.getBalloons().Contains(balloon))
            {
                return p;
            }
        }
        return null;
    }

    private bool getLists()
    {
        Player p0 = players[0];
        Player p1 = players[1];
        
        for (int i = 0; i < 6; i++)
        {
            if (p0.getBalloon(p0.getStation(i)).GetComponent<Renderer>().material.name != p1.getBalloon(p1.getStation(i)).GetComponent<Renderer>().material.name || p0.getBalloon(p0.getStation(i)) == null)
            {
                Debug.Log("error on station: " + p0.getStation(i));
                Debug.Log(p0.getBalloon(p0.getStation(i)).GetComponent<Renderer>().material);
                Debug.Log(p1.getBalloon(p1.getStation(i)).GetComponent<Renderer>().material);
                return false;
            }
        }

        return true;
    }







    // public void addBalloon_old(bool position, Material mat, GameObject station) // true = right, false = left
    // {
    //     Debug.Log(mat.name);
    //     int stationIndex = returnStationIndex(station);
    //     int color = returnColor(mat);

        
    //     if (position) {
    //         if (deliveredBalloons_R[stationIndex] == 11) // se la stazione non e' gia' occupata entra nell'if
    //         {
    //             //Debug.Log(mat.name);
    //             deliveredBalloons_R[stationIndex] = color;
    //             Debug.Log("aggiunto palloncino di colore " + mat.name + " in posizione " + (stationIndex + 1));
    //         }
    //     } else {
    //         if (deliveredBalloons_L[stationIndex] == 11) // se la stazione non e' gia' occupata entra nell'if
    //         {
    //             //Debug.Log(deliveredBalloons_L.Length);
    //             deliveredBalloons_L[stationIndex] = color;//mat.name;
    //             Debug.Log("aggiunto palloncino di colore " + mat.name + " in posizione " + (stationIndex + 1));
    //         }
    //     }
        
    // }

    // public void removeBalloon_old(bool position, Material mat, GameObject station)
    // {
    //     int stationIndex = returnStationIndex(station);
    //     int color = returnColor(mat);

    //     if (position) {
    //         deliveredBalloons_R[color] = 11; // ora libero
    //         Debug.Log("rimosso palloncino di colore " + mat.name + " in posizione " + (stationIndex + 1));
    //     }
    //     else {
    //         deliveredBalloons_L[color] = 11; // ora libero
    //         Debug.Log("rimosso palloncino di colore " + mat.name + " in posizione " + (stationIndex + 1));
    //         Debug.Log(deliveredBalloons_L[color]);
    //     }

    // }

    // public void performMatch_old()
    // {
    //     // Itera per tutti gli elementi degli array deliveredBalloons, che in ogni posizione (stazione) contengono gli indici relativi ai colori
    //     for (int i = 0; i < 6; i++)
    //     {
    //         if (deliveredBalloons_L[i] != deliveredBalloons_R[i] || deliveredBalloons_L[i] == 11 || deliveredBalloons_R[i] == 11) // valori devono essere uguali e nessuna delle due stazioni vuota
    //         {
    //             Debug.Log("not a match");

    //             // Da implementare il ritorno dei palloncini ai posti di partenza

    //             return;
    //         }
    //     }

    //     Debug.Log("match!");
    //     return;
    // }

    // private int returnStationIndex(GameObject station)
    // {
    //     if (station.name == "Balloon_station_1" || station.name == "Balloon_station_1_R")
    //     {
    //         return 0;
    //     } else if (station.name == "Balloon_station_2" || station.name == "Balloon_station_2_R")
    //     {
    //         return 1;
    //     }
    //     else if (station.name == "Balloon_station_3" || station.name == "Balloon_station_3_R")
    //     {
    //         return 2;
    //     }
    //     else if (station.name == "Balloon_station_4" || station.name == "Balloon_station_4_R")
    //     {
    //         return 3;
    //     }
    //     else if (station.name == "Balloon_station_5" || station.name == "Balloon_station_5_R")
    //     {
    //         return 4;
    //     }
    //     else if (station.name == "Balloon_station_6" || station.name == "Balloon_station_6_R")
    //     {
    //         return 5;
    //     } else
    //     {
    //         return 0;
    //     }

    // }

    // private int returnColor(Material mat)
    // {
    //     switch(mat.name)
    //     {
    //         case "blue":
    //             return (int)colors.blue;
    //         case "green":
    //             return (int)colors.green;
    //         case "lightblue":
    //             return (int)colors.lightblue;
    //         case "orange":
    //             return (int)colors.orange;
    //         case "pink":
    //             return (int)colors.pink;
    //         case "purple (Instance)":
    //             return (int)colors.purple;
    //         default:
    //             Debug.Log("error: no color");
    //             return 40; // costante simbolica

    //     }

    // }
}
