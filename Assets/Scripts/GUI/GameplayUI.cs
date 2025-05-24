using Framework.GUI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameplayUI : UIBase
{
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private ColorWaitUI current, next;

    protected override void OnShow()
    {
        base.OnShow();
        ColorWaitUpdate();
        UpdateScore();
        GameEventManager.Instance.StartListening(EventName.ColorWaitUpdate, ColorWaitUpdate);
        GameEventManager.Instance.StartListening(EventName.UpdateScore, UpdateScore);
    }

    protected override void OnHide()
    {
        base.OnHide();
        GameEventManager.Instance.StopListening(EventName.ColorWaitUpdate, ColorWaitUpdate);
        GameEventManager.Instance.StopListening(EventName.UpdateScore, UpdateScore);
    }

    private void UpdateScore(object obj = null)
    {
        score.text = "Score: " + GameplayController.Instance.score;
    }

    private void ColorWaitUpdate(object obj = null)
    {
        this.current.SetData(GameplayController.Instance.current);
        this.next.SetData(GameplayController.Instance.nextWait);
    }

    public void Back()
    {
        GUIManager.ShowOnlyUI<HomeUI>();
        GameplayController.Instance.GameEnd();
    }
}
