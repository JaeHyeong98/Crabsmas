using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    private Transform parent;
    public List<LegEndPoint> legsEnd;
    public List<Transform> legs;
    public List<Rigidbody> legRigid;
    public Vector3 originalCenterOfMass;

    public Rigidbody crabBody;  // 몸체 Rigidbody
    public float force = 100f;

    private float standardDist = 4.5f;
    Coroutine coroutine;

    private void Awake()
    {
        parent = transform.parent.transform;
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


    public void AddLeg(Transform t)
    { 
        legs.Add(t);
        legRigid.Add(t.GetComponent<Rigidbody>());
    }
    
    public void MoveBody(Vector3 vec,LegTarget lt)
    {
        coroutine = StartCoroutine(Move(vec, lt));
    }

    IEnumerator Move(Vector3 vec, LegTarget lt)
    {
        Vector3 moveDir = vec.normalized;
        moveDir = new Vector3(moveDir.x, 0, moveDir.z);

        crabBody.AddForce(moveDir * force);

        while (true)
        {
            float dist = DistanceCheck(lt.transform);

            for (int i = 0; i < legsEnd.Count; i++)
            {
                if (legsEnd[i] != lt)
                {
                    float distt = DistanceCheck(legsEnd[i].transform);

                    if(distt > 7.5f || distt < 3f)
                    {
                        legsEnd[i].EscapeLeg();
                        UpdateCenterOfMass(legsEnd[i].transform.position);
                        legsEnd.RemoveAt(i);


                        Vector3 downForce = Vector3.down * 40f;
                        Vector3 upForce = Vector3.up * legsEnd.Count * 10f;
                        crabBody.AddForce(downForce + upForce);
                        Debug.Log(downForce + upForce);
                    }
                }
            }

            if (dist < standardDist)
            {
                crabBody.AddForce(moveDir * force * -1);
                break;
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

    void UpdateCenterOfMass(Vector3 legPosition)
    {
        Vector3 newCenterOfMass = originalCenterOfMass;
        int detachedLegCount = 0;

        newCenterOfMass += CalculateLegCenterOfMassOffset(legPosition);

        if (detachedLegCount > 0)
        {
            newCenterOfMass /= (detachedLegCount + 1); // 평균 무게 중심 계산
            crabBody.centerOfMass = newCenterOfMass;
        }
        else
        {
            crabBody.centerOfMass = originalCenterOfMass; // 모든 다리가 붙어있으면 초기 무게 중심으로 복원
        }
    }

    Vector3 CalculateLegCenterOfMassOffset(Vector3 legPosition)
    {
        // 떨어진 다리의 위치를 기반으로 무게 중심 이동량 계산
        // 예시: 몸통 중심에서 다리 위치를 뺀 벡터를 사용하여 무게 중심 이동량 계산
        Vector3 offset = transform.position - legPosition;

        // 이동량 벡터의 크기를 조정하여 무게 중심 이동 강도 조절
        offset = offset.normalized * 0.1f; // 0.1은 이동 강도 조절 값 (조절 가능)

        return offset;
    }
}
