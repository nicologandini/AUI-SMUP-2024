using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using SMUP.GameLogic;
using UnityEngine;

public class GameMultiplayer : MonoBehaviour
{
    Player player = null;

    [Header("Master player ref")]
    [SerializeField] List <GameObject> stationsPlayer1 = null;      // Inputs in Unity
    [SerializeField] List <GameObject> balloonsPlayer1 = null;      // Inputs in Unity
    [Header("Guest player ref")]
    [SerializeField] List <GameObject> stationsPlayer2 = null;      // Inputs in Unity
    [SerializeField] List <GameObject> balloonsPlayer2 = null;      // Inputs in Unity

    [Header("Utils")]
    [SerializeField] DisableOtherPlayerObjects disabler;
    //[SerializeField] AutoMoveBalloons autoMoveBalloons;
    [SerializeField] RequestMatchHandler requestMatchHandler;


    List <GameObject> _stationsPlayer = null;  
    List <GameObject> _balloonsPlayer = null; 

    int balloons_counter = 0;

    public static GameMultiplayer GameInstance;

    GameObject passthrough;
    Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameInstance = this;
        if (disabler == null) {
            disabler = FindFirstObjectByType<DisableOtherPlayerObjects>();
        }
        /*
        if (autoMoveBalloons == null) {
            autoMoveBalloons = GetComponent<AutoMoveBalloons>();
        }*/
        if (requestMatchHandler == null) {
            requestMatchHandler = FindFirstObjectByType<RequestMatchHandler>();
        }

        if(PhotonNetwork.IsMasterClient) {
            _stationsPlayer = stationsPlayer1;
            _balloonsPlayer = balloonsPlayer1;

            //disabler.DisableObjects(stationsPlayer2);
            disabler.DisableObjects(balloonsPlayer2);
        } else {
            _stationsPlayer = stationsPlayer2;
            _balloonsPlayer = balloonsPlayer2;

            //disabler.DisableObjects(stationsPlayer1);
            disabler.DisableObjects(balloonsPlayer1);
        }

        /*
        if (autoMoveBalloons.SortAtStart) {
            //autoMoveBalloons.SetBalloons(_balloonsPlayer.ToArray<GameObject>());
            //autoMoveBalloons.SetDeliverySpots(_stationsPlayer.ToArray<GameObject>());

            //autoMoveBalloons.AutoSortBalloons();
        }*/
        
        if(_stationsPlayer.Count != _stationsPlayer.Count){
            Debug.Log("Wrong number of balloons or statios!");
        } else {
            balloons_counter = _balloonsPlayer.Count;
        }

        //ToDo: metteere a posto stationsPlayer2
        player = new Player(_balloonsPlayer, _stationsPlayer, stationsPlayer2);
        Player p = player;

        requestMatchHandler.SetPhotonView(PhotonView.Get(this));
        requestMatchHandler.SetPlayer(player);

        //print platforms info
        for(int i=0; i<p.getStations().Count; i++) {
            Debug.Log("Stazione " + p.getStations()[i] + " " + p.getStations()[i].GetInstanceID() + " - " + p.getBalloons()[i] + " " + p.getBalloons()[i].GetInstanceID());
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

        // // Pretend to press the button
        // if (PhotonNetwork.IsMasterClient) {
        //     FakeButtonPress();
        // } else {
        //     Invoke("FakeButtonPress", 11f);
        // }

        Console_UI.Instance.ClearLog();
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

    private void setTrees(bool value)
    {
        GameObject env = GameObject.Find("Environment");
        GameObject dec = env.transform.GetChild(0).gameObject;
        GameObject vr = dec.transform.GetChild(2).gameObject;
        GameObject trees = vr.transform.GetChild(0).gameObject;
        for (int i = 0; i < 24; i++)
        {
            trees.transform.GetChild(i).gameObject.SetActive(value);
        }

    }

    private void setRocks(bool value)
    {
        GameObject rocks = GameObject.Find("Rocks");
        rocks.transform.GetChild(0).gameObject.SetActive(value);
        rocks.transform.GetChild(1).gameObject.SetActive(value);
    }

    private void setTerrain(bool value)
    {
        GameObject terrain = GameObject.Find("Terrain");
        for (int i = 0; i < 14; i++)
        {
            terrain.transform.GetChild(i).gameObject.SetActive(value);
        }

    }

    private void setGround(bool value)
    {
        GameObject ground = GameObject.Find("Ground");
        ground.SetActive(value);

    }

    private void setWall0(bool value)
    {
        GameObject walls = GameObject.Find("Walls");
        for (int i = 0; i < 22; i++)
        {
            walls.transform.GetChild(i).gameObject.SetActive(value);
        }
    }

    private void setGrass(bool value)
    {
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

    private void setClouds(bool value)
    {
        GameObject clouds = GameObject.Find("SM_Generic_CloudRing_01");
        clouds.SetActive(value);
        GameObject sky = GameObject.Find("SM_SimpleSky_Dome_01");
        sky.SetActive(value);
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
		GameObject otherStation;
        Player p = getPlayer(balloon);
            if(p.getStations().Contains(station) && p.getBalloons().Contains(balloon) && p.removeBalloon(balloon, station))
            {
				otherStation = getOtherStation(station);
				SingletonScript.Instance.stationColour(station, "yellow");
				SingletonScript.Instance.stationColour(otherStation, "yellow");
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


    public void RequestMatch() {
        Console_UI.Instance.ConsolePrint("Requesting mathc");

        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("performMatch", RpcTarget.Others, player.GetBalloonsColors());        
    }   
	
	[PunRPC]
    public void performMatch(string colorNames){
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
        if (p0 == null) {Console_UI.Instance.ConsolePrint("There is no player!"); match = false;}

        for (int i = 0; i < 6; i++)
        {
            GameObject station;
            /*
            Console_UI.Instance.ConsolePrint($"Is balloon {i} null: {p0.getStation(i) == null}");
            Console_UI.Instance.ConsolePrint($"Have different name {i}: {p0.GetBalloonColorName(p0.getBalloon(p0.getStation(i))) != balloonsColors[i]}");
            Console_UI.Instance.ConsolePrint($"Is player balloon NULL {i}: { p0.GetBalloonColorName(p0.getBalloon(p0.getStation(i))) == "NULL"}");
            Console_UI.Instance.ConsolePrint($"Is passed balloon NULL {i}: {balloonsColors[i] == "NULL"}");
            */

            if (p0.getBalloon(p0.getStation(i)) == null || p0.GetBalloonColorName(p0.getBalloon(p0.getStation(i))) != balloonsColors[i] || p0.GetBalloonColorName(p0.getBalloon(p0.getStation(i))) == "NULL" || balloonsColors[i] == "NULL")
            {
                station = p0.getStation(i);
                Console_UI.Instance.ConsolePrint("error on station: " + p0.getStation(i));
                Console_UI.Instance.ConsolePrint(p0.GetBalloonColorName(p0.getBalloon(p0.getStation(i))));
                Console_UI.Instance.ConsolePrint(balloonsColors[i]);
                
				SingletonScript.Instance.stationColour(getOtherStation(station), "grey");
                SingletonScript.Instance.stationColour(station, "grey");
                photonView.RPC("wrongStation", RpcTarget.Others, i);

                match = false;
            } else
            {
                station = p0.getStation(i);
                SingletonScript.Instance.stationColour(getOtherStation(station), "green");
                SingletonScript.Instance.stationColour(station, "green");
                photonView.RPC("correctStation", RpcTarget.Others, i);
            }
        }

        return match;
    }

    [PunRPC]
    private void correctStation(int i)
    {
        Player p0 = player;
        GameObject station = p0.getStation(i);

        SingletonScript.Instance.stationColour(getOtherStation(station), "green");
        SingletonScript.Instance.stationColour(station, "green");
    }

    [PunRPC]
    private void wrongStation(int i)
    {
        Player p0 = player;
        GameObject station = p0.getStation(i);

        SingletonScript.Instance.stationColour(getOtherStation(station), "grey");
        SingletonScript.Instance.stationColour(station, "grey");
    }
	
    private List<string> GetStrings(string allColors) {
        return (allColors.Split(",", StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
    }

    [PunRPC]
    private void Win() {
        Console_UI.Instance.ClearLog();
        Console_UI.Instance.ConsolePrint("All matches!", 40);
    }

    private void FakeButtonPress() {
        requestMatchHandler.RequestMatch();
    }
	
	public GameObject getOtherStation(GameObject station) {
		int index = 11;
        Player p0 = player;

        for (int i = 0; i < 6; i++) {
			if (p0.getStations()[i] == station) {
			index = i;
			}
		}
	
		if (index > 6) {
			return null;
		} else {
			if (PhotonNetwork.IsMasterClient) {
				return stationsPlayer2[index];
			} else {
				return stationsPlayer1[index];
			}
		}
	
	}

}