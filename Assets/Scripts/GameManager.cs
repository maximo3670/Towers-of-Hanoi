using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        // Ensure this GameObject persists between scenes
        DontDestroyOnLoad(gameObject);
        
        // Initialize game settings
        InitializeSettings();
    }

    private void InitializeSettings()
    {
        if (!PlayerPrefs.HasKey("volume"))
        {
            PlayerPrefs.SetFloat("volume", 1.0f);
        }

        // Apply audio volume setting
        AudioListener.volume = PlayerPrefs.GetFloat("volume");
    }
}
