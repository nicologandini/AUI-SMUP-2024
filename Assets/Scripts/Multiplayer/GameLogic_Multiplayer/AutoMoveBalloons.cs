using UnityEngine;

public class AutoMoveBalloons : MonoBehaviour
{
    [SerializeField] private GameObject[] balloons;
    [SerializeField] private GameObject[] deliverySpot;


    [SerializeField] private bool sortAtStart = false;


    private void Start() {
        if (sortAtStart) {
            AutoSortBalloons();
        }
    }

    public void AutoSortBalloons() {
        if(balloons.Length != deliverySpot.Length) {
            Debug.Log("Different number of balloons and delivery spots!!");
            return;
        }

        for (int i = 0; i<balloons.Length; i++) {
            balloons[i].transform.position = deliverySpot[i].transform.position + Vector3.up*0.1f;
        }
    }
}
