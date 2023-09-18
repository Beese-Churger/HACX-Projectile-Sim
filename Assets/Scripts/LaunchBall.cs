using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchBall : MonoBehaviour
{
    public GameObject ball;
    public GameObject target;
    public float angle;
    public float TimeScale;
    public float launchAngleMin;
    public float launchAngleMax = -90f; // Maximum possible launch angle

    GameObject go;
    Quaternion targetRotation;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = TimeScale;


    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 dir = target.transform.position - transform.position;

            targetRotation = Quaternion.LookRotation(dir);

            angle = transform.rotation.eulerAngles.x;
            transform.rotation = targetRotation;

            go = Instantiate(ball, transform.position, transform.rotation);
            
        }

        if (!go)
            return;

        if (go.transform.position.z < target.transform.position.z)
        {
            if (go.transform.position.y > target.transform.position.y)
            {
                launchAngleMax = angle;
            }
            else
            {
                launchAngleMin = angle;
      
            }
            angle = launchAngleMin + launchAngleMax * 0.5f;
            Quaternion tiltRotation = Quaternion.Euler(angle, 0, 0);
            Quaternion finalRotation = targetRotation * tiltRotation;
            transform.rotation = finalRotation;
            go = Instantiate(ball, transform.position, transform.rotation);
        }
    }
}
