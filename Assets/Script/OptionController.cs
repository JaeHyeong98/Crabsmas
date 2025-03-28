using UnityEngine;
using UnityEngine.UI;

public class OptionController : MonoBehaviour
{
    public Transform canvas;
    public Transform option;
    public bool isOptionOpen;

    public SoundOptionController sound;
    public ResolutionController resolution;


    private void Start()
    {
        canvas = this.transform;
        option = canvas.Find("Option").transform;
        option.gameObject.SetActive(false);

        sound = option.GetChild(0).Find("SoundOption").GetComponent<SoundOptionController>();
        resolution = option.GetChild(0).Find("Resolution").GetComponent<ResolutionController>();

    }

    public void OptionOpen()
    {
        sound.SoundUIInit();
        option.gameObject.SetActive(true);
        isOptionOpen = true;
    }

    public void OptionSaveClose()
    {
        sound.SoundOptionSave();
        OptionClose();
    }

    public void OptionClose()
    {
        option.gameObject.SetActive(false);
        isOptionOpen = false;
    }

}
