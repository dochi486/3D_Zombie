using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : SingletonMonoBehavior<HealthUI>
{
    public List<Image> images = new List<Image>();
    public Image enable, current, disable;

    TextMeshProUGUI valueText;

    protected override void OnInit()
    {
        valueText = transform.Find("ValueText").GetComponent<TextMeshProUGUI>(); 
        //OnInit은 유니티 기본 함수의 실행 순서와 관계 없이 항상 실행되므로 초기화할 때 OnInit에서 사용하는 것이 좋다.
    }
    
}
