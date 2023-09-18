using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalcTrajectory : MonoBehaviour
{
    public List<Window> SelectedWindows;
    private List<GameObject> Culprits;

    public List<GameObject> ViableCulprits1;
    public List<GameObject> ViableCulprits2;
    public List<GameObject> Balls;
    public GameObject BallPrefab;

    bool launched = false;
    public void CalculatePath()
    {
        FindViableCulprits();

        LaunchBalls(ViableCulprits1, 0);
        //LaunchBalls(ViableCulprits2, 1);
        launched = true;
    }

    private void Update()
    {
        if (!launched)
            return;

        for(int i = 0; i < ViableCulprits1.Count; ++i)
        {
            Culprit curr = ViableCulprits1[i].GetComponent<Culprit>();
            if (curr.hit || curr.travelling)
                continue;

            if (!curr.travelling && !curr.hit)
            {
                Vector3 dir = SelectedWindows[0].transform.position - curr.ShootPosition.position;
                Quaternion targetRotation = Quaternion.LookRotation(dir);

            

                if (!curr.below)
                {
                    curr.launchAngleMax = curr.angle;
                }
                else
                {
                    curr.launchAngleMin = curr.angle;
                }


                curr.angle = curr.launchAngleMin + curr.launchAngleMax * 0.5f;
                Quaternion tiltRotation = Quaternion.Euler(curr.angle, 0, 0);
                Quaternion finalRotation = targetRotation * tiltRotation;
                curr.ShootPosition.localRotation = finalRotation;
                Instantiate(BallPrefab, curr.ShootPosition.position, curr.ShootPosition.rotation, curr.ShootPosition.root);
                curr.travelling = true;
                curr.hit = false;
                curr.currtarget = 1;
            }
        }
    }
    void FindViableCulprits()
    {
        SelectedWindows = MainGameManager.instance.GetWindows();
        Culprits = MainGameManager.instance.GetCulprits();

        for (int i = 0; i < Culprits.Count; ++i)
        {
            Culprit currCulprit = Culprits[i].GetComponent<Culprit>();

            RaycastHit hit;

            Vector3 rayDir = SelectedWindows[0].transform.position - currCulprit.ShootPosition.position;

            if (Physics.Raycast(currCulprit.ShootPosition.position, rayDir, out hit))
            {
                if (hit.collider.tag != "Window" && hit.collider.tag != "Hitzone")
                    continue;
                ViableCulprits1.Add(Culprits[i]);
            }
        }
        for (int i = 0; i < Culprits.Count; ++i)
        {
            Culprit currCulprit = Culprits[i].GetComponent<Culprit>();

            RaycastHit hit;

            Vector3 rayDir = SelectedWindows[1].transform.position - currCulprit.ShootPosition.position;

            if (Physics.Raycast(currCulprit.ShootPosition.position, rayDir, out hit))
            {
                if (hit.collider.tag != "Window" && hit.collider.tag != "Hitzone")
                    continue;
                ViableCulprits2.Add(Culprits[i]);
            }
        }
    }

    void LaunchBalls(List<GameObject> ViableCulprits, int window)
    {
        for (int i = 0; i < ViableCulprits.Count; ++i)
        {
            Culprit currCulprit = ViableCulprits[i].GetComponent<Culprit>();

            Vector3 dir = SelectedWindows[window].transform.position - currCulprit.ShootPosition.position;
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            currCulprit.angle = currCulprit.ShootPosition.eulerAngles.x;
            currCulprit.ShootPosition.rotation = targetRotation;

            currCulprit.travelling = true;
            currCulprit.hit = false;
            currCulprit.currtarget = window;

            Instantiate(BallPrefab, currCulprit.ShootPosition.position, currCulprit.ShootPosition.rotation, currCulprit.ShootPosition.root);
        }
    }
}
