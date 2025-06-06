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

    private bool isInit = false;

    public Rigidbody crabBody;  // 몸체 Rigidbody
    public LegState state;
    public float force = 100f;
    public float upDownSpeed = 1f;
    public int legCount = 0;
    public int maxLegCount = 0;
    public float standardDist = 2.5f;

    public Transform poles;
    public LegTarget[] legTargets;
    public LegEndPoint[] legsEnd;
    public Transform[] legs;
    
    Coroutine coroutine;
    Coroutine rotateCoroutine;
    Coroutine upDownCoroutine;

    private bool checkOrgPos;
    private int lastMoveLeg;
    private Vector3 targetVec; //몸체 기울기 목표 값
    Vector3 rotationPerSecond = new Vector3(10f, 0, 10f);

    private void Awake()
    {
        StartCoroutine(Init());
    }

    public IEnumerator Init()
    {
        crabBody = GetComponent<Rigidbody>();
        legCount = 0;
        legTargets = new LegTarget[8];
        legsEnd = new LegEndPoint[8];
        legs = new Transform[8];

        yield return new WaitUntil(() => GSC.playerController != null);
        GSC.playerController.player = this;
        isInit = true;

        yield return new WaitUntil(() => GSC.cameraController != null);
        GSC.cameraController.Target = transform.Find("CamTarget").gameObject;
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

    public void BodyUpDown(float n)
    {
        float dist = 0;
        dist = RayCastCheck();

        if (n == 0 && upDownCoroutine != null)
        {
            StopCoroutine(upDownCoroutine);
            upDownCoroutine = null;
        }
        else if (n != 0 && (state == LegState.Normal || state == LegState.Leg3))
        {
            upDownCoroutine = StartCoroutine(BodyUpDow_Fn(n));
        }

    }

    public float RayCastCheck()
    {
        RaycastHit hit;
        float dist = 0;
        if (Physics.Raycast(transform.position, transform.up * -1, out hit))
        {
            // 닿은 물체의 이름을 출력
            dist = hit.distance;
        }

        return dist;
    }

    IEnumerator BodyUpDow_Fn(float n)
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            Debug.Log("press");
            if (n > 0) // 몸 상승
            {
                transform.position += (Vector3.up * upDownSpeed * Time.deltaTime);
            }
            else if (n < 0) // 몸 하강
            {
                transform.position += (Vector3.up * upDownSpeed * -1 * Time.deltaTime);
            }
            else if (n == 0)
            {
                break;
            }
            LegEscapeCheck();
            if (RayCastCheck() < 0.03f || RayCastCheck() > 7f)
                break;
        }
            
    }

    private void MoveLeg() // 플레이어 스테이트에 따라 다리 들리는 기능
    {
        if (!isInit) return;
        if(maxLegCount == 8)
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
                    {
                        lastMoveLeg = 0;
                        if (!legsEnd[0].isDeath)
                            legTargets[0].OnTakeOff();
                        if (!legsEnd[2].isDeath)
                            legTargets[2].OnTakeOff();
                    }
                    break;

                case PlayerState.Leg1Up:
                    {
                        lastMoveLeg = 1;
                        if (!legsEnd[1].isDeath)
                            legTargets[1].OnTakeOff();
                        if (!legsEnd[3].isDeath)
                            legTargets[3].OnTakeOff();
                    }
                    break;
                case PlayerState.Leg2Up:
                    {
                        lastMoveLeg = 4;
                        if (!legsEnd[4].isDeath)
                            legTargets[4].OnTakeOff();
                        if (!legsEnd[6].isDeath)
                            legTargets[6].OnTakeOff();
                    }
                    break;
                case PlayerState.Leg3Up:
                    {
                        lastMoveLeg = 5;
                        if (!legsEnd[5].isDeath)
                            legTargets[5].OnTakeOff();
                        if (!legsEnd[7].isDeath)
                            legTargets[7].OnTakeOff();
                    }
                    break;

                case PlayerState.LegDown:
                    legTargets[lastMoveLeg].OnLand();
                    legTargets[lastMoveLeg+2].OnLand();
                    break;

            }
        }
        else if(maxLegCount == 4)
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
        
    } 

    public void BodyRotation() // 다리로 몸 기울기 설정
    {
        Vector3 vec = Vector3.zero;
        int cnt = 0;
        for(int i = 0; i < legCount; i++)
        {
            if (legsEnd[i]!= null & !legsEnd[i].isDeath && legTargets[i].landGround != null)
            {
                vec.x -= legTargets[i].landGround.eulerAngles.x;
                vec.y += 0;
                vec.z -= legTargets[i].landGround.eulerAngles.z;
                cnt++;
            }
        }

        vec = vec / cnt;
        Vector3 eul = new Vector3(vec.z, vec.y, vec.x);
        targetVec = eul;

        Debug.Log(eul);

        if (rotateCoroutine == null)
            rotateCoroutine = StartCoroutine(BodyRotate());
        else
        {
            StopCoroutine(rotateCoroutine);
            rotateCoroutine = StartCoroutine(BodyRotate());
        }
    }

    IEnumerator BodyRotate()
    {
        while(Mathf.Abs(targetVec.x - transform.eulerAngles.x) < 0.5f && Mathf.Abs(targetVec.z - transform.eulerAngles.z) < 0.5f)
        {
            yield return new WaitForFixedUpdate();
            Debug.Log("dd");
            //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetVec, 0.1f);
        }
    }

    private void GravityLegForce() // 몸 중력 적용 시점 변경
    {
        Vector3 upForce = Vector3.zero;
        //01 23 02 13
        switch (state)
        {
            case LegState.Normal:
                break;

            case LegState.Leg2_parallel:
                crabBody.useGravity = true;
                break;

            case LegState.Leg2_diagonal: // safe
                break;

            case LegState.No:
                crabBody.useGravity = true;
                break;
        }

        //poles.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }
    
    public void MoveBody(Vector3 vec,LegTarget lt) // 몸 이동
    {
        //Debug.Log("[Body] MoveBody");
        if(coroutine == null)
            coroutine = StartCoroutine(Move(vec, lt));
        else
        {
            StopCoroutine(coroutine);
            crabBody.linearVelocity = Vector3.zero;
            crabBody.angularVelocity = Vector3.zero;
            crabBody.isKinematic = true;
            coroutine = StartCoroutine(Move(vec, lt));
        }
    }

    IEnumerator Move(Vector3 vec, LegTarget lt) // 몸 이동 기능
    {
        crabBody.AddForce(vec * force);
        float dist = Mathf.Abs(DistanceCheck(lt.transform));
        while (true)
        {
            LegEscapeCheck();

            //Debug.Log(Mathf.Abs(DistanceCheck(lt.transform)));
            if (dist <= standardDist)
            {
                crabBody.linearVelocity = Vector3.zero;
                crabBody.angularVelocity = Vector3.zero;
                crabBody.isKinematic = true;
                break;
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
        Vector3 vec = new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z);
        float dist = Vector3.Distance(t.position, vec);
        return dist;
    }

    private void LegEscapeCheck() // 다리 떨어져 나가는 시점 계산
    {
        for (int i = 0; i < maxLegCount; i++)
        {
            if (legsEnd[i]!=null && !legsEnd[i].isDeath)
            {
                float dist = DistanceCheck(legTargets[i].transform);
                //Debug.Log(i + " dist = " + dist);
                //Debug.Log(i+", "+dist);
                if((i== 3 || i == 7) && (dist > 3.3f || dist < 1.2f)) // 짧은다리
                {
                    legsEnd[i].EscapeLeg();
                    legTargets[i].gameObject.SetActive(false);
                    crabBody.linearVelocity = Vector3.zero;
                    crabBody.angularVelocity = Vector3.zero;
                    crabBody.isKinematic = true;
                    LegStateChage(i);
                }
                else if (dist > 3.65f || dist < 1.3f) // 긴 다리
                {
                    legsEnd[i].EscapeLeg();
                    legTargets[i].gameObject.SetActive(false);
                    crabBody.linearVelocity = Vector3.zero;
                    crabBody.angularVelocity = Vector3.zero;
                    crabBody.isKinematic = true;
                    LegStateChage(i);
                }
            }
        }
    }

    public void LegStateChage(int num) // 다리 갯수 상태 변화
    { 
        legCount--;
        num = int.Parse(legs[num].name.Split('_')[1]);
        int preNum = 0;
        //Debug.Log(legCount);
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
                    for (int i = 0; i < maxLegCount; i++)
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
        if (!isInit) return;
        if (GSC.playerController.moveState == LegMoveState.Move && !legsEnd[lastMoveLeg].isDeath)
        {
            Vector3 orgPos = Vector3.zero;
            if (!checkOrgPos)
            {
                orgPos = legTargets[lastMoveLeg].transform.localPosition;
                checkOrgPos = true;
            }

            Vector3 val = new Vector3(GSC.inputController.look.y, 1.5f, GSC.inputController.look.x).normalized;
            Vector3 localMovement = Quaternion.Inverse(transform.rotation) * val;
            //Vector3 nextPos = orgPos + val;
            if(maxLegCount == 4)
                legTargets[lastMoveLeg].transform.Translate(localMovement * 5f * Time.deltaTime, Space.Self);

            else if (maxLegCount == 8)
            {
                legTargets[lastMoveLeg].transform.Translate(localMovement * 5f * Time.deltaTime, Space.Self);
                legTargets[lastMoveLeg+2].transform.Translate(localMovement * 5f * Time.deltaTime, Space.Self);
            }
            //legTargets[lastMoveLeg].transform.localPosition += val;
        }
        else
        {
            if(checkOrgPos) checkOrgPos = false;
            return;
        }
    }
}
