using Main;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Transform canvas;
    public Transform keyGuide;
    public Transform intro;
    public Transform clear;
    public Transform failed;

    private void Awake()
    {
        GSC.uiController = this;

        canvas = this.transform;
        keyGuide = canvas.Find("KeyGuide").transform;
        intro = canvas.Find("Intro").transform;
        intro.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(StartBtn);

        clear = canvas.Find("Clear").transform;
        failed = canvas.Find("Failed").transform;

        clear.gameObject.SetActive(false);
        failed.gameObject.SetActive(false);

        if (!intro.gameObject.activeSelf)
            {
                GSC.main.canControl = true;
            }
    }


    public void GameResult(bool val)
    {
        if(val) // clear
        {
            clear.gameObject.SetActive(true);
        }
        else // fail
        {
            failed.gameObject.SetActive(true);
        }
    }

    public void GameResultBtn(bool val)
    {
        intro.gameObject.SetActive(val);
        if (!val) // failed -> retry
        {
            GSC.main.canControl = true;
            GSC.cameraController.camState = CamState.Unlock;
        }

        clear.gameObject.SetActive(false);
        failed.gameObject.SetActive(false);
    }

    public void KeyGuide(int num, bool val)
    {
        keyGuide.GetChild(num).GetChild(0).gameObject.SetActive(val);
    }

    public void StartBtn()
    {
        intro.gameObject.SetActive(false);
        GSC.main.canControl = true;
        GSC.cameraController.camState = CamState.Unlock;
    }
}
