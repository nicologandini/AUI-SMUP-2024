using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("I'm the player, I enter to a trigger");
    }
}
