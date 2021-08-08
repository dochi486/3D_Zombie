using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    //Ambient Color 환경광 영향을 받기 때문에 ~~ 
    public Color dayColor;
    public Color nightColor;


    [ContextMenu("SetDayLight")]
    void SetDayLight()
    {
        var allLight = FindObjectsOfType<Light>();
        foreach (var item in allLight)
        {
            if (item.type == LightType.Directional)
                item.enabled = true;
            else
                item.enabled = false;
        }
        RenderSettings.ambientLight = dayColor;
    }

    [ContextMenu("SetNightLight")]
    void SetNightLight()
    {
        var allLight = FindObjectsOfType<Light>();
        foreach (var item in allLight)
        {
            if (item.type == LightType.Directional)
                item.enabled = false;
            else
                item.enabled = true;
        }
        RenderSettings.ambientLight = dayColor;
    }

}
