using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class sliderManager : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI value;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        value.text = slider.value.ToString();
    }

    public void SaveSliderValue()
    {
        PlayerPrefs.SetInt("SliderValue", (int)slider.value);
        PlayerPrefs.Save();
    }
}
