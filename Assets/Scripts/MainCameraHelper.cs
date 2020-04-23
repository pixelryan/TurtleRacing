using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraHelper : MonoBehaviour
{

    public AudioClip soundTrack;
    public static bool shouldStartSoundtrack;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldStartSoundtrack)
        {
            //AudioSource.PlayClipAtPoint()
            GetComponent<AudioSource>().Play();
            shouldStartSoundtrack = false;
        }
    }

    public static void StartTheRaceMusic()
    {
        if (BonusRoundManager.IsMusicEnabled)
        {
            Debug.Log("Starting soundtrack on camera");
            //shouldStartSoundtrack = true;
        }
    }
}
