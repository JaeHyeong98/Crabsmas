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

    public Rigidbody crabBody;  // ¸öÃ¼ Rigidbody
    public float force = 100f;
    public float legForce = 10f;
    public float centerWeight = 1f;

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

        crabBody.AddForce(Vector3.up * legForce);
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
            if (DistanceCheck(lt.transform) < standardDist)
            {
                crabBody.AddForce(moveDir * force * -1);
                break;
            }

            for (int i = 0; i < legsEnd.Count; i++)
            {
                float dist = DistanceCheck(legsEnd[i].transform);

                if (dist > 7.5f || dist < 3f)
                {
                    legsEnd[i].EscapeLeg();
                    ResetCenterOfMess(legsEnd[i].name.Split("_")[2]);

                    legsEnd.RemoveAt(i);


                    Vector3 downForce = Vector3.down * 40f;
                    Vector3 upForce = Vector3.up * legsEnd.Count * 10f;
                    crabBody.AddForce(downForce + upForce);
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

    private void ResetCenterOfMess(string name)
    {
        int num = int.Parse(name);
        Debug.Log(num);

        Vector3 orgCenter = crabBody.centerOfMass;
        Vector3 val = Vector3.zero;

        switch(num)
        {
            case 0:
                //x- y-
                val = new Vector3(-1f, 0, -1f) * centerWeight;
                break;

            case 1:
                //x+ y-
                val = new Vector3(1f, 0, -1f) * centerWeight;
                break;

            case 2:
                //x- y+
                val = new Vector3(-1f, 0, 1f) * centerWeight;
                break;

            case 3:
                //x+ y+
                val = new Vector3(1f, 0, 1f) * centerWeight;
                break;
        }

        orgCenter = orgCenter + val;
        crabBody.centerOfMass = orgCenter;
        Debug.Log(orgCenter);
    }
}
