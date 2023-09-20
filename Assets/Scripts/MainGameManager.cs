using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class MainGameManager : MonoBehaviour
{
    public static MainGameManager instance;

    [Header("Controllers")]
    public Animator CameraAnimator;
    public AnimationClip CameraToMainGameTransition;
    public AnimationClip CameraToSettingsTransition;
    public AnimationClip CameraToOverviewTransition;
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
    public List<HitBall> RegisteredHitsOnBothWindows = new List<HitBall>();

    [Header("UI Related")]
    public Button ChangeWindowDisplay;
    public int WindowDisplayOption = 2;
    public TMP_Text DisplayingCulpritText;
    public Toggle ToggleHeatMap, ToggleAccuracy;
    public GameObject PostResultsUIGO;
    
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

    public void SwapToCamOverview()
    {
        CameraAnimator.CrossFade(CameraToOverviewTransition.name, TransitionSpeed);
        PostResultsUIGO.SetActive(true);
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
        
    }

    public void RemoveAllCulprits()
    {
        foreach(GameObject C in SpawnedCulprits)
        {
            C.SetActive(false);
        }
    }

    public void ToggleCulpritDisplays()
    {
        WindowDisplayOption++;
        if (WindowDisplayOption > 2) WindowDisplayOption = 0;
        RemoveAllCulprits();
        if (WindowDisplayOption == 0)
        {
            ToggleFirstWindowHavers();
            DisplayingCulpritText.text = "Display: Window 1 Only";
        }
        else if(WindowDisplayOption == 1)
        {
            ToggleSecondWindowHavers();
            DisplayingCulpritText.text = "Display: Window 2 Only";
        }
        else if(WindowDisplayOption == 2)
        {
            ToggleBothWindowHavers();
            DisplayingCulpritText.text = "Display: Both Windows";
        }
    }
    
    public void ToggleFirstWindowHavers()
    {
        foreach (HitBall B in RegisteredHits)
        {
            if (B.WindowHit == 0)
            {
                bool k = false;
                foreach(HitBall b in RegisteredHitsOnBothWindows)
                {
                    if (b.RelatedHumanGameObject == B.RelatedHumanGameObject)
                    {
                        k = true;
                        break;
                    }
                }
                if(!k)
                    B.RelatedHumanGameObject.SetActive(true);
            }
        }
    }

    public void ToggleBothWindowHavers()
    {
        foreach (HitBall B in RegisteredHitsOnBothWindows)
        {
            if (B.canHitBoth)
            {
                B.RelatedHumanGameObject.SetActive(true);
            }
        }
    }

    public void ToggleSecondWindowHavers()
    {

        foreach (HitBall B in RegisteredHits)
        {
            if (B.WindowHit == 1)
            {
                bool k = false;
                foreach (HitBall b in RegisteredHitsOnBothWindows)
                {
                    if (b.RelatedHumanGameObject == B.RelatedHumanGameObject)
                    {
                        k = true;
                        break;
                    }
                }
                if (!k)
                    B.RelatedHumanGameObject.SetActive(true);
            }
        }
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

    public void SortDuplicateHits()
    {
        for (int i = 0; i < RegisteredHits.Count; i++)
        {
            for (int j = i + 1; j < RegisteredHits.Count; j++)
            {
                if (RegisteredHits[i].RelatedHumanGameObject.name == RegisteredHits[j].RelatedHumanGameObject.name)
                {
                    if (RegisteredHits[i].WindowHit == 0 && RegisteredHits[j].WindowHit == 1 || RegisteredHits[i].WindowHit == 1 && RegisteredHits[j].WindowHit == 0)
                    {
                        HitBall HB = new HitBall();
                        HB.RelatedHumanGameObject = RegisteredHits[i].RelatedHumanGameObject;
                        HB.WindowHit = 2;
                        HB.canHitBoth = true;
                        HB.DistanceFromCenter = 0;
                        HB.CalculateAccuracy();
                        RegisteredHitsOnBothWindows.Add(HB);
                    }
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
    public float Accuracy;
    public Vector3 Hitposition;
    public void CalculateAccuracy()
    {
        Accuracy = DistanceFromCenter / 0.5f;
    }
}
