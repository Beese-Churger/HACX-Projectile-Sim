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
    public TMP_Text DragCoeffecientValueText;
    public TMP_Text CalculationDensValueText, SimSpeedValueText, MaxIterationsValueText;

    private float drag = 0.47f;
    private int MaxIterations = 10;

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
    }
    private void Awake()
    {
       
        GetSplitManagers();
        ChangeAllSliderValueTexts();
    }

    public void ChangeAllSliderValueTexts()
    {
        CalculationDensValueText.text = ((int)CalculationDensitySlider.value).ToString();
        DragCoeffecientValueText.text = DragCoefficientSlider.value.ToString("F2");
        SimSpeedValueText.text = SimulationSpeedSlider.value.ToString("F1");
        MaxIterationsValueText.text = (MaxIterationsSlider.value).ToString("F1");
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
            foreach (HitBall B in MainGameManager.instance.RegisteredHits)
            {
                float accuracy = B.Accuracy;
                // Make sure accuracy is clamped between 0 and 1
                accuracy = Mathf.Clamp01(accuracy);
                // Lerp between green and red based on accuracy
                Color targetColor = Color.Lerp(Color.green, Color.red, accuracy * 1.5f);
                // Assign the target color to the material
                B.RelatedHumanGameObject.transform.Find("Model").GetComponent<SkinnedMeshRenderer>().material.color = targetColor;
            }
            HMIsOn = true;
        }
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
    public int GetMaxIterations()
    {
        return MaxIterations;
    }
    public float GetDragCoefficient() 
    {
        return  drag;
    }
}
