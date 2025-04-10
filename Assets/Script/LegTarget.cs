using System.Collections;
using Main;
using UnityEngine;

public class LegTarget : MonoBehaviour
{
    public Body body;

    public enum PointState
    {
        Idle,
        TakeOff,
        Land
    }

    public PointState curState;
    private Vector3 prePos;
    private Vector3 curPos;
    private Vector3 preMove;
    private Rigidbody rb;

    private void OnEnable()
    {
        StartCoroutine(Init());
    }

    public IEnumerator Init()
    {
        //Debug.Log("[LegTarget] Init Start");
        yield return new WaitUntil(() => GSC.playerController != null && GSC.playerController.player != null);
        //Debug.Log("[LegTarget] waituntil end");
        body = GSC.playerController.player;
        curState = PointState.Idle;

        prePos = transform.position;
        curPos = transform.position;

        rb = GetComponent<Rigidbody>();

        for (int i = 0; i < 4; i++)
        {
            if (transform.name.Contains(i.ToString()))
            {
                body.legTargets[i] = this;
                break;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground") && curState == PointState.TakeOff) // 공중에 떠있는 상태에서 타겟이 땅에 다인 경우
        {
            curState = PointState.Land; // 타겟이 땅에 내린 상태
            LandAction();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground") && curState == PointState.Idle) // 현재 땅에서 가만히 있는 상태에서 탈출 할 때
        {
            curState = PointState.TakeOff; // 타겟이 공중에 뜬 상태
            body.RigidStopControl(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter " + other.name);
        if (other.CompareTag("Ground") && curState == PointState.TakeOff) // 공중에 떠있는 상태에서 타겟이 땅에 다인 경우
        {
            curState = PointState.Land; // 타겟이 땅에 내린 상태
            LandAction();
        }

        
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("OnTriggerStay " + other.name);
        if (other.CompareTag("Goal") && curState == PointState.Idle) // 다리가 결승점에 착지한 경우
        {
            GSC.main.GameClear();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit " + other.name);
        if(other.CompareTag("Ground") && curState == PointState.Idle) // 현재 땅에서 가만히 있는 상태에서 탈출 할 때
        {
            curState = PointState.TakeOff; // 타겟이 공중에 뜬 상태
            body.RigidStopControl(true);
        }
    }

    private void LandAction()
    {
        //GSC.audioController.PlaySound2D("Click");
        GSC.audioController.PlaySound3D("Walk",transform,0,false,SoundType.Eff,true,8,80);
        body.RigidStopControl(false);
        curPos = transform.position;
        curState = PointState.Idle;
        
        if (prePos == curPos)
            body.MoveBody(preMove, this);
        else
        {
            Vector3 change = (curPos - prePos).normalized; //이동 방향
            body.MoveBody(change, this);
            preMove = change;
        }

        prePos = curPos;
    }

    public void OnTakeOff()
    {
        rb.isKinematic = true;
        transform.position = new Vector3(transform.position.x, 2f, transform.position.z);
    }

    public void OnLand()
    {
        rb.isKinematic = false;
    }

    public void KinematicOnOff(bool val)
    {
        transform.gameObject.layer = 6;
        rb.isKinematic = val;
    }
}
