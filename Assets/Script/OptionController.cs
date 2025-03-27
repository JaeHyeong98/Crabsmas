using UnityEngine;
using UnityEngine.UI;

public class OptionController : MonoBehaviour
{
    public Transform canvas;
    public Transform option;
    public bool isOptionOpen;

    public Transform[] volumes = new Transform[3];
    public Slider[] sliders = new Slider[3];
    public InputField[] inputs = new InputField[3]; 

    private void Start()
    {
        canvas = this.transform;
        option = canvas.Find("Option").transform;
        option.gameObject.SetActive(false);

        Transform sounds = option.GetChild(0).Find("SoundOption");
        for(int i = 0; i < 3; i++)
        {
            volumes[i] = sounds.GetChild(i);
            sliders[i] = volumes[i].Find("Slider").GetComponent<Slider>();
            inputs[i] = volumes[i].Find("InputField").GetComponent<InputField>();
        }
    }

    public void OptionOpen()
    {
        for(int i = 0; i < 3; i++)
        {
            float val = 0;
            switch (i)
            {
                case 0:
                    val = PlayerPrefs.GetFloat("MasterVol");
                    sliders[i].value = (int)val;
                    inputs[i].text = "" + (int)val;
                    break;

                case 1:
                    val = PlayerPrefs.GetFloat("BGMVol");
                    sliders[i].value = (int)val;
                    inputs[i].text = "" + (int)val;
                    break;

                case 2:
                    val = PlayerPrefs.GetFloat("EffVol");
                    sliders[i].value = (int)val;
                    inputs[i].text = "" + (int)val;
                    break;
            }
        }
        option.gameObject.SetActive(true);
        isOptionOpen = true;
    }

    public void OptionSaveClose()
    {
        //option Setting
        PlayerPrefs.SetFloat("MasterVol", sliders[0].value);
        PlayerPrefs.SetFloat("BGMVol", sliders[1].value);
        PlayerPrefs.SetFloat("EffVol", sliders[2].value);

        OptionClose();
    }

    public void OptionClose()
    {
        option.gameObject.SetActive(false);
        isOptionOpen = false;
    }

    public void SliderOnValueChange(int num)
    {
        int val = (int)sliders[num].value;
        inputs[num].text = "" + val;
    }

    public void InputFieldSubmit(int num)
    {
        int val = int.Parse(inputs[num].text);
        sliders[num].value = val;
    }
}
