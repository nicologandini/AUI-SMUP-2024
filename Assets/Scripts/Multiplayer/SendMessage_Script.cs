using System;
using Photon.Pun;
using UnityEngine;

public class SendMessage_Script : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Console_UI.Instance.ClearLog();
        //if (!PhotonNetwork.IsMasterClient) { Destroy(this); }

        Invoke("RequestBalloons", 1f);
    }

  
    private void SendMessage() {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("ChatMessage", RpcTarget.All, "jup", "and jup.");        
    }

    [PunRPC]                //fa diventare target del messaaggio online
    void ChatMessage(string a, string b, PhotonMessageInfo info)
    {
        // the photonView.RPC() call is the same as without the info parameter.
        // the info.Sender is the player who called the RPC.
        Console_UI.Instance.ConsolePrint(string.Format("ChatMessage {0} {1}", a, b));
        Console_UI.Instance.ConsolePrint(string.Format("Info: {0} | {1} | {2}", info.Sender, info.photonView, info.SentServerTime));

        if (PhotonNetwork.IsMasterClient) {
            Console_UI.Instance.ConsolePrint("Sono il master client");
        } else {
            Console_UI.Instance.ConsolePrint("NON sono il master client");
        }
    }


    void RequestBalloons() {
        if (!PhotonNetwork.IsMasterClient) { return; }

        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("SendBalloons", RpcTarget.All);  
    }

    [PunRPC]
    void SendBalloons() {
        if (PhotonNetwork.IsMasterClient) { return; }

        string[] balloons = new string []{"b1", "b2", "b3"};

        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("ReceiveBalloons", RpcTarget.All, balloons);  
    }

    [PunRPC]
    void ReceiveBalloons(string[] balloonsName) {
        if (!PhotonNetwork.IsMasterClient) { return; }

        string balloons = "";

        foreach(var name in balloonsName) {
            balloons += name + " ";
        }

        Console_UI.Instance.ConsolePrint("Balloons: " + balloons);
    }
}
