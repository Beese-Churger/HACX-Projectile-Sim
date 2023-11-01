using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BuildingPresets : MonoBehaviour
{
    public string Block1Name, Block2Name;
    public TMP_Text Block1NameText, Block2NameText;

    public TMP_Text loadingTip;
    public Slider loadslider;
    float speed;
    public List<textclass> textClasses = new List<textclass>();
    public GameObject confirm;
    public GameObject SliderMenu, MapMenu;
    public IEnumerator SliderTextChange()
    {
        foreach(textclass tc in textClasses)
        {
            loadingTip.text = tc.text;
            speed = tc.newSpeed;
            yield return new WaitForSeconds(tc.lastfor);
        }
    }

    public IEnumerator SliderValueChange()
    {
        while (loadslider.value != loadslider.maxValue)
        {
            loadslider.value += Time.deltaTime * speed;
            yield return null;
        }
        SliderMenu.SetActive(false);
        confirm.SetActive(true);
        StopAllCoroutines();
    }

    public void ChangeBlock1NameText()
    {
        Block1NameText.text = "Building 1: Blk " + Block1Name;
    }

    public void ChangeBlock2Nametext()
    {
        Block2NameText.text = "Building 2: Blk " + Block2Name;
    }

    public void Confirm()
    {
        StartCoroutine(SliderTextChange());
        StartCoroutine(SliderValueChange());
        SliderMenu.SetActive(true);
        MapMenu.SetActive(false);
    }
}

[System.Serializable]
public class textclass
{
    public float lastfor;
    public string text;
    public float newSpeed;
}
