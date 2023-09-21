using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalcTrajectory : MonoBehaviour
{
    public List<Window> SelectedWindows;
    private List<GameObject> Culprits;

    public List<GameObject> ViableCulprits1;
    public List<GameObject> ViableCulprits2;
    public GameObject BallPrefab;

    bool launch1 = false;
    bool launch2 = false;

    public SettingsMenu SM;

    public void CalculatePath()
    {
        FindViableCulprits();
        LaunchBalls(ViableCulprits1, 1);
        launch1 = true;
        launch2 = false;
      
    }

    public IEnumerator DelayedSortingOfCulprits()
    {
        yield return new WaitForSeconds(1);
        MainGameManager.instance.SwapToCamOverview();
        Debug.Log("SortedList!");
        MainGameManager.instance.SortDuplicateHits();
        MainGameManager.instance.ToggleBothWindowHavers();
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach(GameObject ball in balls)
        {
            SM.Balls.Add(ball);
        }
    }
    private void Update()
    {

        if (launch1)
        {
            if (!CheckIsTravelling(ViableCulprits1))
            {
                for (int i = 0; i < ViableCulprits1.Count; ++i)
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
                        curr.Launch(1);
                        curr.ResetMinMax();
                        GameObject go = Instantiate(BallPrefab, curr.ShootPosition.position, curr.ShootPosition.rotation, curr.ShootPosition.root);
                        go.GetComponent<Ball>().SetTarget(0);
                    }
                }
            }
        }
        if (CheckIsWindowDone(ViableCulprits1) && !launch2 && launch1)
        {

            LaunchBalls(ViableCulprits2, 2);
            launch2 = true;
            launch1 = false;
        }
        if (launch2)
        {
            if (!CheckIsTravelling(ViableCulprits1))
            {
                for (int i = 0; i < ViableCulprits2.Count; ++i)
                {
                    Culprit curr = ViableCulprits2[i].GetComponent<Culprit>();
                    if (curr.hit || curr.travelling)
                        continue;

                    if (!curr.travelling && !curr.hit)
                    {
                        Vector3 dir = SelectedWindows[1].transform.position - curr.ShootPosition.position;
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
                        curr.Launch(2);
                        curr.ResetMinMax();
                        GameObject go = Instantiate(BallPrefab, curr.ShootPosition.position, curr.ShootPosition.rotation, curr.ShootPosition.root);
                        go.GetComponent<Ball>().SetTarget(1);
                    }
                }
            }
        }

        if (CheckIsWindowDone(ViableCulprits2) && launch2 && !launch1)
        {
            launch2 = false;
            StartCoroutine(DelayedSortingOfCulprits());
        }
    }
    bool CheckIsWindowDone(List<GameObject> culprits)
    {
        for (int i = 0; i < culprits.Count; ++i)
        {
            Culprit curr = culprits[i].GetComponent<Culprit>();
            if (!curr.hit)
                return false;
        }
        return true;
    }

    bool CheckIsTravelling(List<GameObject> culprits)
    {
        for (int i = 0; i < culprits.Count; ++i)
        {
            Culprit curr = culprits[i].GetComponent<Culprit>();
            if (curr.travelling)
                return true;
        }
        return false;
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
            //currCulprit.Reset(window);
            Vector3 dir = SelectedWindows[window - 1].transform.position - currCulprit.ShootPosition.position;
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            currCulprit.angle = currCulprit.ShootPosition.eulerAngles.x;
            currCulprit.ShootPosition.rotation = targetRotation;

            currCulprit.Launch(window);
            currCulprit.ResetMinMax();
            GameObject go = Instantiate(BallPrefab, currCulprit.ShootPosition.position, currCulprit.ShootPosition.rotation, currCulprit.ShootPosition.root);
            go.GetComponent<Ball>().SetTarget(window - 1);
        }
    }

    void CalcCompleted()
    {

    }
}
