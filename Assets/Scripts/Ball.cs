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

    private List<Window> targets;
    void Awake()
    {
        targets = MainGameManager.instance.GetWindows();
        rbody = GetComponent<Rigidbody>();
        distToGround = GetComponent<SphereCollider>().bounds.extents.y;

        dragCoefficient = 0.1f;
            // get drag coefficient 

        r = distToGround;
        volume = (4 * Mathf.PI * r * r * r) / 3;
        rbody.mass = density * volume; // in grams

        area = 2 * Mathf.PI * r * r;
        force = rbody.mass * 70;

        // Assuming that 'transform.forward' represents the direction the ball is facing.
        Vector3 facingDirection = transform.forward;

        // Calculate the angle between the forward vector and the upward direction.
        float angle = Mathf.Atan2(facingDirection.y, facingDirection.z) * Mathf.Rad2Deg;

        //float yComponent = Mathf.Cos(angle * Mathf.Deg2Rad) * force;
        //float zComponent = Mathf.Sin(angle * Mathf.Deg2Rad) * force;
        //Vector3 forceApplied = new Vector3(0, yComponent, zComponent);

        rbody.AddForce(facingDirection * force, ForceMode.Impulse);

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
           // Debug.Log(rbody.velocity.magnitude);
            SimulateInRealTime(Time.deltaTime);
        }

    }

    bool IsGrounded()
    {
        return (transform.position.y < 0);
    }

    void SimulateInRealTime(float dt)
    {
        Vector3 direction = -rbody.velocity.normalized;
        float velocity = rbody.velocity.magnitude;
        var forceAmount = (p * velocity * velocity * dragCoefficient * area) * 0.5f;
       // Debug.Log("drag: " + forceAmount);
        rbody.AddForce(direction * forceAmount);
    }

    public void Simulate()
    {

    }
    
    public void setDragCoefficient(float newValue) 
    {
        dragCoefficient = newValue; 
    }

    private void OnTriggerEnter(Collider other)
    {
        Culprit shooter = transform.root.GetComponent<Culprit>();
        if (other.tag != "Window")
        {
            shooter.travelling = false;
            shooter.hit = false;
            switch(shooter.currtarget)
            {
                case 1:
                    {
                        if (transform.position.y < targets[0].transform.position.y)
                            shooter.below = true;
                        else
                            shooter.below = false;
                        break;
                    }
                case 2:
                    {
                        if (transform.position.y < targets[1].transform.position.y)
                            shooter.below = true;
                        else
                            shooter.below = false;
                        break;
                    }
                default:
                    break;
            }
            Destroy(gameObject);
        }
        else
        {
            shooter.hit = true;
            shooter.travelling = false;
            rbody.isKinematic = true;
            rbody.velocity = Vector3.zero;
            Debug.Log("hi");
            //Destroy(gameObject);
        }
    }
}
