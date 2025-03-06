using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
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

    public List<LegEndPoint> legsEnd;
    public List<Transform> legs;

    public Rigidbody crabBody;  // ¸öÃ¼ Rigidbody
    public LegState state;
    public float force = 100f;

    private int legCount = 4;
    private float standardDist = 4.5f;
    Coroutine coroutine;

    private void Awake()
    {
        crabBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            RigidStopControl(true);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            RigidStopControl(false);
        }
    }

    private void FixedUpdate()
    {
        Vector3 upForce = Vector3.zero;
        Debug.Log(legCount+", "+upForce);
        //01 23 02 13
        switch (state) 
        {
            case LegState.Normal:
                upForce = Physics.gravity / 4 * -1 * crabBody.mass;
                for (int i = 0; i<legs.Count; i++)
                {
                    crabBody.AddForceAtPosition(upForce, legs[i].position);
                }
                break;

            case LegState.Leg2_parallel:
                break;

            case LegState.Leg2_diagonal:
                break;

            case LegState.No:
                break;
        }
    }
    
    public void MoveBody(Vector3 vec,LegTarget lt)
    {
        coroutine = StartCoroutine(Move(vec, lt));
        Vector3 upForce = Physics.gravity / legCount * -1;
    }

    IEnumerator Move(Vector3 vec, LegTarget lt)
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

            for (int i = 0; i < legsEnd.Count; i++)
            {
                if (!legsEnd[i].isDeath)
                {
                    float dist = DistanceCheck(legsEnd[i].transform);
                    if (legsEnd[i] != lt && (dist > 7.5f || dist < 3f))
                    {
                        legsEnd[i].EscapeLeg();
                        LegStateChage(i);

                        Vector3 downForce = Vector3.down * 40f;
                        Vector3 upForce = Vector3.up * legsEnd.Count * 10f;
                        crabBody.AddForce(downForce + upForce);
                    }
                }
            }

            yield return null;
        }
        coroutine = null;
    }

    public void RigidStopControl(bool val)
    {
        crabBody.isKinematic = val;
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public float DistanceCheck(Transform t)
    {
        Vector3 ltPos = new Vector3(t.position.x, 0, t.position.z);
        Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);
        float dist = Vector3.Distance(ltPos, pos);
        return dist;
    }

    public void LegStateChage(int num)
    { 
        legCount--;
        num = int.Parse(legs[num].name.Split('_')[1]);
        int preNum = 0;
        switch (legCount)
        {
            case 0:
                state = LegState.No;
                break;

            case 1:
                state = LegState.Leg1;
                break;

            case 2:
                {
                    for (int i = 0; i < legs.Count; i++)
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
}
