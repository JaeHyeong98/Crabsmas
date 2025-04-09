using Main;
using UnityEngine;
using UnityEngine.UI;

public class SoundOptionController : MonoBehaviour
{
    public Transform[] volumes = new Transform[3];
    public Slider[] sliders = new Slider[3];
    public InputField[] inputs = new InputField[3];
    private int[] preVol = new int[3];

    private void Awake()
    {
        Transform sounds = transform;
        for (int i = 0; i < 3; i++)
        {
            volumes[i] = sounds.GetChild(i);
            sliders[i] = volumes[i].Find("Slider").GetComponent<Slider>();
            inputs[i] = volumes[i].Find("InputField").GetComponent<InputField>();
        }
    }

    public void SoundUIInit()
    {
        for (int i = 0; i < 3; i++)
        {
            string val = "";
            switch (i)
            {
                case 0:
                    val = PlayerPrefs.GetString("Master");
                    break;

                case 1:
                    val = PlayerPrefs.GetString("BGM");
                    break;

                case 2:
                    val = PlayerPrefs.GetString("Eff");
                    break;
            }

            int vol = (int)float.Parse(val);
            sliders[i].value = vol;
            inputs[i].text = "" + vol;
            preVol[i] = vol;
        }
    }

    public void Save()
    {
        //option Setting
        PlayerPrefs.SetString("Master", sliders[0].value.ToString());
        PlayerPrefs.SetString("BGM", sliders[1].value.ToString());
        PlayerPrefs.SetString("Eff", sliders[2].value.ToString());
    }

    public void Close()
    {
        for(int i = 0; i<3; i++)
        {
            int val = preVol[i];
            SetVolume(i, val);
        }
    }

    public void SliderOnValueChange(int num)
    {
        int val = (int)sliders[num].value;
        inputs[num].text = "" + val;
        preVol[num] = val;

        SetVolume(num, val);
    }

    public void InputFieldSubmit(int num)
    {
        int val = int.Parse(inputs[num].text);
        sliders[num].value = val;
        preVol[num] = val;

        SetVolume(num, val);
    }

    private void SetVolume(int idx, int vol)
    {
        switch (idx)
        {
            case 0:
                GSC.audioController.SetVolume(SoundType.Master, vol - 80);
                break;
            case 1:
                GSC.audioController.SetVolume(SoundType.BGM, vol - 80);
                break;
            case 2:
                GSC.audioController.SetVolume(SoundType.Eff, vol - 80);
                break;
        }
    }
}
