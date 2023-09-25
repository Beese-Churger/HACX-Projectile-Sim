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

    int maxIteration = 10;

    public SettingsMenu SM;

    public void CalculatePath()
    {
        FindViableCulprits();
        LaunchBalls(ViableCulprits1, 0);
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
        foreach (GameObject ball in balls)
        {
            SM.Balls.Add(ball);
        }
    }
    private void Update()
    {

        if (launch1)
        {
            if (!CheckIsTravelling(ViableCulprits1))
                SetCanShoot(ViableCulprits1);
        }

        if (CheckIsWindowDone(ViableCulprits1, 0) && !launch2 && launch1)
        {
            LaunchBalls(ViableCulprits2, 1);
            launch2 = true;
            launch1 = false;
        }

        if (launch2)
        {
            if (!CheckIsTravelling(ViableCulprits2))
                SetCanShoot(ViableCulprits2);
        }

        if (CheckIsWindowDone(ViableCulprits2, 1) && launch2 && !launch1)
        {
            launch2 = false;
            StartCoroutine(DelayedSortingOfCulprits());
        }
    }
    bool CheckIsWindowDone(List<GameObject> culprits, int window)
    {
        for (int i = 0; i < culprits.Count; ++i)
        {
            Culprit curr = culprits[i].GetComponent<Culprit>();

            if(window == 0)
            {
                if(curr.iterations1 < curr.maxIterations)
                {
                    if (!curr.hitWindow1)
                        return false;
                }
            }

            if (window == 1)
            {
                if (curr.iterations2 < curr.maxIterations)
                {
                    if (!curr.hitWindow2)
                        return false;
                }
            }
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

    void SetCanShoot(List<GameObject> culprits)
    {
        for (int i = 0; i < culprits.Count; ++i)
        {
            Culprit curr = culprits[i].GetComponent<Culprit>();
            curr.canShoot = true;
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
            currCulprit.FireProjectileAt(window);
        }
    }
}
