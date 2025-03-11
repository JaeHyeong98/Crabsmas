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
            if (state == PlayerState.Idle || state == PlayerState.CanMove || state == PlayerState.LegDown)
                camState = CamState.Unlock;
            else
                camState = CamState.Lock;
        }
    }

    public CamState camState;

    public void OnMove(InputValue val)
    {
        Debug.Log(val.Get<Vector2>());
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            state = PlayerState.Idle;

        if (Input.GetKeyUp(KeyCode.Space))
            state = PlayerState.CanMove;

        if(Input.GetKeyDown(KeyCode.Q))
            state = PlayerState.Leg0Up;

        if (Input.GetKeyDown(KeyCode.A))
            state = PlayerState.Leg1Up;

        if (Input.GetKeyDown(KeyCode.E))
            state = PlayerState.Leg2Up;

        if (Input.GetKeyDown(KeyCode.D))
            state = PlayerState.Leg3Up;

        if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.D))
            state = PlayerState.LegDown;
    }
}
public enum PlayerState
{
    Idle,
    CanMove,
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

public enum LegPointState
{
    Moveable,
    UnMoveable
}
