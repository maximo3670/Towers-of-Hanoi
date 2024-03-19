using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class options : MonoBehaviour
{
    public Slider slider;

    // Start is called before the first frame update
    void Awake()
    {
        if(!PlayerPrefs.HasKey("volume")){
            PlayerPrefs.SetFloat("volume", 1);
            load();
        } else {
            load();
        }
    }

    public void ChangeVolume(){
        AudioListener.volume = slider.value;
        SaveSliderValue();
    }

    public void load(){
        slider.value = PlayerPrefs.GetFloat("volume");
    }

    public void SaveSliderValue()
    {
        PlayerPrefs.SetFloat("volume", slider.value);
    }
}
