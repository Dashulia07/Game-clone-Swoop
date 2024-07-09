using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour
{
    public Button playSoundButton;
    public Sprite noSoundImage;
    public Sprite soundImage;
    public SoundController soundController;

    private void Start()
    {
        if (SoundController.soundOn())
        {
            playSoundButton.image.sprite = soundImage;
        }
        else 
        {
            playSoundButton.image.sprite = noSoundImage;
        }
    }
    public void click()
    {
        if (SoundController.soundOn())
        {
            PlayerPrefs.SetInt("Sound", 0);
            playSoundButton.image.sprite = noSoundImage;
        }
        else
        {
            PlayerPrefs.SetInt("Sound", 1);
            playSoundButton.image.sprite = soundImage;
        }
    }
}
