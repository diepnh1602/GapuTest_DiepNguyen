using Framework.GUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageUI : UIBase
{
    private LevelConfig _levelConfig;
    public void SetData(LevelConfig level)
    {
        this._levelConfig = level;
    }

    public void Play()
    {
        if (this._levelConfig != null)
        {
            GameplayController.Instance.Play(this._levelConfig);
            GUIManager.ShowOnlyUI<GameplayUI>();
        }
    }

    public void Edit()
    {
        if(this._levelConfig != null)
        {
            EditLevelController.Instance.Init(this._levelConfig);
            GUIManager.ShowOnlyUI<EditLevelUI>();
        }
    }
}
