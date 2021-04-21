using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyGravity : MonoBehaviour {

    //credit some: podperson

    public Transform planet;
    public bool AlignToPlanet;
    private float gravityConstant = 9.8f;
    private Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 toCenter = planet.position - transform.position;
        toCenter.Normalize();

        rigidbody.AddForce(toCenter * gravityConstant, ForceMode.Acceleration);

        if (AlignToPlanet)
        {
            Quaternion q = Quaternion.FromToRotation(transform.up, -toCenter);
            q = q * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, q, 1);
        }
    }
}
