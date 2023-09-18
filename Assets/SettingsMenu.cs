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
    public Slider AngleIncrementSlider;

    public List<AreaSplitManager> SplitManagers = new List<AreaSplitManager>();
    public MainGameManager MGM;

    [Header("HelpMenu")]
    public Image HelpImage;
    public GameObject HelpHeaderDisplay;
    public TMP_Text HelpDescText, HelpTitleText;

    [Header("Windows")]
    public List<GameObject> SettingsWindows = new List<GameObject>();

    [Header("SliderValues")]
    public TMP_Text DragCoeffecientValueText;
    public TMP_Text CalculationDensValueText, SimSpeedValueText, AngIncValueText;

    private float drag = 0.47f;
    private int AngleIncrement = 5;

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
        SimSpeedValueText.text = SimulationSpeedSlider.value.ToString();
        AngIncValueText.text = (AngleIncrementSlider.value).ToString();
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
    public void OnAngleIncrementChange()
    {
        AngleIncrement = (int)AngleIncrementSlider.value;
        ChangeAllSliderValueTexts();
    }
    public int GetAngleIncrement()
    {
        return AngleIncrement;
    }
    public float GetDragCoefficient() 
    {
        return  drag;
    }
}
