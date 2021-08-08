using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
            ChangeDayLight();

        if (Input.GetKeyDown(KeyCode.Alpha6))
            ChangeNightLight();
    }

    Dictionary<Light, float> allLight; //float으로 intensity값을 저장
    //멤버변수 allLight
    public float changeDuration = 4;
    private void ChangeNightLight()
    {
        if(allLight == null) //null이면 딕셔너리 초기화
        {
            allLight = new Dictionary<Light, float>();
            var _allLight = FindObjectsOfType<Light>(); //지역변수 _allLight
            foreach (var item in _allLight)
            {
                allLight[item] = item.intensity;
            }
        }

        foreach (var item in allLight)
        {
            item.Key.enabled = true;
            if (item.Key.type == LightType.Directional)
                DOTween.To(() => 0f, (x) => item.Key.intensity = x, item.Value, changeDuration);
            else
                DOTween.To(() => item.Value, (x) => item.Key.intensity = x, 0, changeDuration);
        }
        DOTween.To(() => Camera.main.backgroundColor, (x) => Camera.main.backgroundColor = x, dayColor, changeDuration);
        //카메라의 백그라운드 컬러를 dayColor와 맞춰서 배경 이질감 줄이기
    }

    private void ChangeDayLight()
    {
        if (allLight == null)
        {
            allLight = new Dictionary<Light, float>();
            var _allLight = FindObjectsOfType<Light>();
            foreach (var item in _allLight)
            {
                allLight[item] = item.intensity;
            }
        }

        foreach (var item in allLight)
        {
            item.Key.enabled = true;
            if (item.Key.type != LightType.Directional)
                DOTween.To(() => 0f, (x) => item.Key.intensity = x, item.Value, changeDuration);
            else
                DOTween.To(() => item.Value, (x) => item.Key.intensity = x, 0, changeDuration);
        }
        DOTween.To(() => Camera.main.backgroundColor, (x) => Camera.main.backgroundColor = x, nightColor, changeDuration);
    }
}
