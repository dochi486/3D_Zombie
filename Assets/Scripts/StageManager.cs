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
        base.Awake();
        //highScore = new SaveInt(highScore.Value.ToNumber()); //이렇게 하면 다른 스크립트에서 오류가 생길 수 있기 때문에
        highScore = new SaveInt("highScore"); //이렇게 해주는 것이 좋다
        gold = new SaveInt("Gold");
        ScoreUI.Instance.ScoreUIRefresh(score, highScore.Value);


    }
    public void AddScore(int addScore)
    {
        score += addScore;
        if (highScore.Value < score)
            highScore.Value = score;

        ScoreUI.Instance.ScoreUIRefresh(score, highScore.Value); //UI를 새로고침한다.
    }

    public SaveInt gold;
    internal void AddGold(int amount)
    {
        gold += amount;

        GoldUIRefresh();
    }


    private void GoldUIRefresh()
    {
        GoldUI.Instance.GoldUIRefresh(gold);
    }
}
