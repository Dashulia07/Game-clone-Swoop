using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioSource mainSound;
    public AudioSource failSound;
    private bool isPlaneAlive;

    public static bool soundOn() {
        return PlayerPrefs.GetInt("Sound", 1) == 1;
    }

    private void tryPlay(AudioSource sound)
    {
        if (!sound.isPlaying)
        {
            sound.Play();
        }
    }

    public void setPlaneAlive(bool isAlive) { 
        isPlaneAlive = isAlive;
    }

    public void checkState() {
        if (soundOn())
        {
            if (isPlaneAlive)
            {
                tryPlay(mainSound);
                failSound.Stop();
            }
            else 
            {
                mainSound.Stop();
                tryPlay(failSound);
            }
        }
        else 
        {
            mainSound.Stop();
            failSound.Stop();
        }
    }

}
