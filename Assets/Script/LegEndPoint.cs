using System.Collections;
using UnityEngine;

public class LegEndPoint : MonoBehaviour
{
    public Body body;
    public float springForce = 100f;
    public float damper = 5f;
    private SpringJoint spring;


    private void Start()
    {
        spring = transform.gameObject.AddComponent<SpringJoint>();
        spring.connectedBody = body.crabBody;
        spring.spring = springForce;
        spring.damper = damper;
        spring.autoConfigureConnectedAnchor = false;
        spring.connectedAnchor = body.crabBody.position;
    }

}
