using Photon.Pun;
using UnityEngine;

public class CheckMessage : MonoBehaviour
{
    public void CheckBalloons(string[] materialsName) {
        if (!PhotonNetwork.IsMasterClient) {return; }

        for (int i = 0; i<materialsName.Length; i++) {
            
        }
    }
}
