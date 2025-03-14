using Main;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public Transform canvas;
    private int lastKey = 0;

    public PlayerState state
    {
        get
        {
            return state_;
        }
        set 
        {
            state_ = value;
            if (state == PlayerState.Idle || state == PlayerState.Stop || state == PlayerState.LegDown)
                moveState = LegMoveState.Stop;
            else
                moveState = LegMoveState.Move;
        }
    }
    private PlayerState state_;

    public LegMoveState moveState;


    void Awake()
    {
        GSC.playerController = this;
        state = PlayerState.Idle;
        moveState = LegMoveState.Stop;
    }

    public void OnMoveFn(Vector2 vec)
    {
        if(vec.x == 0)
        {
            if (vec.y > 0)//q
            {
                canvas.GetChild(0).GetChild(0).gameObject.SetActive(true);
                lastKey = 0;
                state = PlayerState.Leg0Up;
                GSC.cameraController.camState = CamState.Lock;
                
            }
            else if (vec.y < 0)//a
            {
                canvas.GetChild(1).GetChild(0).gameObject.SetActive(true);
                lastKey = 1;
                state = PlayerState.Leg1Up;
                GSC.cameraController.camState = CamState.Lock;
            }
            else
            {
                canvas.GetChild(lastKey).GetChild(0).gameObject.SetActive(false);
                state = PlayerState.LegDown;
            }
        }
        else
        {
            if (vec.x > 0) //e
            {
                canvas.GetChild(2).GetChild(0).gameObject.SetActive(true);
                lastKey = 2;
                state = PlayerState.Leg2Up;
                GSC.cameraController.camState = CamState.Lock;
            }
            else //d
            {
                canvas.GetChild(3).GetChild(0).gameObject.SetActive(true);
                lastKey = 3;
                state = PlayerState.Leg3Up;
                GSC.cameraController.camState = CamState.Lock;
            }
        }
        Debug.Log(state);
    }

    public void OnStop(InputValue val)
    {
        if (val.isPressed)
            state = PlayerState.Stop;
        else
            state = PlayerState.Idle;
    }

}
public enum PlayerState
{
    Idle,
    Stop,
    Leg0Up,
    Leg1Up,
    Leg2Up,
    Leg3Up,
    LegDown
}

public enum LegMoveState
{
    Move,
    Stop
}

