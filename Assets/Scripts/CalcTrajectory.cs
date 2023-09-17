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
    public void CalculatePath()
    {
        FindViableCulprits();

        LaunchBalls(ViableCulprits1, 0, 1);
        LaunchBalls(ViableCulprits2, 1, 1);
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
                if (hit.collider.tag != "Window")
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
                if (hit.collider.tag != "Window")
                    continue;
                ViableCulprits2.Add(Culprits[i]);
            }
        }
    }

    void LaunchBalls(List<GameObject> ViableCulprits, int window, int increase)
    {
        for (int i = 0; i < ViableCulprits1.Count; ++i)
        {
            Culprit currCulprit = ViableCulprits[i].GetComponent<Culprit>();
            currCulprit.ShootPosition.LookAt(SelectedWindows[window].transform);
            Quaternion rot = Quaternion.Euler(currCulprit.ShootPosition.rotation.x + increase, currCulprit.ShootPosition.rotation.y, currCulprit.ShootPosition.rotation.z);
            Instantiate(BallPrefab, currCulprit.ShootPosition.position, currCulprit.ShootPosition.rotation);
        }
    }
}
