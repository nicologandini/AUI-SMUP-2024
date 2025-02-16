using UnityEngine;
using System.Collections.Generic;
using SMUP.GameLogic;
using SMUP.AI;

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
	
	[SerializeField]
    List <GameObject> otherStations = null; // Inputs in Unity


    [SerializeField] private TextTTS_SO startingText;


    int balloons_counter = 0;

    Player p0, p1;

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
        players.Add(new Player(balloonsPlayer1, stationsPlayer1/*, otherStations*/));
        players.Add(new Player(balloonsPlayer2, stationsPlayer2/*, otherStations*/));

        p0 = players[0];
        p1 = players[1];

        foreach (Player p in players){
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

        DirectSpeechManager.Instance.StartSpeech(startingText);
        print("End game start");
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
                SingletonScript.Instance.stationColour(station, "yellow");
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

        bool match = true;

        for (int i = 0; i < 6; i++)
        {
            if (p0.getBalloon(p0.getStation(i)).GetComponent<Renderer>().material.name != p1.getBalloon(p1.getStation(i)).GetComponent<Renderer>().material.name || p0.getBalloon(p0.getStation(i)) == null)
            {
                Debug.Log("error on station: " + p0.getStation(i));
                Debug.Log(p0.getBalloon(p0.getStation(i)).GetComponent<Renderer>().material);
                Debug.Log(p1.getBalloon(p1.getStation(i)).GetComponent<Renderer>().material);
                match = false;
            } else
            {
                SingletonScript.Instance.stationColour(p0.getStation(i), "green");
                SingletonScript.Instance.stationColour(p1.getStation(i), "green");

            }
        }

        return match;
    }

    public bool canMatch()
    {

        for (int i = 0; i < 6; i++)
        {
            if (p0.getBalloon(p0.getStation(i)) == null || p1.getBalloon(p1.getStation(i)) == null)
            {
                return false;
            }
        }

        return true;

    }

}
