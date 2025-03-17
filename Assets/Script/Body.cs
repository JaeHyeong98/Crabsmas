using System;
using System.Collections;
using Main;
using UnityEngine;

public class Body : MonoBehaviour
{
    public enum LegState
    {
        Normal,
        Leg3,
        Leg2_diagonal,
        Leg2_parallel,
        Leg1,
        No
    }

    public Rigidbody crabBody;  // 몸체 Rigidbody
    public LegState state;
    public float force = 100f;
    private int legCount = 4;
    private float standardDist = 4.5f;

    public Transform poles;
    public LegTarget[] legTargets;
    public LegEndPoint[] legsEnd;
    public Transform[] legs;
    
    Coroutine coroutine;

    private bool checkOrgPos;
    private int lastMoveLeg;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        GSC.playerController.player = this;

        legTargets = new LegTarget[4];
        legsEnd = new LegEndPoint[4];
        legs = new Transform[4];
        crabBody = GetComponent<Rigidbody>();

    }

    private void Update()
    {
        MoveLeg();
        MoveTarget();
    }

    private void FixedUpdate()
    {
        GravityLegForce();
    }

    private void MoveLeg() // 플레이어 스테이트에 따라 다리 들리는 기능
    {
        switch (GSC.playerController.state)
        {
            case PlayerState.Idle:
                RigidStopControl(false);
                break;

            case PlayerState.Stop:
                RigidStopControl(true);
                break;

            case PlayerState.Leg0Up:
                lastMoveLeg = 0;
                if (!legsEnd[0].isDeath)
                    legTargets[0].OnTakeOff();
                break;

            case PlayerState.Leg1Up:
                lastMoveLeg = 1;
                if (!legsEnd[1].isDeath)
                    legTargets[1].OnTakeOff();
                break;
            case PlayerState.Leg2Up:
                lastMoveLeg = 2;
                if (!legsEnd[2].isDeath)
                    legTargets[2].OnTakeOff();
                break;
            case PlayerState.Leg3Up:
                lastMoveLeg = 3;
                if (!legsEnd[3].isDeath)
                    legTargets[3].OnTakeOff();
                break;

            case PlayerState.LegDown:
                legTargets[lastMoveLeg].OnLand();
                break;

        }
    } 

    private void GravityLegForce()
    {
        Vector3 upForce = Vector3.zero;
        //01 23 02 13
        switch (state)
        {
            case LegState.Normal:
                upForce = Physics.gravity / 4 * -1 * crabBody.mass;
                for (int i = 0; i < 4; i++)
                {
                    crabBody.AddForceAtPosition(upForce, legs[i].position);
                }
                transform.localEulerAngles = Vector3.zero;
                break;

            case LegState.Leg2_parallel:
                break;

            case LegState.Leg2_diagonal:
                break;

            case LegState.No:
                break;
        }

        poles.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }
    
    public void MoveBody(Vector3 vec,LegTarget lt) // 몸 이동
    {
        Debug.Log("[Body] MoveBody");
        coroutine = StartCoroutine(Move(vec, lt));
        Vector3 upForce = Physics.gravity / legCount * -1;
    }

    IEnumerator Move(Vector3 vec, LegTarget lt) // 몸 이동 기능
    {
        Vector3 moveDir = vec.normalized;
        moveDir = new Vector3(moveDir.x, 0, moveDir.z);

        crabBody.AddForce(moveDir * force);

        while (true)
        {
            if (DistanceCheck(lt.transform) < standardDist)
            {
                crabBody.AddForce(moveDir * force * -1);
                break;
            }

            for (int i = 0; i < 4; i++)
            {
                if (!legsEnd[i].isDeath)
                {
                    float dist = DistanceCheck(legsEnd[i].transform);
                    if (legsEnd[i] != lt && (dist > 7.5f || dist < 3f))
                    {
                        legsEnd[i].EscapeLeg();
                        LegStateChage(i);

                        Vector3 downForce = Vector3.down * 40f;
                        Vector3 upForce = Vector3.up * 4 * 10f;
                        crabBody.AddForce(downForce + upForce);
                    }
                }
            }

            yield return null;
        }
        coroutine = null;
    }

    public void RigidStopControl(bool val) // 몸체 정지 on off
    {
        crabBody.isKinematic = val;
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public float DistanceCheck(Transform t) // 몸과 한 지점 간의 거리
    {
        Vector3 ltPos = new Vector3(t.position.x, 0, t.position.z);
        Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);
        float dist = Vector3.Distance(ltPos, pos);
        return dist;
    }

    public void LegStateChage(int num) // 다리 갯수 상태 변화
    { 
        legCount--;
        num = int.Parse(legs[num].name.Split('_')[1]);
        int preNum = 0;
        switch (legCount)
        {
            case 0:
                state = LegState.No;
                GSC.main.GameOver();
                break;

            case 1:
                state = LegState.Leg1;
                break;

            case 2:
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (i != num && legsEnd[i].isDeath)
                        {
                            preNum = int.Parse(legs[i].name.Split('_')[1]);
                        }
                    }

                    switch(preNum)
                    {// 01 23 02 13
                        case 0:
                            {
                                if (num == 1 || num == 2) state = LegState.Leg2_parallel;
                                else state = LegState.Leg2_diagonal;
                            }
                            break;

                        case 1:
                            {
                                if (num == 0 || num == 3) state = LegState.Leg2_parallel;
                                else state = LegState.Leg2_diagonal;
                            }
                            break;

                        case 2:
                            {
                                if (num == 0 || num == 3) state = LegState.Leg2_parallel;
                                else state = LegState.Leg2_diagonal;
                            }
                            break;

                        case 3:
                            {
                                if (num == 1 || num == 2) state = LegState.Leg2_parallel;
                                else state = LegState.Leg2_diagonal;
                            }
                            break;
                    }
                }
                break;
            //case 3:
            //    state = LegState.Leg3;
            //    break;
            default:
                state = LegState.Normal;
                break;
        }
    }

    public void MoveTarget() // 마우스로 다리 이동 기능
    {
        if (GSC.playerController.moveState == LegMoveState.Move && !legsEnd[lastMoveLeg].isDeath)
        {
            Vector3 orgPos = Vector3.zero;
            if (!checkOrgPos)
            {
                orgPos = legTargets[lastMoveLeg].transform.localPosition;
                checkOrgPos = true;
            }

            Vector3 val = new Vector3(GSC.inputController.look.y, 0f, GSC.inputController.look.x).normalized * 0.05f;
            Vector3 nextPos = orgPos + val;

            if (MathF.Abs(Vector3.Distance(nextPos, orgPos)) <= 1f)
            {
                legTargets[lastMoveLeg].transform.localPosition += val;
            }
        }
        else
        {
            if(checkOrgPos) checkOrgPos = false;
            return;
        }
    }
}
