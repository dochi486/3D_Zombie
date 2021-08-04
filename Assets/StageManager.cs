using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageManager : SingletonMonoBehavior<StageManager>
{
    public int highScore; //저장된 최고 점수 값
    public int score; //현재 스테이지의 스코어

    public void AddScore(int addScore)
    {
        score += addScore;
        if (highScore < score)
            highScore = score;

        ScoreUI.Instance.UIRefresh(score, highScore); //UI를 새로고침한다.
    }


}
