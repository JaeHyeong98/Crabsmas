using UnityEngine;
using Main;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public Vector2 look; // input value
    
    public Vector2 move
    {
        get
        {
            return move_;
        }
        set
        {
            move_ = value;
            GSC.playerController.OnMoveFn(move_);
        }
    }
    private Vector2 move_;

    public float zoom
    {
        get
        {
            return zoom_;
        }
        set
        {
            zoom_ = value;
            GSC.cameraController.ZoomFn(zoom_);
        }
    }
    private float zoom_;

    public bool camLock
    {
        get
        {
            return camLock_;
        }
        set
        {
            camLock_ = value;
            GSC.cameraController.CamLockUp(camLock_);
        }
    }
    private bool camLock_;

    public bool option
    {
        get
        {
            return option_;
        }
        set
        {
            option_ = value;
            GSC.uiController.PauseUI();
        }
    }
    private bool option_;

    public bool click
    {
        get
        {
            return click_;
        }
        set
        {
            click_ = value;
        }
    }
    private bool click_;

    public float upDown
    {
        get
        {
            return upDown_;
        }
        set
        {
            upDown_ = value;
            GSC.playerController.player.BodyUpDown(upDown_);
        }
    }
    private float upDown_;

    private void Awake()
    {
        GSC.inputController = this;
        option_ = false;
    }

    public void OnLook(InputValue value)
    {
        if (!GSC.main.canControl || option) return;
        look = value.Get<Vector2>();
    }

    public void OnMove(InputValue val)
    {
        if (!GSC.main.canControl || option) return;
        move = val.Get<Vector2>();
    }

    public void OnZoom(InputValue value)
    {
        if (!GSC.main.canControl || option) return;
        zoom = value.Get<float>();
    }

    public void OnCamLock(InputValue value)
    {
        if (!GSC.main.canControl || option) return;
        camLock = value.isPressed;
    }

    public void OnOption(InputValue value)
    {
        option = !option;
    }

    public void OnClick(InputValue value)
    {
        click = value.isPressed;
    }

    public void OnUpDown(InputValue value)
    {
        upDown = value.Get<float>();
    }
}
