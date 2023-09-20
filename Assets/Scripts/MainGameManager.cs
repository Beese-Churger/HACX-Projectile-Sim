using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    public static MainGameManager instance;

    [Header("Controllers")]
    public Animator CameraAnimator;
    public AnimationClip CameraToMainGameTransition;
    public AnimationClip CameraToSettingsTransition;
    public float TransitionSpeed;
    [Header("MainMenuElements")]
    public GameObject MainMenuUI;
    [Header("MainGameUI")]
    public GameObject MainGameUI;
    public GameObject SimButton;
    public GameObject RealButton;

    [Header("window")]
    public Color SelectHighlightColor = Color.green;
    public Color selectedColor = Color.red;
    public Color DeselectHighlightColor;
    public Material OriginalWindowMaterial;
    private Window currentlyHoveredWindow;
    private List<Window> SelectedWindows = new List<Window>();

    [Header("Positions")]
    public List<Vector3> CulpritPositions = new List<Vector3>();
    public List<GameObject> SpawnedCulprits = new List<GameObject>();

    [Header("Hits")]
    public List<HitBall> RegisteredHits = new List<HitBall>();
    
    public void ResetCulprits()
    {
        foreach(GameObject GO in SpawnedCulprits)
        {
            Destroy(GO);
        }
        SpawnedCulprits.Clear();
        CulpritPositions.Clear();
    }

    private void Start()
    {
        if (!instance)
        {
            instance = this;
        }
        else
            Destroy(this);

    }
    public void StartGame()
    {
        CameraAnimator.CrossFade(CameraToMainGameTransition.name, TransitionSpeed);
        MainMenuUI.SetActive(false);
        MainGameUI.SetActive(true);
    }

    public void TransitionToSettings()
    {
        CameraAnimator.CrossFade(CameraToSettingsTransition.name, TransitionSpeed);
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Window"))
            {
                currentlyHoveredWindow = hitObject.GetComponent<Window>();
                Renderer renderer = hitObject.GetComponent<Renderer>();
                if (!currentlyHoveredWindow.isSelected)
                {
                    renderer.material.color = SelectHighlightColor;
                }
                else if (currentlyHoveredWindow.isSelected)
                {
                    renderer.material.color = DeselectHighlightColor;
                }
            }
            else if (currentlyHoveredWindow)
            {
                if(!currentlyHoveredWindow.isSelected) ResetWindowColor(currentlyHoveredWindow.gameObject);
                currentlyHoveredWindow = null;
            }

        }

        if(Input.GetMouseButtonDown(0))
        {
            if(currentlyHoveredWindow && !currentlyHoveredWindow.isSelected && SelectedWindows.Count < 2)
            {
                currentlyHoveredWindow.isSelected = true;
                currentlyHoveredWindow.GetComponent<Renderer>().material.color = selectedColor;
                SelectedWindows.Add(currentlyHoveredWindow);
                if (SelectedWindows.Count == 2)
                {
                    SimButton.SetActive(true);
                    RealButton.SetActive(true);
                }
                return;
            }
            else if(currentlyHoveredWindow && currentlyHoveredWindow.isSelected)
            {
                currentlyHoveredWindow.isSelected = false;
                ResetWindowColor(currentlyHoveredWindow.gameObject);
                SelectedWindows.Remove(currentlyHoveredWindow);
                return;
            }
        }
      
    }
    public void StartRealTime()
    {
        MainGameUI.SetActive(false);
        gameObject.GetComponent<CalcTrajectory>().CalculatePath();
    }
    void ResetWindowColor(GameObject window)
    {
        Renderer renderer = window.GetComponent<Renderer>();
        renderer.material.color = OriginalWindowMaterial.color;
    }

    public List<Window> GetWindows()
    {
        return SelectedWindows;
    }

    public List<GameObject> GetCulprits()
    {
        return SpawnedCulprits;
    }

    public void AddNewHitRegistryToList(HitBall HB)
    {
        RegisteredHits.Add(HB);
        RemoveDuplicateHits();
    }

    public void RemoveAllCulpritsThatMissed()
    {
        foreach(GameObject C in SpawnedCulprits)
        {
            foreach(HitBall HB in RegisteredHits)
            {
                if (HB.RelatedHumanGameObject == C)
                {
                    continue;
                }
                else C.SetActive(false);
            }
        }
    }

    public void RemoveDuplicateHits()
    {
        for (int i = 0; i < RegisteredHits.Count; i++)
        {
            for (int j = i + 1; j < RegisteredHits.Count; j++)
            {
                if (RegisteredHits[i].RelatedHumanGameObject == RegisteredHits[j].RelatedHumanGameObject)
                {
                    if (RegisteredHits[i].WindowHit == 1 && RegisteredHits[j].WindowHit == 2)
                    {
                        RegisteredHits[i].canHitBoth = true;
                    }

                    RegisteredHits.RemoveAt(j);
                    j--; // Adjust the index after removal
                }
            }
        }
    }
}

[System.Serializable]
public class HitBall
{
    public GameObject RelatedHumanGameObject;
    public bool canHitBoth = false;
    public int WindowHit;
    public float DistanceFromCenter;
}
