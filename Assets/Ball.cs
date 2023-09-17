using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody rbody;

    public bool islaunched;

    public float density = 7750f; //kg/m^3
    public float dragCoefficient = 0.1f;
    public float volume = 0;
    public float initialVel = 70f; // m/s
    private float force;
    float distToGround;
    public float r;
    float p = 1.225f; //density of air 1.225kg/m^3
    public float area;
    // Start is called before the first frame update
    void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        distToGround = GetComponent<SphereCollider>().bounds.extents.y;
        r = distToGround * 0.5f;
        volume = (4 * Mathf.PI * r * r * r) / 3;
        rbody.mass = density * volume; // in grams

        area = 4 * Mathf.PI * r * r;
        force = rbody.mass * 70;
        float angle = 45f;
        angle *= Mathf.Deg2Rad;
        float yComponent = Mathf.Cos(angle) * force;
        float zComponent = Mathf.Sin(angle) * force;
        Vector3 forceApplied = new Vector3(0, yComponent, zComponent);
        rbody.AddForce(forceApplied, ForceMode.Impulse);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(IsGrounded())
        {
            GetComponent<SphereCollider>().isTrigger = true;
            rbody.velocity = Vector3.zero;
            rbody.angularVelocity = Vector3.zero;
            rbody.useGravity = false;
        }
        else
        {
            Debug.Log(rbody.velocity.magnitude);
            SimulateInRealTime(Time.fixedDeltaTime);
        }

    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.01f);
    }

    void SimulateInRealTime(float dt)
    {
        Vector3 direction = -rbody.velocity.normalized;
        float velocity = rbody.velocity.magnitude;
        var forceAmount = (p * velocity * velocity * dragCoefficient * area) * 0.5f;
        Debug.Log("drag: " + forceAmount);
        rbody.AddForce(direction * forceAmount);
    }

    public void Simulate()
    {

    }
}