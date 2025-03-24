using Main;
using UnityEngine;

public class IntroCamAniEventHandler : MonoBehaviour
{
    public void IntroEnd()
    {
        GSC.main.canControl = true;
        GSC.cameraController.camState = CamState.Unlock;
        GSC.cameraController.CamChange();
    }
}
