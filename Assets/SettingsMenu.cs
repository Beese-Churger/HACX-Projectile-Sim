using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public static SettingsMenu instance; 
    public Slider CalculationDensitySlider;
    public Slider DragCoefficientSlider;
    public Slider InitialVelocitySlider;
    public Slider SimulationSpeedSlider;
    public Slider MaxIterationsSlider;

    public List<AreaSplitManager> SplitManagers = new List<AreaSplitManager>();
    public MainGameManager MGM;

    public List<GameObject> Balls = new List<GameObject>();

    [Header("HelpMenu")]
    public Image HelpImage;
    public GameObject HelpHeaderDisplay;
    public TMP_Text HelpDescText, HelpTitleText;
    public bool HMIsOn = false;
    public bool AccIsOn = false;

    [Header("Windows")]
    public List<GameObject> SettingsWindows = new List<GameObject>();

    [Header("SliderValues")]
    public TMP_Text DragCoeffecientValueText, InitialVelocityValueText;
    public TMP_Text CalculationDensValueText, SimSpeedValueText, MaxIterationsValueText;

    private float drag = 0.47f;
    private int MaxIterations = 10;
    private int InitVel = 70;
    public Slider AccuracyLimitSlider;
    float LowestAccuracy = 100f, HighestAccuracy = 0f;

    public Camera freeCam;

    [Header("EditBuildingProperties")]
    public Slider BuildingFloorSlider;
    int currentFloor;
    public GameObject BuildingFloorPrefab;
    public Transform[] PlacesToAddBuildingTo;
    public List<GameObject> FloorsAdded = new List<GameObject>();
    public GameObject[] Roofs;
    public float OriginalRoofY;
    //bool flip = false;
    public void OnFloorSliderChange()
    {
        foreach (GameObject GO in FloorsAdded) Destroy(GO);
        FloorsAdded.Clear();
        int floorsToAdd = (int)BuildingFloorSlider.value - 7;
        foreach(Transform T in PlacesToAddBuildingTo)
        {
            for(int i = 0; i < floorsToAdd; i++)
            {
                GameObject GO = Instantiate(BuildingFloorPrefab);
                Vector3 Position = T.position;
                Position.y = (float)T.position.y + i * 3.6f;
                GO.transform.position = Position;
                FloorsAdded.Add(GO);
            }
        }

        foreach(GameObject GO in Roofs)
        {
            Vector3 vec = GO.transform.position;
            vec.y = OriginalRoofY + (floorsToAdd) * 3.6f;
            GO.transform.position = vec;
        }
    }

    public void CleanUp()
    {
        foreach(GameObject GO in Balls)
        {
            Destroy(GO);
        }
        Balls.Clear();
        freeCam.transform.position = new Vector3(0, 15, -20);
        freeCam.transform.rotation = Quaternion.identity;
    }
    public void OnChangeAccuracyLimitSlider()
    {
        foreach (GameObject B in MainGameManager.instance.SpawnedCulprits)
        {
            Culprit C = B.GetComponent<Culprit>();
            float accuracy = ScaleValue(float.Parse(C.AccuracyText.text), LowestAccuracy, HighestAccuracy);
            if (accuracy > AccuracyLimitSlider.value && C.hitWindow1 && C.hitWindow2) B.SetActive(true);
            else B.SetActive(false);
        }
    }

    public void ToggleBallTracers()
    {
        foreach(GameObject ball in Balls)
        {
            ball.SetActive(!ball.activeSelf);
        }
    }

    public void ToggleOffAllSettingsWindows()
    {
        foreach(GameObject GO in SettingsWindows)
        {
            GO.SetActive(false);
        }
    }

    void Start() 
    {
        if (!instance) 
        {
            instance = this; 
        } else {
            Destroy(this);
        }
        OriginalRoofY = Roofs[0].transform.position.y;
    }
    private void Awake()
    {
       
        GetSplitManagers();
        ChangeAllSliderValueTexts();
    }

    public void ChangeAllSliderValueTexts()
    {
        CalculationDensValueText.text = ((int)CalculationDensitySlider.value).ToString();
        InitialVelocityValueText.text = ((int)InitialVelocitySlider.value).ToString();
        DragCoeffecientValueText.text = DragCoefficientSlider.value.ToString("F2");
        SimSpeedValueText.text = SimulationSpeedSlider.value.ToString("F1");
        MaxIterationsValueText.text = ((int)MaxIterationsSlider.value).ToString();

    }


    public void DisplayHelpMenu(string title, string desc, Sprite Image)
    {
        if (Image)
        {
            HelpImage.sprite = Image;
            HelpImage.gameObject.SetActive(true);
        }
        else HelpImage.gameObject.SetActive(false);
        HelpDescText.text = desc;
        HelpHeaderDisplay.SetActive(true);
        HelpTitleText.text = title;
    }
    void GetSplitManagers()
    {
        SplitManagers.Clear();
        GameObject[] GOs = GameObject.FindGameObjectsWithTag("CulpritSpawnArea");
        foreach(GameObject G in GOs)
        {
            SplitManagers.Add(G.GetComponent<AreaSplitManager>());
        }
    }

    public void OnSplitManagerSliderChange()
    {
        GetSplitManagers();
        MGM.ResetCulprits();

        foreach(AreaSplitManager ASM in SplitManagers)
        {
            ASM.numberOfAreas = (int)CalculationDensitySlider.value;
            ASM.SpawnCulprits();
        }

        ChangeAllSliderValueTexts();
    }

    public void ToggleAccuracy()
    {
        if(AccIsOn)
        {
            foreach(HitBall B in MainGameManager.instance.RegisteredHits)
            {
                B.RelatedHumanGameObject.GetComponent<Culprit>().AccuracyText.gameObject.SetActive(false);
            }
            AccIsOn = false;
        }
        else if (!AccIsOn)
        {
            foreach (HitBall B in MainGameManager.instance.RegisteredHits)
            {
                B.RelatedHumanGameObject.GetComponent<Culprit>().AccuracyText.gameObject.SetActive(true);
            }
            AccIsOn = true;
        }
    }

    public void CalculateAccuracies()
    {
        foreach (HitBall A in MainGameManager.instance.RegisteredHitsOnBothWindows)
        {
            GameObject B = A.RelatedHumanGameObject;
            Culprit C = B.GetComponent<Culprit>();
            float Acc = float.Parse(C.AccuracyText.text);
            if (LowestAccuracy > Acc && Acc != 0)
            {
                LowestAccuracy = Acc;
            }
            if (HighestAccuracy < Acc && Acc != 0)
            {
                HighestAccuracy = Acc;
            }
        }
    }

    public void ToggleHM()
    {
        if (HMIsOn)
        {
            foreach (HitBall B in MainGameManager.instance.RegisteredHits)
            {
                B.RelatedHumanGameObject.transform.Find("Model").GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
            }
            HMIsOn = false;
        }
        else
        {
          
            foreach (HitBall A in MainGameManager.instance.RegisteredHitsOnBothWindows)
            {
                GameObject B = A.RelatedHumanGameObject;
                Culprit C = B.GetComponent<Culprit>();
                float accuracy = ScaleValue(float.Parse(C.AccuracyText.text), LowestAccuracy, HighestAccuracy);
                // Lerp between green and red based on accuracy
                Color targetColor = Color.Lerp(Color.red, Color.green, accuracy);
                // Assign the target color to the material
                C.transform.Find("Model").GetComponent<SkinnedMeshRenderer>().material.color = targetColor;
            }
            HMIsOn = true;
        }
    }
    float ScaleValue(float initialValue, float minValue, float maxValue)
    {
        return Mathf.Clamp01((initialValue - minValue) / (maxValue - minValue));
    }
    public void OnDragChange()
    {
        drag = DragCoefficientSlider.value;
        ChangeAllSliderValueTexts();
    }

    public void OnSpeedChange()
    {
        Time.timeScale = SimulationSpeedSlider.value;
        ChangeAllSliderValueTexts();
    }
    public void OnMaxIterationsChange()
    {
        MaxIterations = (int)MaxIterationsSlider.value;
        ChangeAllSliderValueTexts();
    }
    public void OnInitVelChange()
    {
        InitVel = (int)InitialVelocitySlider.value;
        ChangeAllSliderValueTexts();
    }
    public int GetMaxIterations()
    {
        return MaxIterations;
    }
    public float GetDragCoefficient() 
    {
        return  drag;
    }

    public int GetInitVel()
    {
        return InitVel;
    }
}
