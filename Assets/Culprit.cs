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

    public float angle1;
    public float angle2;

    public int iterations1;
    public int iterations2;
    public int maxIterations;

    public TMP_Text AccuracyText;

    private List<Window> targets = new List<Window>();
    Quaternion targetRotation;
    GameObject go;
    public GameObject Ball;

    public void Cleanup()
    {
        done = false;
        hitWindow1 = false; hitWindow2 = false;
        iterations1 = 0; iterations2 = 0;
        AccuracyText.gameObject.SetActive(false);
        travelling = false;
        hit = false;
        below = true;
        targets.Clear();
    }
    private void Awake()
    {
        iterations1 = 0;
        iterations2 = 0;
        maxIterations = 10;
    }

    private void Update()
    {
        if (done || !canShoot)
            return;

        if (currTarget == 0)
        {
            if (iterations1 >= maxIterations)
                return;

            if (hitWindow1)
                return;
        }
        else if (currTarget == 1)
        {
            if (iterations2 >= maxIterations)
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
            else if(go.transform.position.y < targets[currTarget].transform.position.y)
            {
                launchAngleMin = angle;
            }
          
            angle = (launchAngleMin + launchAngleMax) * 0.5f;
            //Debug.Log(angle + " " + launchAngleMin + " " + launchAngleMax);
            Quaternion tiltRotation = Quaternion.Euler(angle, 0, 0);
            Quaternion finalRotation = targetRotation * tiltRotation;
            ShootPosition.rotation = finalRotation;
            Destroy(go);
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
        maxIterations = SettingsMenu.instance.GetMaxIterations();

        targets = MainGameManager.instance.GetWindows();

        launchAngleMax = -90f;
 
        currTarget = window;

        Vector3 dir = targets[currTarget].transform.position - ShootPosition.position;

        targetRotation = Quaternion.LookRotation(dir);

        angle = transform.rotation.eulerAngles.x;
        launchAngleMin = angle;
        ShootPosition.rotation = targetRotation;

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
