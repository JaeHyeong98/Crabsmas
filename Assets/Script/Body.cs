using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    private Transform parent;
    public List<LegEndPoint> legsEnd;
    public List<Transform> legs;
    public List<Rigidbody> legRigid;

    public Rigidbody crabBody;  // ��ü Rigidbody
    public float pullForce = 10f;
    public float pushForce = 2f; // �и��� �ٸ��� ������ ��
    private Transform selectedLeg;

    private void Awake()
    {
        parent = transform.parent.transform;
        crabBody = parent.GetComponent<Rigidbody>();
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => legsEnd.Count > 0);

    }

    public void AddLeg(Transform t)
    { 
        legs.Add(t);
        legsEnd.Add(t.GetComponent<LegEndPoint>());
        legRigid.Add(t.GetComponent<Rigidbody>());
    }
    
    //void FixedUpdate()
    //{
    //    if (selectedLeg != null)
    //    {
    //        // ���õ� �ٸ� �������� ��ü ����
    //        Vector3 pullDirection = (selectedLeg.position - crabBody.position).normalized;
    //        crabBody.AddForce(pullDirection * pullForce, ForceMode.Acceleration);
    //
    //        // ������ �ٸ��� �ݴ� �������� �з����� ȿ�� ����
    //        foreach (Rigidbody legRb in legRigid)
    //        {
    //            if (legRb.transform != selectedLeg)
    //            {
    //                Vector3 pushDir = (legRb.position - crabBody.position).normalized;
    //                legRb.AddForce(pushDir * pushForce, ForceMode.Acceleration);
    //            }
    //        }
    //    }
    //}

    public void MoveBody(Vector3 vec,string name)
    {
        int n = int.Parse(name.Split("_")[1]);
        selectedLeg = legs[n];
        Debug.Log(n);
        //StartCoroutine(BodyCorrection(vec));
    }

    IEnumerator BodyCorrection(Vector3 vec) // �ٸ� �� ������ ��ȭ���� ���� ���� �ٸ� �ٸ��� ������
    {
        Vector3 pNextPos = parent.transform.position + vec;
        Debug.Log("[body] " + parent.transform.position);
        Debug.Log("[body] " + pNextPos);

        while(Vector3.Distance(parent.transform.position, pNextPos) > 0.1f)
        {
            parent.transform.position = Vector3.MoveTowards(parent.transform.position, pNextPos, 0.1f);
            for (int i = 0; i < legsEnd.Count; i++)
                legsEnd[i].transform.position = Vector3.MoveTowards(legsEnd[i].transform.position, legsEnd[i].transform.position-vec, 0.1f);
            yield return new WaitForSeconds(0.2f);

            if (Vector3.Distance(parent.transform.position, pNextPos) <= 0.1f)
                break;
        }
        yield return null;
    }

    IEnumerator LegEndCorrection(Vector3 vec) // �ٸ� �� ������ ���� �ʹ� ����� �� ��ġ ������
    {
        Vector3 pNextPos = parent.transform.position + vec;
        Debug.Log("[body] " + parent.transform.position);
        Debug.Log("[body] " + pNextPos);

        while (Vector3.Distance(parent.transform.position, pNextPos) > 0.1f)
        {
            parent.transform.position = Vector3.MoveTowards(parent.transform.position, pNextPos, 0.1f);
            for (int i = 0; i < legsEnd.Count; i++)
                legsEnd[i].transform.position = Vector3.MoveTowards(legsEnd[i].transform.position, legsEnd[i].transform.position - vec, 0.1f);
            yield return new WaitForSeconds(0.2f);

            if (Vector3.Distance(parent.transform.position, pNextPos) <= 0.1f)
                break;
        }
        yield return null;
    }
}
