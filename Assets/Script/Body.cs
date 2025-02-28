using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    private Transform parent;
    public List<LegEndPoint> legsEnd;
    public List<Transform> legs;
    public List<Rigidbody> legRigid;

    public Rigidbody crabBody;  // ¸öÃ¼ Rigidbody
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
        
        while(true)
        {
            float dist = DistanceCheck(lt.transform);

            for (int i = 0; i < 4; i++)
            {
                if (legsEnd[i] != lt)
                {
                    float distt = DistanceCheck(legsEnd[i].transform);

                    if(distt > 7.5f || distt < 3f)
                    {
                        legsEnd[i].EscapeLeg();
                        legsEnd.RemoveAt(i);
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

    
}
