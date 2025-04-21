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

    private bool isInit = false;

    public Rigidbody crabBody;  // ��ü Rigidbody
    public LegState state;
    public float force = 100f;
    public float upDownSpeed = 1f;
    private int legCount = 4;
    private float standardDist = 4.5f;

    public Transform poles;
    public LegTarget[] legTargets;
    public LegEndPoint[] legsEnd;
    public Transform[] legs;
    
    Coroutine coroutine;
    Coroutine upDownCoroutine;

    private bool checkOrgPos;
    private int lastMoveLeg;

    private void Awake()
    {
        StartCoroutine(Init());
    }

    public IEnumerator Init()
    {
        crabBody = GetComponent<Rigidbody>();
        legTargets = new LegTarget[4];
        legsEnd = new LegEndPoint[4];
        legs = new Transform[4];

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
            // ���� ��ü�� �̸��� ���
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
            if (n > 0) // �� ���
            {
                transform.position += (Vector3.up * upDownSpeed * Time.deltaTime);
            }
            else if (n < 0) // �� �ϰ�
            {
                transform.position += (Vector3.up * upDownSpeed * -1 * Time.deltaTime);
            }
            else if (n == 0)
            {
                break;
            }
            LegEscapeCheck();
            if (RayCastCheck() < 0.8f || RayCastCheck() > 7f)
                break;
        }
            
    }

    private void MoveLeg() // �÷��̾� ������Ʈ�� ���� �ٸ� �鸮�� ���
    {
        if (!isInit) return;
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

    public void BodyRotation() // �ٸ� 4���� �� ���� ����
    {
        if (legCount < 3) return;
        int cnt = 0;
        Vector3[] legs = new Vector3[4];
        Vector3 normalVector = Vector3.zero;
        Vector3 forwardVector = Vector3.zero;

        for (int i=0; i<legCount; i++)
        {
            if (!legsEnd[i].isDeath)
            {
                legs[cnt] = legTargets[i].transform.position;
                cnt++;
            }
        }
        
        switch(legCount)
        {
            case 4:
                // ù ��° ���� ���Ϳ� ���� ����
                Vector3 v1_1 = legs[1] - legs[0];
                Vector3 v1_2 = legs[2] - legs[0];
                Vector3 normal1 = Vector3.Cross(v1_1, v1_2).normalized;

                // �� ��° ���� ���Ϳ� ���� ����
                Vector3 v2_1 = legs[3] - legs[0];
                Vector3 v2_2 = legs[2] - legs[1];
                Vector3 normal2 = Vector3.Cross(v2_1, v2_2).normalized;

                // �� ���� ������ ����� ����ϰ� ����ȭ
                normalVector = (normal1 + normal2).normalized;

                forwardVector = (v1_1 - Vector3.Project(v1_1, normalVector)).normalized;
                break;
        
            case 3:
                Vector3 v1 = legs[1] - legs[0];
                Vector3 v2 = legs[2] - legs[0];
                normalVector = Vector3.Cross(v1, v2).normalized;

                // ����� forward ���͸� ��� (v1�� normalVector�� ������ �������� ����)
                forwardVector = (v1 - Vector3.Project(v1, normalVector)).normalized;
                break;
        }
        
        // Quaternion.LookRotation�� ����Ͽ� rotation ����
        Quaternion targetRotation = Quaternion.LookRotation(forwardVector, normalVector);

        Debug.Log(targetRotation.eulerAngles);
        //transform.rotation = targetRotation;
    }

    private void GravityLegForce() // �� �߷� ���� ���� ����
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
    
    public void MoveBody(Vector3 vec,LegTarget lt) // �� �̵�
    {
        //Debug.Log("[Body] MoveBody");
        coroutine = StartCoroutine(Move(vec, lt));
    }

    IEnumerator Move(Vector3 vec, LegTarget lt) // �� �̵� ���
    {
        Vector3 moveDir = vec.normalized;

        crabBody.AddForce(moveDir * force);

        while (true)
        {
            if (DistanceCheck(lt.transform) < standardDist)
            {
                crabBody.AddForce(moveDir * force * -1);
                break;
            }

            LegEscapeCheck();

            yield return null;
        }
        coroutine = null;
    }

    public void RigidStopControl(bool val) // ��ü ���� on off
    {
        crabBody.isKinematic = val;
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public float DistanceCheck(Transform t) // ���� �� ���� ���� �Ÿ�
    {
        float dist = Vector3.Distance(t.position, transform.position);
        return dist;
    }

    private void LegEscapeCheck() // �ٸ� ������ ������ ���� ���
    {
        for (int i = 0; i < 4; i++)
        {
            if (!legsEnd[i].isDeath)
            {
                float dist = DistanceCheck(legsEnd[i].transform);
                if (dist > 7.5f || dist < 3f)
                {
                    legsEnd[i].EscapeLeg();
                    legTargets[i].gameObject.SetActive(false);
                    LegStateChage(i);
                }
            }
        }
    }

    public void LegStateChage(int num) // �ٸ� ���� ���� ��ȭ
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

    public void MoveTarget() // ���콺�� �ٸ� �̵� ���
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

            Vector3 val = new Vector3(GSC.inputController.look.y, 0f, GSC.inputController.look.x).normalized * 0.05f;
            Vector3 nextPos = orgPos + val;
            legTargets[lastMoveLeg].transform.localPosition += val;
            if (MathF.Abs(Vector3.Distance(nextPos, orgPos)) <= 1f)
            {
                
            }
        }
        else
        {
            if(checkOrgPos) checkOrgPos = false;
            return;
        }
    }
}
