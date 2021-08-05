using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldUI : SingletonMonoBehavior<GoldUI>
{
    TextMeshProUGUI goldText;
    // Start is called before the first frame update
    void Start()
    {
        goldText = transform.Find("GoldText").GetComponent<TextMeshProUGUI>();
    }

    public void GoldUIRefresh(int gold)
    {
        goldText.text = gold.ToNumber();
    }
}
