using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchBall : MonoBehaviour
{
    public GameObject ball;
    public float angle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Quaternion rot = Quaternion.Euler(angle, 0, 0);
            Instantiate(ball, transform.position, rot);
        }
    }
}
