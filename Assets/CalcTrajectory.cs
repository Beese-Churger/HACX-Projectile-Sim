using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalcTrajectory : MonoBehaviour
{
    public List<Window> SelectedWindows;
    
    void CalculatePath()
    {
        SelectedWindows = MainGameManager.instance.GetWindows();


    }
}
