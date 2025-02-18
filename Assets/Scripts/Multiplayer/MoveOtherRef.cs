using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MoveOtherRef : MonoBehaviour {
    [SerializeField] private Transform refTransform; 
    [SerializeField] private PhotonView photonView; 
    [SerializeField] private PlayerRole playerRole;
    [SerializeField] private GameObject visuals;



    private void Start() {
        if (playerRole == PlayerRole.Guest) {
            if(photonView == null) {photonView = PhotonView.Get(this);}

            if(photonView != null) {
                photonView.RequestOwnership();
            }
        }

        if (playerRole == PlayerRole.Master && PhotonNetwork.IsMasterClient) {
            if(visuals != null) {visuals.SetActive(false);}
        } else if (playerRole == PlayerRole.Guest && !PhotonNetwork.IsMasterClient) {
            if(visuals != null) {visuals.SetActive(false);}
        } else {
            if(visuals != null) {visuals.SetActive(true);}
        }
    }


    private void Update() {
        if(playerRole == PlayerRole.Master) {
            if(PhotonNetwork.IsMasterClient) {
                transform.SetPositionAndRotation(refTransform.position, refTransform.rotation);
            }
        } else {
            if(!PhotonNetwork.IsMasterClient) {
                transform.SetPositionAndRotation(refTransform.position, refTransform.rotation);
            }
        }
    }

    private enum PlayerRole {
        Master, Guest
    }
}