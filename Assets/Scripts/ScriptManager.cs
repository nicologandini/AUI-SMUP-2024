using UnityEngine;
using System.Collections.Generic; 

public class ScriptManager : MonoBehaviour
{
    public static ScriptManager instance;
    //private Script2 s2;

    void Awake() {
        
    }

    public void sendBalloonPosition(Dictionary<int, int> dict) {
        
    }

    private void SingletonPattern() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }
}
