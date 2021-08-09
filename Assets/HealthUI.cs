using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : GaugeUI<HealthUI>
{

}

public class GaugeUI<T> : SingletonMonoBehavior<T>
    where T: SingletonBase //Generic으로 GaugeUI 변환..? 
{
    public List<Image> images = new List<Image>();
    public Sprite enable, current, disable;

    public TextMeshProUGUI valueText;

    protected override void OnInit()
    {
        valueText = transform.Find("ValueText").GetComponent<TextMeshProUGUI>(); 
        //OnInit은 유니티 기본 함수의 실행 순서와 관계 없이 항상 실행되므로 초기화할 때 OnInit에서 사용하는 것이 좋다.
    }

    internal void SetGauge(int value, int maxValue)
    {
        valueText.text = $"{value}/{maxValue}";

        float percent = (float)value / (float)maxValue; //hp와 maxHp모두 int이기 때문에 둘이 연산하면 결과값도 int로 반올림해버린다.
        //둘 중 하나를 float으로 바꾸면 float과 int의 연산 결과는 float(더 넓은 범위를 포함)으로 나오기 때문에 하나를 바꿔주면 된다. 

        int currentCount = Mathf.RoundToInt(percent * images.Count);
        for (int i = 0; i < images.Count; i++) //리스트로 만들었기 때문에 나는 count로 해야지...
        {
            if (i == currentCount)
                images[i].sprite = current;
            else if (i < currentCount)
                images[i].sprite = enable;
            else
                images[i].sprite = disable;
        }
    }
}
