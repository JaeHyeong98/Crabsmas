using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private PlayerState state_;
    public PlayerState state
    {
        get
        {
            return state_;
        }
        set 
        {
            if (state == PlayerState.Idle || state == PlayerState.Stop || state == PlayerState.LegDown)
                camState = CamState.Unlock;
            else
                camState = CamState.Lock;
        }
    }

    public CamState camState;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void OnMove(InputValue val)
    {
        Vector2 vec = val.Get<Vector2>();
        Debug.Log(vec);
        if(vec.x == 0)
        {
            if (vec.y > 0)//q
                state = PlayerState.Leg0Up;
            else if (vec.y < 0)//a
                state = PlayerState.Leg1Up;
            else
                state = PlayerState.LegDown;
        }
        else
        {
            if (vec.x > 0) //e
                state = PlayerState.Leg2Up;
            else //d
                state = PlayerState.Leg3Up;
        }
    }

    public void OnStop(InputValue val)
    {
        Debug.Log("Stop " + val.isPressed);
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

public enum CamState
{
    Lock,
    Unlock
}
