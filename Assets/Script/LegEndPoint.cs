using System.Collections;
using UnityEngine;

public class LegEndPoint : MonoBehaviour
{
    [SerializeField]
    public Transform deathLeg;
    public Body body;
    public Transform legStart;
    public bool isDeath;

    private void Start()
    {
        isDeath = false;
        legStart = transform.parent.parent.parent.parent.parent;
        if (!legStart.name.Contains("Leg"))
            legStart = legStart.transform.parent;
        body.legsEnd.Add(this);
        body.legs.Add(legStart);
        //legStart.GetComponent<Rigidbody>().AddForce(transform.up);
    }


    public void EscapeLeg()
    {
        StartCoroutine(EscapeLeg_cou());
    }

    IEnumerator EscapeLeg_cou()
    {
        isDeath = true;
        body.RigidStopControl(true);
        body.RigidStopControl(false);
        legStart.parent = deathLeg;
        Destroy(legStart.GetComponent<FixedJoint>());

        Rigidbody rb = legStart.GetComponent<Rigidbody>();
        legStart.GetComponent<Collider>().isTrigger = false;
        rb.mass = 5;
        rb.useGravity = true;

        yield return new WaitForSecondsRealtime(1.5f);

        rb.isKinematic = true;
        legStart.GetComponent<Collider>().enabled = false;
        
    }
}
