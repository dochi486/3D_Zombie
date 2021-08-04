using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageManager : SingletonMonoBehavior<StageManager>
{
    public SaveInt highScore; //저장된 최고 점수 값
    public int score; //현재 스테이지의 스코어

    new private void Awake()
    {
        highScore = new SaveInt(highScore.Value.ToNumber());
        ScoreUI.Instance.ScoreUIRefresh(score, highScore.Value);
    }
    public void AddScore(int addScore)
    {
        score += addScore;
        if (highScore.Value < score)
            highScore.Value = score;

        ScoreUI.Instance.ScoreUIRefresh(score, highScore.Value); //UI를 새로고침한다.
    }


}
