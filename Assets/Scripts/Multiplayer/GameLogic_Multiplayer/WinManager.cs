using Photon.Pun;
using UnityEngine;

public class WinManager : MonoBehaviour
{
    [SerializeField] private GameObject winScreen;
    [SerializeField] private Animation winAnimation;
    [SerializeField] private bool useGeneralPos = true;


    [SerializeField] private Transform generalWinScreenPos;
    [SerializeField] private Transform masterWinScreenPos;
    [SerializeField] private Transform guestWinScreenPos;


    void Start()
    {
        winScreen.SetActive(false);

        //Invoke("Win",2f);
    }


    public void Win() {
        if(useGeneralPos) {
            SetWinScreenActive(generalWinScreenPos);
            return;
        }

        if(PhotonNetwork.IsMasterClient)
        {
            SetWinScreenActive(masterWinScreenPos);
        }
        else {
            SetWinScreenActive(guestWinScreenPos);
        }
    }


    private void SetWinScreenActive(Transform target)
    {
        winScreen.transform.SetPositionAndRotation(target.position, target.rotation);
        winScreen.SetActive(true);
        PlayWinAnim();
    }

    private void PlayWinAnim() {
        if(winAnimation == null) {winAnimation = winScreen.GetComponent<Animation>();}

        if(winAnimation == null) {return;}

        winAnimation.Stop();
        winAnimation.Play();
    }
}
