using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public static SettingsMenu instance; 
    public Slider CalculationDensitySlider;
    public Slider DragCoefficientSlider; 
    public Slider SimulationSpeedSlider; 

    public List<AreaSplitManager> SplitManagers = new List<AreaSplitManager>();
    public MainGameManager MGM;
    public GameObject HelpHeaderDisplay;
    public TMP_Text HelpText;

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
    }

    public void DisplayHelp(string s)
    {
        HelpHeaderDisplay.SetActive(true);
        HelpText.text = s;
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
    }
    public float GetDragCoefficientSlider() 
    {
        return DragCoefficientSlider.value;
    }
    public float GetSimulationSpeed()
    {
        return SimulationSpeedSlider.value;
    }
}
