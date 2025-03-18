using System.Collections;
using Main;
using UnityEngine;

public class LegEndPoint : MonoBehaviour
{
    public Body body;
    public Transform legStart;
    public bool isDeath;


    private void OnEnable()
    {
        StartCoroutine(Init());
        //legStart.GetComponent<Rigidbody>().AddForce(transform.up);
    }

    public IEnumerator Init()
    {
        yield return new WaitUntil(() => GSC.playerController != null && GSC.playerController.player != null);
        isDeath = false;
        legStart = transform.parent.parent.parent.parent.parent;
        if (!legStart.name.Contains("Leg"))
            legStart = legStart.transform.parent;

        for (int i = 0; i < 4; i++)
        {
            if (legStart.name.Contains(i.ToString()))
            {
                body.legsEnd[i] = this;
                body.legs[i] = legStart;
                break;
            }
        }
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
        legStart.parent = GSC.main.deathLegs;
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
