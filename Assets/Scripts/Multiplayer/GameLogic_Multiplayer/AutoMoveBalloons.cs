using UnityEngine;

public class AutoMoveBalloons : MonoBehaviour
{
    private GameObject[] balloons;
    private GameObject[] deliverySpots;


    [SerializeField] private bool sortAtStart = false;
    public bool SortAtStart => sortAtStart;


    private void Start() {
        // if (sortAtStart) {
        //     AutoSortBalloons();
        // }
    }

    public void AutoSortBalloons() {
        if(balloons.Length != deliverySpots.Length) {
            Debug.Log("Different number of balloons and delivery spots!!");
            return;
        }

        for (int i = 0; i<balloons.Length; i++) {
            balloons[i].transform.position = deliverySpots[i].transform.position + Vector3.up*0.1f;
        }
    }

    public void SetBalloons (GameObject[] p_balloons){
        balloons = new GameObject[p_balloons.Length];

        for (int i = 0; i<balloons.Length; i++) {
            balloons[i] = p_balloons[i];
        }
    }

    public void SetDeliverySpots(GameObject[] p_spots) {
        deliverySpots = new GameObject[p_spots.Length]; 

        Console_UI.print($"spots lenght: {p_spots.Length}");
        foreach(GameObject obj in p_spots) {
            Console_UI.print($"spot: {obj}");
        }

        for (int i = 0; i<deliverySpots.Length; i++) {
            deliverySpots[i] = p_spots[i];
        }
    }
}
