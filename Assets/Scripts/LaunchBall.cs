using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchBall : MonoBehaviour
{
    public GameObject ball;
    public float angle;
    public float TimeScale;
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
            for (int i = 0; i < 10; i++)
            {
                Quaternion rot = Quaternion.Euler(angle - i * 15, 0, 0);
                Instantiate(ball, transform.position, rot);
            }
        }
    }
}
