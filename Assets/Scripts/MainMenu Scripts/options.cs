using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class options : MonoBehaviour
{
    public Slider slider;

    private void Start()
    {
        // Initialize the slider's position based on the current volume setting
        // This ensures the slider visually reflects the current volume when the options menu is opened
        slider.value = AudioListener.volume;
    }

    public void ChangeVolume()
    {
        // Update the game's volume based on the slider's position
        AudioListener.volume = slider.value;
        SaveSliderValue(); 
    }

    private void SaveSliderValue()
    {
        // Save the current slider value to PlayerPrefs
        PlayerPrefs.SetFloat("volume", slider.value);
    }
}
