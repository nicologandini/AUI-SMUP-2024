using SMUP.Audio;
using UnityEngine;

public class TestingSounds : MonoBehaviour
{
    [SerializeField] private float timeBetweenSounds = 3f;  
    [SerializeField] private AudioClipEnum soundToPlay;  


    private float timePassed = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.Instance.PlayAudioEnum(soundToPlay);
    }

    // Update is called once per frame
    void Update()
    {
        if (timePassed > timeBetweenSounds) {
            AudioManager.Instance.PlayAudioEnum(soundToPlay);
            timePassed = 0;
        }

        timePassed += Time.deltaTime;
    }
}
