using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoUI : GaugeUI<AmmoUI>
{
    internal void SetBulletCount(int bulletCountInClip, int maxBulletCountInClip, int allBulletCount, int maxBulletCount)
    {
        SetGauge(bulletCountInClip, maxBulletCountInClip);
        valueText.text = $"{allBulletCount}/{maxBulletCount}";
    }

    internal void StartReload(int bulletCountInClip, int maxBulletCountInClip, int allBulletCount, int maxBulletCount, float duration)
    {
        //총알UI 이미지가 서서히 차오르도록 구현
        StartCoroutine(SetAnimateGaugeCo(bulletCountInClip, maxBulletCountInClip, duration));


        valueText.text = $"{allBulletCount}/{maxBulletCount}";
    }
}
