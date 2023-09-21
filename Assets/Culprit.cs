using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Culprit : MonoBehaviour
{
    public Transform ShootPosition;
    public int currTarget = 1;
    public bool travelling = false;
    public bool below = true;
    public bool hit = false;

    public bool hitWindow1 = false;
    public bool hitWindow2 = false;
    public bool done = false;
    public bool canShoot = false;

    public float launchAngleMax = 0f;
    public float launchAngleMin = -90f;
    public float angle;

    public int iterations1;
    public int iterations2;

    public TMP_Text AccuracyText;

    private List<Window> targets;
    Quaternion targetRotation;
    GameObject go;
    public GameObject Ball;
    private void Awake()
    {
        //launchAngleMin = ShootPosition.eulerAngles.x;
        launchAngleMax = -90f;
        launchAngleMin = 0f;

    }
    public void Launch(int target)
    {
        currTarget = target;
        travelling = true;
        below = true;
        hit = false;
    }

    public void ResetMinMax()
    {
        launchAngleMax = -90f;
        launchAngleMin = 0;
    }

    private void Update()
    {
        if (done || !canShoot)
            return;

        if (currTarget == 0)
        {
            if (iterations1 >= 10)
                return;

            if (hitWindow1)
                return;
        }
        else if (currTarget == 1)
        {
            if (iterations2 >= 10)
            {
                done = true;
                return;
            }
            if (hitWindow2)
            {
                done = true;
                return;
            }
        }

        if (!go)
            return;

        if (go.GetComponent<Rigidbody>().isKinematic)
        {
            if (go.transform.position.y > targets[currTarget].transform.position.y)
            {
                launchAngleMax = angle;
            }
            else
            {
                launchAngleMin = angle;
            }
            Destroy(go);
            angle = launchAngleMin + launchAngleMax * 0.5f;
            Quaternion tiltRotation = Quaternion.Euler(angle, 0, 0);
            Quaternion finalRotation = targetRotation * tiltRotation;
            ShootPosition.rotation = finalRotation;
            go = Instantiate(Ball, ShootPosition.position, ShootPosition.rotation, ShootPosition.root);
            go.GetComponent<Ball>().SetTarget(currTarget);
            travelling = true;
            canShoot = false;

            switch (currTarget)
            {
                case 0:
                    iterations1++;
                    break;
                case 1:
                    iterations2++;
                    break;
                default:
                    break;
            }
        }
    }

    public void FireProjectileAt(int window)
    {
        targets = MainGameManager.instance.GetWindows();
        iterations1 = 0;
        iterations2 = 0;
        launchAngleMax = -90f;
        launchAngleMin = 0;
        currTarget = window;

        Vector3 dir = targets[currTarget].transform.position - ShootPosition.position;

        targetRotation = Quaternion.LookRotation(dir);

        angle = transform.rotation.eulerAngles.x;
        ShootPosition.rotation = targetRotation;

        go = Instantiate(Ball, ShootPosition.position, ShootPosition.rotation, ShootPosition.root);
        go.GetComponent<Ball>().SetTarget(currTarget);
        travelling = true;
        canShoot = false;
    }
}
