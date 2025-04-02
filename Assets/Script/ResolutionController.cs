using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionController : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    private string resolutionString;

    void Start()
    {
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        
    }

    public void SetResolution()
    {
        int idx = resolutionDropdown.value;

        string resolution = resolutionDropdown.options[idx].text;

        int width = int.Parse(resolution.Split(" x ")[0]);
        int height = int.Parse(resolution.Split(" x ")[1]);

        resolutionString = resolution;
        Debug.Log(resolution);
        
        Screen.SetResolution(width, height, Screen.fullScreen);
    }

    public void SetFullscreen(bool val)
    {
        Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void Init()
    {
        if (PlayerPrefs.GetInt("fullScreen") == 1)
            fullscreenToggle.isOn = true;

        for(int i = 0; i < resolutionDropdown.options.Count; i++)
        {
            if (resolutionDropdown.options[i].text.Equals(resolutionString))
                resolutionDropdown.value = i;
        }
    }

    public void Save()
    {
        if (fullscreenToggle.isOn)
            PlayerPrefs.SetInt("fullScreen", 1);
        else
            PlayerPrefs.SetInt("fullScreen", 0);

        PlayerPrefs.SetString("resolution", resolutionString);
    }
}
