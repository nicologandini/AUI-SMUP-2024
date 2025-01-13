using Photon.Pun;
using UnityEngine;
using SMUP.GameLogic;
using System.Collections;
using System;

public class RequestMatchHandler : MonoBehaviour
{
    [SerializeField] private float countdownTimer = 10f;
    [SerializeField] private GameMultiplayer gameMultiplayer;

    private bool thisRequested = false;
    private bool otherRequested = false;
    private PhotonView photonView;
    private Player player;


    private void Start() {
        if (gameMultiplayer == null) {
            gameMultiplayer = FindFirstObjectByType<GameMultiplayer>();
        }
    }


    public void SetPhotonView(PhotonView p_photonView) {
        photonView = p_photonView;
    }
    public void SetPlayer(Player p_player) {
        player = p_player;
    }

    // First player press the button and requests match 
    public void RequestMatch() {
        if (thisRequested) {
            Debug.Log("Requested already!");
            return;
        }
        if (otherRequested) {
            Debug.Log("Other already requested => answering the match");
            thisRequested = true;
            AnswerRequest();
            return;
        }
        
        if (photonView == null) { return; }

        Console_UI.Instance.ConsolePrint("Requesting match");
        thisRequested = true;
        StartCoroutine(CountdownCoroutine());
        photonView.RPC("SetRequestFlag", RpcTarget.Others, true);  
        //photonView.RPC("performMatch", RpcTarget.Others, player.GetBalloonsColors());  
    }

    // Other player press the button and answer the match 
    public void AnswerRequest() {
        photonView.RPC("RequestAnswered", RpcTarget.Others);
    }

    // Both players pressed the button in time => starting ballons exchange
    [PunRPC]  
    public void RequestAnswered() {
        if (photonView == null) { return; }
        if (player == null) { return; }

        Console_UI.Instance.ConsolePrint("Exchanging match");
        photonView.RPC("performMatch", RpcTarget.Others, player.GetBalloonsColors()); 
    }

    [PunRPC]  
    private void performMatch(string colorNames){
        if (gameMultiplayer == null) {
            Console_UI.Instance.ConsolePrint("!gameMultiplayer is null => cannot perform Match!");
            TimeUp();
            return;
        }

        gameMultiplayer.performMatch(colorNames);
    }

    [PunRPC]  
    public void SetRequestFlag(bool value) {
        Console_UI.Instance.ConsolePrint($"set otherRequest bool to: {value}");
        otherRequested = value;
    }

    IEnumerator CountdownCoroutine() {
        yield return new WaitForSeconds(countdownTimer);

        TimeUp();
    }

    private void TimeUp()
    {   
        Console_UI.Instance.ConsolePrint("Timeup!");

        thisRequested = false;
        photonView.RPC("SetRequestFlag", RpcTarget.Others, false);  
    }
}