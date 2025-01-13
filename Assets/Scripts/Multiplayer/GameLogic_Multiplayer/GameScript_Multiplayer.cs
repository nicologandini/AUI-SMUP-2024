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
    [SerializeField] AutoMoveBalloons autoMoveBalloons;
    [SerializeField] RequestMatchHandler requestMatchHandler;


    List <GameObject> _stationsPlayer = null;  
    List <GameObject> _balloonsPlayer = null; 

    int balloons_counter = 0;

    public static GameMultiplayer GameInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameInstance = this;
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

            disabler.DisableObjects(stationsPlayer2);
            disabler.DisableObjects(balloonsPlayer2);
        } else {
            _stationsPlayer = stationsPlayer2;
            _balloonsPlayer = balloonsPlayer2;

            disabler.DisableObjects(stationsPlayer1);
            disabler.DisableObjects(balloonsPlayer1);
        }

        if (autoMoveBalloons.SortAtStart) {
            autoMoveBalloons.SetBalloons(_balloonsPlayer.ToArray<GameObject>());
            autoMoveBalloons.SetDeliverySpots(_stationsPlayer.ToArray<GameObject>());

            autoMoveBalloons.AutoSortBalloons();
        }
        
        if(_stationsPlayer.Count != _stationsPlayer.Count){
            Debug.Log("Wrong number of balloons or statios!");
        } else {
            balloons_counter = _balloonsPlayer.Count;
        }

        player = new Player(_balloonsPlayer, _stationsPlayer);
        Player p = player;

        requestMatchHandler.SetPhotonView(PhotonView.Get(this));
        requestMatchHandler.SetPlayer(player);

        //print platforms info
        for(int i=0; i<p.getStations().Count; i++) {
            Debug.Log("Stazione " + p.getStations()[i] + " " + p.getStations()[i].GetInstanceID() + " - " + p.getBalloons()[i] + " " + p.getBalloons()[i].GetInstanceID());
        }

        // // Pretend to press the button
        // if (PhotonNetwork.IsMasterClient) {
        //     FakeButtonPress();
        // } else {
        //     Invoke("FakeButtonPress", 11f);
        // }

        Console_UI.Instance.ClearLog();
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
        Console_UI.Instance.ClearLog();

        Player p0 = player;
        if (p0 == null) {Console_UI.Instance.ConsolePrint("There is no player!");}

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
                return false;
            }
        }

        return true;
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
}