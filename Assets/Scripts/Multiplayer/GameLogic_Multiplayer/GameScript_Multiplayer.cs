using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using SMUP.GameLogic;
using UnityEngine;

public class GameMultiplayer : MonoBehaviour
{
    Player player = null;

    [SerializeField] List <GameObject> stationsPlayer = null;  // Inputs in Unity
    [SerializeField] List <GameObject> balloonsPlayer = null; // Inputs in Unity
	[SerializeField] List <GameObject> otherStationsPlayer = null; // Inputs in Unity

    int balloons_counter = 0;

    public static GameMultiplayer GameInstance;

    GameObject passthrough;
	Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameInstance = this;
        
        if(stationsPlayer.Count != stationsPlayer.Count){
            Debug.Log("Wrong number of balloons or statios!");
        } else {
            balloons_counter = balloonsPlayer.Count;
        }

        player = new Player(balloonsPlayer, stationsPlayer, otherStationsPlayer);
        Player p = player;

        for(int i=0; i<p.getStations().Count; i++) {
            Debug.Log("Stazione " + p.getStations()[i] + " " + p.getStations()[i].GetInstanceID() + " - " + p.getBalloons()[i] + " " + p.getBalloons()[i].GetInstanceID());
        }

        // Pretend to press the button
        if (!PhotonNetwork.IsMasterClient) {
            //Invoke("RequestMatch", 5f);
        }

		mainCamera = Camera.main;
		Debug.Log(mainCamera);
		Debug.Log(mainCamera.clearFlags);
        //Console_UI.Instance.ClearLog();

        GameObject camera = GameObject.Find("MR Interaction Setup");
        GameObject xrorigin = camera.transform.GetChild(3).gameObject;
        camera = xrorigin.transform.GetChild(0).gameObject;
        passthrough = camera.transform.GetChild(0).gameObject;
        passthrough.GetComponent<OVRPassthroughLayer>().enabled = false;
		
    }

    public void passthroughAction()
    {
        if (passthrough.GetComponent<OVRPassthroughLayer>().enabled == true)
        {
			mainCamera.clearFlags = CameraClearFlags.Skybox;
            passthrough.GetComponent<OVRPassthroughLayer>().enabled = false;
            setTrees(true);
			setRocks(true);
			setWall0(true);
			setTerrain(true);
			setGround(true);
			setGrass(true);
			setClouds(true);

        }
        else
        {
			mainCamera.clearFlags = CameraClearFlags.SolidColor;
            passthrough.GetComponent<OVRPassthroughLayer>().enabled = true;
			setTrees(false);
			setRocks(false);
			setWall0(false);
			setTerrain(false);
			setGround(false);
			setGrass(false);
			setClouds(false);
			//mainCamera.clearFlags = CameraClearFlags.SolidColor;
			
        }
    }
	
	private void setTrees(bool value) {
		GameObject env = GameObject.Find("Environment");
        GameObject dec = env.transform.GetChild(0).gameObject;
        GameObject vr = dec.transform.GetChild(2).gameObject;
        GameObject trees = vr.transform.GetChild(0).gameObject;
		for (int i = 0; i < 24; i++)
        {
            trees.transform.GetChild(i).gameObject.SetActive(value);
        }
	
	}
	
	private void setRocks(bool value) {
		GameObject rocks = GameObject.Find("Rocks");
		rocks.transform.GetChild(0).gameObject.SetActive(value);
		rocks.transform.GetChild(1).gameObject.SetActive(value);
	}
	
	private void setTerrain(bool value) {
		GameObject terrain = GameObject.Find("Terrain");
		for (int i = 0; i < 14; i++)
        {
            terrain.transform.GetChild(i).gameObject.SetActive(value);
        }
		
	}
	
	private void setGround(bool value) {
		GameObject ground = GameObject.Find("Ground");
		ground.SetActive(value);
	
	}
	
	private void setWall0(bool value) {
		GameObject walls = GameObject.Find("Walls");
		for (int i = 0; i < 22; i++)
        {
            walls.transform.GetChild(i).gameObject.SetActive(value);
        }
	}
	
	private void setGrass(bool value) {
		GameObject grass = GameObject.Find("Grass");
		grass.transform.GetChild(0).gameObject.SetActive(value);
		grass.transform.GetChild(1).gameObject.SetActive(value);
		grass.transform.GetChild(2).gameObject.SetActive(value);
		grass.transform.GetChild(3).gameObject.SetActive(value);
		grass.transform.GetChild(4).gameObject.SetActive(value);
		grass.transform.GetChild(5).gameObject.SetActive(value);
		grass.transform.GetChild(6).gameObject.SetActive(value);
		grass.transform.GetChild(7).gameObject.SetActive(value);
		grass.transform.GetChild(8).gameObject.SetActive(value);
		grass.transform.GetChild(9).gameObject.SetActive(value);
		grass.transform.GetChild(10).gameObject.SetActive(value);
		grass.transform.GetChild(11).gameObject.SetActive(value);
		grass.transform.GetChild(12).gameObject.SetActive(value);
		grass.transform.GetChild(13).gameObject.SetActive(value);
		
		/*
		for (int i = 0; i < 13; i++)
        {
            grass.transform.GetChild(i).gameObject.SetActive(value);
        }*/
	}
	
	private void setClouds(bool value) {
		GameObject clouds = GameObject.Find("SM_Generic_CloudRing_01");
		clouds.SetActive(value);
		GameObject sky = GameObject.Find("SM_SimpleSky_Dome_01");
		sky.SetActive(value);
	}
	
	
    public void addBalloon(GameObject balloon, GameObject station){
        
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
        
        Debug.Log("Balloon positioned on the wrong station");
        Debug.Log("Situation on " + station + ": " + p.getBalloon(station));
    }

    public void removeBalloon(GameObject balloon, GameObject station){
    
        Player p = getPlayer(balloon);

            if(p.getStations().Contains(station) && p.getBalloons().Contains(balloon) && p.removeBalloon(balloon, station))
            {
				//int index = p.getStationIndex(station);
				//SingletonScript.Instance.stationColour(p.getOtherStation(index), "yellow");
				changeOtherStation(station, true);
                SingletonScript.Instance.stationColour(station, "yellow");
                Debug.Log("entering removeBalloon if");
                p.removeDeliveredBalloon(station);
                Debug.Log("Balloon " + balloon + " removed from the station " + station);
                Debug.Log("Situation on " + station + ": " + p.getBalloon(station));
                return;
            }
        
        Debug.Log("Error during removing balloon" + balloon + " from the station " + station);
        Debug.Log("Situation on " + station + ": " + p.getBalloon(station));
    }

    public void RequestMatch() {
        Console_UI.Instance.ConsolePrint("Requesting mathc");

        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("performMatch", RpcTarget.Others, player.GetBalloonsColors());        
    }   

    [PunRPC]  
    private void performMatch(string colorNames){
        Console_UI.Instance.ConsolePrint("Colors Name: " + colorNames);
        List<string> colorsList = GetStrings(colorNames);

        Console_UI.Instance.ConsolePrint("Color List Length: " + colorsList.Count);
        foreach (var name in colorsList) {
            Console_UI.Instance.ConsolePrint("Color List: " + name);
        }
        
        List<List<GameObject>> matrix = new List<List<GameObject>>();
        if(player is null){
            Debug.Log("Players do not exist");
            return;
        }

        if (!getLists(colorsList))
        {
            Console_UI.Instance.ConsolePrint("Not a match!");
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

        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("Win", RpcTarget.All);  
    }

    public Player getPlayer(GameObject balloon)
    {
        if (player.getBalloons().Contains(balloon))
        {
            return player;
        }
        return null;
    }

    private bool getLists(List<string> balloonsColors)
    {
        //Console_UI.Instance.ClearLog();

        PhotonView photonView = PhotonView.Get(this);
        bool match = true;

        Player p0 = player;
        if (p0 == null) {Console_UI.Instance.ConsolePrint("There is no player!"); match = false; }

        for (int i = 0; i < 6; i++)
        {
            /*
            Console_UI.Instance.ConsolePrint($"Is balloon {i} null: {p0.getStation(i) == null}");
            Console_UI.Instance.ConsolePrint($"Have different name {i}: {p0.GetBalloonColorName(p0.getBalloon(p0.getStation(i))) != balloonsColors[i]}");
            Console_UI.Instance.ConsolePrint($"Is player balloon NULL {i}: { p0.GetBalloonColorName(p0.getBalloon(p0.getStation(i))) == "NULL"}");
            Console_UI.Instance.ConsolePrint($"Is passed balloon NULL {i}: {balloonsColors[i] == "NULL"}");
            */

            if (p0.getBalloon(p0.getStation(i)) == null || p0.GetBalloonColorName(p0.getBalloon(p0.getStation(i))) != balloonsColors[i] || p0.GetBalloonColorName(p0.getBalloon(p0.getStation(i))) == "NULL" || balloonsColors[i] == "NULL")
            {
                Console_UI.Instance.ConsolePrint("error on station: " + p0.getStation(i));
                Console_UI.Instance.ConsolePrint(p0.GetBalloonColorName(p0.getBalloon(p0.getStation(i))));
                Console_UI.Instance.ConsolePrint(balloonsColors[i]);
				SingletonScript.Instance.stationColour(p0.getOtherStation(i), "grey");
                SingletonScript.Instance.stationColour(p0.getStation(i), "grey");
                photonView.RPC("wrongStation", RpcTarget.Others, i);
                match = false;
            } else
            {
				SingletonScript.Instance.stationColour(p0.getOtherStation(i), "green");
                SingletonScript.Instance.stationColour(p0.getStation(i), "green");
                photonView.RPC("correctStation", RpcTarget.Others, i);
            }
        }

        return match;
    }

    private List<string> GetStrings(string allColors) {
        return (allColors.Split(",", StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
    }

    [PunRPC]
    private void Win() {
        //Console_UI.Instance.ClearLog();
        Console_UI.Instance.ConsolePrint("All matches!", 40);
    }

    [PunRPC]
    private void correctStation(int i)
    {
        Player p0 = player;
		SingletonScript.Instance.stationColour(p0.getOtherStation(i), "green");
        SingletonScript.Instance.stationColour(p0.getStation(i), "green");
    }

    [PunRPC]
    private void wrongStation(int i)
    {
        Player p0 = player;
		SingletonScript.Instance.stationColour(p0.getOtherStation(i), "grey");
        SingletonScript.Instance.stationColour(p0.getStation(i), "grey");
    }
	
	[PunRPC]
	private void changeStationColorGrey(int i)
	{
		Player p0 = player;
		SingletonScript.Instance.stationColour(p0.getOtherStation(i), "grey");
	}
	
	[PunRPC]
	private void changeStationColorYellow(int i)
	{
		Player p0 = player;
		SingletonScript.Instance.stationColour(p0.getOtherStation(i), "yellow");
	}

	public void changeOtherStation(GameObject station, bool free) {
		
		PhotonView photonView = PhotonView.Get(this);
		Player p0 = player;
		int index = p0.getStationIndex(station);
		
		if (free == false) {
			photonView.RPC("changeStationColorGrey", RpcTarget.Others, index); // non libero
		} else {
			photonView.RPC("changeStationColorYellow", RpcTarget.Others, index); // libero
		}
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
