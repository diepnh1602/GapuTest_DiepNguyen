using Framework.GUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditLevelUI : UIBase
{
    [SerializeField] private ColorSelect colorSelectPref;
    [SerializeField] private Transform colorsTF;
    private List<ColorSelect> selects = new List<ColorSelect>();
    
    protected override void OnShow()
    {
        base.OnShow();
        var configs = ConfigManager.Instance.blockColorConfigs;
        foreach (var config in configs)
        {
            var ui = Instantiate(colorSelectPref, colorsTF);
            ui.SetColor(config);
            ui.OnClick += Select;
            ui.UnSelect();
            selects.Add(ui);
        }
        Select(selects[0]);
    }

    protected override void OnHide()
    {
        base.OnHide();
        foreach (var s in selects)
        {
            Destroy(s.gameObject);
        }
        selects.Clear();
    }

    private void Select(ColorSelect select)
    {
        foreach (var s in selects)
        {
            if (s == select)
            {
                s.Select();
            }
            else
            {
                s.UnSelect();
            }
        }
        EditLevelController.Instance.OnSelectColor(select.color);
    }

    public void Back()
    {
        GUIManager.ShowOnlyUI<HomeUI>();
        EditLevelController.Instance.Clear();
        foreach (var s in selects)
        {
            Destroy(s.gameObject);
        }
        selects.Clear();
    }

    public void Save()
    {
        EditLevelController.Instance.SaveLevel();
    }
}
