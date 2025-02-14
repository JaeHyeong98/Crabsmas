using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Body : MonoBehaviour
{
    private Transform parent;
    public List<Transform> legs;
    public List<LegEndPoint> legsEnd;

    private void Awake()
    {
        parent = transform.parent.transform;

        for(int i = 0;i < transform.childCount;i++)
        {
            if (transform.GetChild(i).gameObject.name.Contains("Leg"))
                legs.Add(transform.GetChild(i));
        }
    }

    public void MoveBody(Vector3 vec,string name)
    {
        StartCoroutine(BodyCorrection(vec));
        StartCoroutine(OtherLegCorrection(vec,name));
    }

    IEnumerator BodyCorrection(Vector3 vec)
    {
        Vector3 pNextPos = parent.transform.position + vec;
        Debug.Log("[body] " + parent.transform.position);
        Debug.Log("[body] " + pNextPos);

        while(Vector3.Distance(parent.transform.position, pNextPos) > 0.1f)
        {
            parent.transform.position = Vector3.MoveTowards(parent.transform.position, pNextPos, 0.1f);
            yield return new WaitForSeconds(0.2f);

            if (Vector3.Distance(parent.transform.position, pNextPos) <= 0.1f)
                break;
        }
        
        yield return null;
    }

    IEnumerator OtherLegCorrection(Vector3 vec,string name)
    {
        int n = int.Parse(name.Split("_")[1]);
        Vector3 reversVec = legs[n].position - legsEnd[n].transform.position;
        Debug.Log(reversVec);
        Debug.Log(vec);

        //while (true)
        //{
        //    Debug.Log("[body] leg point move");
        //    for (int i = 0; i < legsEnd.Count; i++)
        //    {
        //        if (i == n)
        //        {
        //            legsEnd[i].transform.position = Vector3.MoveTowards(legsEnd[i].transform.position, legs[i].position, 0.1f);
        //        }
        //        else
        //        {
        //            legsEnd[i].transform.position = Vector3.MoveTowards(legsEnd[i].transform.position, legsEnd[i].transform.position+reversVec, 0.1f);
        //        }
        //
        //        yield return new WaitForSeconds(0.2f);
        //    }
        //    if (Vector3.Distance(legsEnd[legsEnd.Count - 1].transform.position, legs[legsEnd.Count - 1].position) <= 3.65f)
        //        break;
        //}
        yield return null;
    }
}
