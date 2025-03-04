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

    public Rigidbody crabBody;  // ��ü Rigidbody
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
            newCenterOfMass /= (detachedLegCount + 1); // ��� ���� �߽� ���
            crabBody.centerOfMass = newCenterOfMass;
        }
        else
        {
            crabBody.centerOfMass = originalCenterOfMass; // ��� �ٸ��� �پ������� �ʱ� ���� �߽����� ����
        }
    }

    Vector3 CalculateLegCenterOfMassOffset(Vector3 legPosition)
    {
        // ������ �ٸ��� ��ġ�� ������� ���� �߽� �̵��� ���
        // ����: ���� �߽ɿ��� �ٸ� ��ġ�� �� ���͸� ����Ͽ� ���� �߽� �̵��� ���
        Vector3 offset = transform.position - legPosition;

        // �̵��� ������ ũ�⸦ �����Ͽ� ���� �߽� �̵� ���� ����
        offset = offset.normalized * 0.1f; // 0.1�� �̵� ���� ���� �� (���� ����)

        return offset;
    }
}
