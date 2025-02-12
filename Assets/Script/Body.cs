using System.Collections;
using UnityEngine;

public class Body : MonoBehaviour
{
    private Transform parent;

    private void Awake()
    {
        parent = transform.parent.transform;
    }
    public void MoveBody(Vector3 vec)
    {
         // 다리가 이동한 거리의 70%
        //transform.position = vec;

        StartCoroutine(BodyCorrection(vec));
    }

    IEnumerator BodyCorrection(Vector3 vec)
    {
        Vector3 pNextPos = parent.transform.position + vec;
        Debug.Log("[body] " + parent.transform.position);
        Debug.Log("[body] " + pNextPos);

        for (int i = 0; i < 10; i++) 
        {
            parent.transform.position = Vector3.MoveTowards(parent.transform.position, pNextPos, 0.1f);
            yield return new WaitForSeconds(0.2f);

            Debug.Log("[body] "+parent.transform.position);
        }
        
        yield return null;
    }
}
