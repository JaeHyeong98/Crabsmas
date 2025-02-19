﻿using System.Collections;
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

    private void Start()
    {
        curState = PointState.Idle;

        prePos = transform.position;
        curPos = transform.position;

        body.AddLeg(transform);
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


    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit " + other.name);
        if(other.CompareTag("Ground") && curState == PointState.Idle) // 현재 땅에서 가만히 있는 상태에서 탈출 할 때
        {
            curState = PointState.TakeOff; // 타겟이 공중에 뜬 상태
        }
    }

    private void LandAction()
    {
        curPos = transform.position;
        curState = PointState.Idle;
        Vector3 change = Vector3.Lerp(curPos - prePos, Vector3.zero, 0.3f); // 다리 이동거리의 70%

        body.MoveBody(change,transform.name);
    }
}
