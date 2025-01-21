using System.Collections.Generic;
using Meta.WitAi.Utilities;
using Photon.Pun;
using UnityEngine;

public class DisableOtherPlayerObjects : MonoBehaviour
{
    //Dont put teh balloons or stations here, they are disbked in GameScript_Multilplayer
    [SerializeField] GameObject[] masterObjects;
    [SerializeField] GameObject[] guestObjects;



    private void Start() {
		/*
        if(PhotonNetwork.IsMasterClient) {
            foreach(GameObject obj in guestObjects) {
                obj.SetActive(false);
            }
        } else {
            foreach(GameObject obj in masterObjects) {
                obj.SetActive(false);
            }
        }*/
    }

    public void DisableObjects(List<GameObject> objects) {
        foreach(GameObject obj in objects) {
            obj.SetActive(false);
        }
    }
}
