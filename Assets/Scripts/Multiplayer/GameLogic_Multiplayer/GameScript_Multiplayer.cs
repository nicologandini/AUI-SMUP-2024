using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using SMUP.AI;
using SMUP.Audio;
using SMUP.GameLogic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class GameMultiplayer : MonoBehaviour
{
    Player player = null;

    [Header("Master player ref")]
    [SerializeField] List <GameObject> stationsPlayer1 = null;      // Inputs in Unity
    [SerializeField] List <GameObject> balloonsPlayer1 = null;      // Inputs in Unity
    [SerializeField] List <GameObject> buttonsPlayer1 = null;      // Inputs in Unity

    [Header("Guest player ref")]
    [SerializeField] List <GameObject> stationsPlayer2 = null;      // Inputs in Unity
    [SerializeField] List <GameObject> balloonsPlayer2 = null;      // Inputs in Unity
    [SerializeField] List <GameObject> buttonsPlayer2 = null;      // Inputs in Unity


    [Header("Sounds")]
    [SerializeField] AudioClipEnum rightMatchSFX;
    [SerializeField] AudioClipEnum wrongMatchSFX;


    [Header("Utils")]
    [SerializeField] DisableOtherPlayerObjects disabler;
    [SerializeField] AutoMoveBalloons autoMoveBalloons;
    [SerializeField] RequestMatchHandler requestMatchHandler;
    [SerializeField] private InputActionManager inputManager;

    [SerializeField] private TextTTS_SO startingText;


    List <GameObject> _stationsPlayer = null;  
    List <GameObject> _balloonsPlayer = null; 

    int balloons_counter = 0;

    public static GameMultiplayer GameInstance;

    GameObject passthrough;
    Camera mainCamera;
	Color originalTableColor;
	Color newTableColor;
    private InputAction actionBinding;
	
	bool isMaster;

    public bool StartInAR = false;

    void Awake()
    {
        GameInstance = this;

        mainCamera = Camera.main;
        Debug.Log(mainCamera);
        Debug.Log(mainCamera.clearFlags);

        GameObject camera = GameObject.Find("MR Interaction Setup");
        GameObject xrorigin = camera.transform.GetChild(3).gameObject;
        camera = xrorigin.transform.GetChild(0).gameObject;
        passthrough = camera.transform.GetChild(0).gameObject;
        passthrough.GetComponent<OVRPassthroughLayer>().enabled = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(StartInAR) {
            //ActivateAR();
        }

		// disabilitare da qui per attivare pasthru
        if (disabler == null) {
            disabler = FindFirstObjectByType<DisableOtherPlayerObjects>();
        } 
        
        if (autoMoveBalloons == null) {
            autoMoveBalloons = GetComponent<AutoMoveBalloons>();
        }
        if (requestMatchHandler == null) {
            requestMatchHandler = FindFirstObjectByType<RequestMatchHandler>();
        }
		
		
        if(PhotonNetwork.IsMasterClient) {
            _stationsPlayer = stationsPlayer1;
            _balloonsPlayer = balloonsPlayer1;

            //disabler.DisableObjects(stationsPlayer2);
            disabler.DisableObjects(balloonsPlayer2);
            disabler.DisableObjects(buttonsPlayer2);
			
			isMaster = true;
        } else {
            _stationsPlayer = stationsPlayer2;
            _balloonsPlayer = balloonsPlayer2;

            //disabler.DisableObjects(stationsPlayer1);
            disabler.DisableObjects(balloonsPlayer1);
            disabler.DisableObjects(buttonsPlayer1);
			
			isMaster = false;
        }


        foreach (var station in _stationsPlayer) {
            ColliderDetection collider = station.GetComponentInChildren<ColliderDetection>();
            if(collider == null) {
                Debug.LogWarning("No ColliderDetection found for" + station.name);
                continue;
            }
            collider.SetOwnedBalloons(_balloonsPlayer);
        }

        if (autoMoveBalloons != null && autoMoveBalloons.SortAtStart) {
            autoMoveBalloons.SetBalloons(_balloonsPlayer.ToArray<GameObject>());
            autoMoveBalloons.SetDeliverySpots(_stationsPlayer.ToArray<GameObject>());

            autoMoveBalloons.AutoSortBalloons();
        }
        
		
        /*
        if(_stationsPlayer.Count != _stationsPlayer.Count){
            Debug.Log("Wrong number of balloons or statios!");
        } else {
            balloons_counter = _balloonsPlayer.Count;
        } // disabilitare fino a qui per disabilitare psthrugh*/

		//////////////////////////// Da abilitare per Passthrough
		// _stationsPlayer = stationsPlayer1;
        // _balloonsPlayer = balloonsPlayer1;
	
		/////////////////////////////////// FINE PROVA 
		
		
        player = new Player(_balloonsPlayer, _stationsPlayer);
        Player p = player;

		/*
        requestMatchHandler.SetPhotonView(PhotonView.Get(this));
        requestMatchHandler.SetPlayer(player);
        */

        //print platforms info
        for(int i=0; i<p.getStations().Count; i++) {
            Debug.Log("Stazione " + p.getStations()[i] + " " + p.getStations()[i].GetInstanceID() + " - " + p.getBalloons()[i] + " " + p.getBalloons()[i].GetInstanceID());
        }



        //Console_UI.Instance.ClearLog();



        // // Pretend to press the button
        // if (PhotonNetwork.IsMasterClient) {
        //     FakeButtonPress();
        // } else {
        //     Invoke("FakeButtonPress", 11f);
        // }

        //Console_UI.Instance.ClearLog();
		//GameObject table = GameObject.Find("table");
		//originalTableColor = table.GetComponent<Renderer>().material.color;
		//newTableColor = new Color (originalTableColor.r, originalTableColor.g, originalTableColor.b, 1.0f);
		
		Debug.Log("player is " + player);

        actionBinding = inputManager.actionAssets[0].FindActionMap("Main").FindAction("Y Constraint"); 
        actionBinding.performed += _ => passthroughAction();

        DirectSpeechManager speechMaager = DirectSpeechManager.Instance;
        if (speechMaager != null) {
            speechMaager.StartSpeech(startingText, 0f);
        }
        
        print("End game start");
    }

    private void OnDisable() {
        if(actionBinding == null) {return;}
        actionBinding.performed -= _ => passthroughAction();
    }


    private void ActivateAR() {
        if(passthrough.GetComponent<OVRPassthroughLayer>().enabled == true) return;

        passthroughAction();
    }

    public void passthroughAction()
    {
		GameObject table = GameObject.Find("table");
		GameObject table2 = GameObject.Find("table_R");
		Color oldColor = table.GetComponent<Renderer>().material.color;
        if (passthrough.GetComponent<OVRPassthroughLayer>().enabled == true)
        {
			table.GetComponent<Renderer>().material.color = new Color(oldColor.r, oldColor.g, oldColor.b, 1.0f);
			table2.GetComponent<Renderer>().material.color = new Color(oldColor.r, oldColor.g, oldColor.b, 1.0f);
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
			table.GetComponent<Renderer>().material.color = new Color(oldColor.r, oldColor.g, oldColor.b, 0.4f);
			table2.GetComponent<Renderer>().material.color = new Color(oldColor.r, oldColor.g, oldColor.b, 0.4f);
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
        //ground.SetActive(value);
		//ground.GetComponent<Renderer>().enabled = value;

    }

    private void setWall0(bool value)
    {
        GameObject walls = GameObject.Find("Walls");
        for (int i = 0; i < 22; i++)
        {
            //walls.transform.GetChild(i).gameObject.SetActive(value);
            walls.transform.GetChild(i).gameObject.GetComponent<Renderer>().enabled = value;
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
        GameObject sky = GameObject.Find("SM_SimpleSky_Dome_01");

        // clouds.SetActive(value);
        // sky.SetActive(value);

        clouds.GetComponent<Renderer>().enabled = value;
        sky.GetComponent<Renderer>().enabled = value;
    }

    public void addBalloon(GameObject balloon, GameObject station){
        //foreach(Player p in players){
        Player p = getPlayer(balloon);
        print($"player: {p}");

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
				//otherStation = getOtherStation(station);
				SingletonScript.Instance.stationColour(station, "yellow");
				changeOtherStation(station, 0); // 0 = yellow
				//SingletonScript.Instance.stationColour(otherStation, "yellow");
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

        PhotonView photonView = PhotonView.Get(this);
        if (!getLists(colorsList))
        {
            photonView.RPC("Lost", RpcTarget.All);  
        } else {
            photonView.RPC("Win", RpcTarget.All);  
        }
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

		
		for (int i = 0; i < 6; i++) {
			if (p0.GetBalloonColorName(p0.getBalloon(p0.getStation(i))) == "NULL" || balloonsColors[i] == "NULL") {
				match = false;
				return match;
			}
		}
		
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
				//changeOtherStation(station, 1);
                photonView.RPC("wrongStation", RpcTarget.Others, i);

                match = false;
            } else
            {
                station = p0.getStation(i);
                SingletonScript.Instance.stationColour(getOtherStation(station), "green");
                SingletonScript.Instance.stationColour(station, "green");
				//changeOtherStation(station, 2);
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

    /// <summary>
    /// Called when the playeers get a correct match
    /// </summary>
    [PunRPC]
    private void Win() {
        AudioManager.Instance.PlayAudioEnum(rightMatchSFX);
        Console_UI.Instance.ClearLog();
        Console_UI.Instance.ConsolePrint("All matches!", 40);
    }

    /// <summary>
    /// Called when the players get a wrong match
    /// </summary>
    [PunRPC]
    private void Lost() {
        AudioManager.Instance.PlayAudioEnum(wrongMatchSFX);     
        Console_UI.Instance.ConsolePrint("Not a match!");
        print("Not a match!");
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
			if (isMaster) {
				return stationsPlayer2[index];
			} else {
				return stationsPlayer1[index];
			}
		}
	}

	[PunRPC]
	private void greyStation(int i, bool master) {
			if (master) { /// devo modificare tavolo 1 dell'altro player
				SingletonScript.Instance.stationColour(stationsPlayer1[i], "grey");
			} else { // azione partita dal non master, devo modificare tavolo 2 del master
				SingletonScript.Instance.stationColour(stationsPlayer2[i], "grey");
			}
	}
	
	private int getOtherStationIndex(GameObject station) {
		int index = 11;
        Player p0 = player;

		Debug.Log(station);
		Debug.Log(player);
        index = p0.getStationIndex(station);
		return index;
	}
	
	public void changeOtherStation(GameObject station, int color) { // color 0 = yellow, color 1 = grey, color 2 = green
		
		int i;
		
		PhotonView photonView = PhotonView.Get(this);
		
		i = getOtherStationIndex(station);
		if (i < 7) {
			if (color == 0) {
				photonView.RPC("yellowStation", RpcTarget.Others, i, isMaster);
			} else if (color == 1) {
				photonView.RPC("greyStation", RpcTarget.Others, i, isMaster);
			} else if (color == 2) {
				photonView.RPC("greenStation", RpcTarget.Others, i, isMaster);
			}
		}
	}
	
	[PunRPC]
	private void yellowStation(int i, bool master) {
		if (master) { /// devo modificare tavolo 1 dell'altro player
			SingletonScript.Instance.stationColour(stationsPlayer1[i], "yellow");
		} else { // azione partita dal non master, devo modificare tavolo 2 del master
			SingletonScript.Instance.stationColour(stationsPlayer2[i], "yellow");
		}
	}
	
	[PunRPC]
	private void greenStation(int i, bool master) {
		if (master) { /// devo modificare tavolo 1 dell'altro player
			SingletonScript.Instance.stationColour(stationsPlayer1[i], "green");
		} else { // azione partita dal non master, devo modificare tavolo 2 del master
			SingletonScript.Instance.stationColour(stationsPlayer2[i], "green");
		}
	}
}