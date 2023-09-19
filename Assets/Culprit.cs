using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Culprit : MonoBehaviour
{
    public Transform ShootPosition;
    public int currtarget = 1;
    public bool travelling = false;
    public bool below = true;
    public bool hit = false;

    public float launchAngleMax = 0f;
    public float launchAngleMin = -90f;
    public float angle;

    private void Awake()
    {
        //launchAngleMin = ShootPosition.eulerAngles.x;
        launchAngleMax = -90f;
        launchAngleMin = 0f;
    }
    public void Launch(int target)
    {
        currtarget = target;
        travelling = true;
        below = true;
        hit = false;
    }

    public void ResetMinMax()
    {
        launchAngleMax = -90f;
        launchAngleMin = 0;
    }
}
