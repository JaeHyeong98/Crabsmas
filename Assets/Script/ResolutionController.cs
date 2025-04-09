using Main;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionController : MonoBehaviour
{
    public MyDropDown resolutionDropdown;
    public Toggle fullscreenToggle;

    private string resolutionString;

    void Start()
    {
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    public void SetResolution()
    {
        GSC.audioController.PlaySound2D("Click");
        int idx = resolutionDropdown.value;

        string resolution = resolutionDropdown.options[idx].text;
        resolutionString = resolution;

        int width = int.Parse(resolution.Split(" x ")[0]);
        int height = int.Parse(resolution.Split(" x ")[1]);

        Screen.SetResolution(width, height, Screen.fullScreen);
    }

    public void SetFullscreen(bool val)
    {
        GSC.audioController.PlaySound2D("Click");
        Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void Init()
    {
        if (PlayerPrefs.GetInt("fullScreen") == 1)
            fullscreenToggle.isOn = true;
        else
            fullscreenToggle.isOn = false;

        string resolutionStringVal = PlayerPrefs.GetString("resolution");
        resolutionString = resolutionStringVal;

        for (int i = 0; i < resolutionDropdown.options.Count; i++)
        {
            if (resolutionDropdown.options[i].text.Equals(resolutionStringVal))
                resolutionDropdown.value = i;
        }
    }

    public void Save()
    {
        if (fullscreenToggle.isOn)
            PlayerPrefs.SetInt("fullScreen", 1);
        else
            PlayerPrefs.SetInt("fullScreen", 0);

        //Debug.Log(resolutionString);
        PlayerPrefs.SetString("resolution", resolutionString);
    }

    public void Close()
    {
        string resolutionStringVal = PlayerPrefs.GetString("resolution");
        Debug.Log(resolutionStringVal);
        int width = int.Parse(resolutionStringVal.Split(" x ")[0]);
        int height = int.Parse(resolutionStringVal.Split(" x ")[1]);
        Screen.SetResolution(width, height, Screen.fullScreen);
    }
}
