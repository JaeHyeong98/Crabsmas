using System.Collections;
using UnityEngine;

public class LegEndPoint : MonoBehaviour
{
    public Body body;

    private void Start()
    {
        body.legsEnd.Add(this);
    }

}
