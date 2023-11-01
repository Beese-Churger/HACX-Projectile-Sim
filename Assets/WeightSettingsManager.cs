using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeightSettingsManager : MonoBehaviour
{
    public TMP_Text CurrentValueText;
    public TMP_Text VOIWeightText, AOIWeightText;
    public Slider weightSlider;
    public SettingsMenu SM;
    public float VOIWeight, AOIWeight;
    public static WeightSettingsManager instance;
    public void Start()
    {
        VOIWeight = 1;
        AOIWeight = 1;
        CurrentValueText.text = weightSlider.value.ToString("F2");

        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void ResetSlider()
    {
        weightSlider.value = 1;
        OnBiasSliderChange();
    }



    public void OnBiasSliderChange()
    {
        VOIWeight = weightSlider.value;
        AOIWeight = weightSlider.maxValue - weightSlider.value;

        CurrentValueText.text = weightSlider.value.ToString("F2");
        VOIWeightText.text = "VOI Weight: " + VOIWeight.ToString("F2");
        AOIWeightText.text = "AOI Weight: " + AOIWeight.ToString("F2");
        SM.ReaccurateAndReHM();
    }
}
