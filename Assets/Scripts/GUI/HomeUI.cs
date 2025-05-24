using Framework.GUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeUI : UIBase
{
    [SerializeField] private LevelButton levelButtonPrefab;
    [SerializeField] private Transform levelHolder;
    [SerializeField] private AddLevelButton addBtn;
    private List<LevelButton> levelButtons = new List<LevelButton>();

    protected override void OnDataUpdate(object obj = null)
    {
        base.OnDataUpdate(obj);

        foreach (var button in levelButtons)
        {
            Destroy(button.gameObject);
        }
        levelButtons.Clear();

        var levels = ConfigManager.Instance.levelConfigs;
        foreach (var level in levels)
        {
            var btn = Instantiate(levelButtonPrefab, levelHolder);
            btn.SetData(level);
            levelButtons.Add(btn);
        }
        addBtn.transform.SetAsLastSibling();
    }
}
