using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionController : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    
    private Resolution[] resolutions;
    private GameObject scrollView;

    void Start()
    {
        transform.Find("Btn").GetComponent<Button>().onClick.AddListener(OpenResolution);
        scrollView = transform.Find("Scroll View").gameObject;

        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    public void OpenResolution()
    {
        scrollView.SetActive(true);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        scrollView.SetActive(false);
    }

    public void SetFullscreen(bool val)
    {
        Screen.fullScreen = fullscreenToggle.isOn;
    }
}
