using Main;
using UnityEngine;

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
        GSC.audioController.PlaySound2D("Click");
        option.gameObject.SetActive(true);
        sound.SoundUIInit();
        resolution.Init();
        isOptionOpen = true;
    }


    public void OptionClose(bool val)
    {
        GSC.audioController.PlaySound2D("Click");
        isOptionOpen = false;

        if(val) // save close
        {
            sound.Save();
            resolution.Save();
        }
        else // just close
        {
            resolution.Close();
        }

        option.gameObject.SetActive(false);
    }

}
