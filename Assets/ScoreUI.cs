using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : SingletonMonoBehavior<ScoreUI>
{
    
    TextMeshProUGUI scoreText;
    TextMeshProUGUI highScoreText
        ;
    //StageManager는 씬이 사라질 때 함께 부서지도록
    protected override void OnInit()
    {
        //base.Awake();
        ////Awake에서 하지 않는 이유-> 아주 가끔씩 Awake보다 더 먼저 싱글턴이 먼저 호출되는 경우가 있는데 그 때에도 호출이 되도록 하려고
        //highScore = new SaveInt("highScore");

        scoreText = transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        highScoreText = transform.Find("HighScoreText").GetComponent<TextMeshProUGUI>();
    }
    public void ScoreUIRefresh(int score, int highScore)
    {
        scoreText.text = score.ToNumber();
        highScoreText.text = "High :" + highScore.ToNumber();
    }
}

