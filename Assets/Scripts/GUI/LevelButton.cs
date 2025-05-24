using Framework.GUI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtLevel;
    private LevelConfig level;
    public void SetData(LevelConfig level)
    {
        this.level = level;
        txtLevel.text = level.level.ToString();
    }

    public void OpenManagePopup()
    {
        GUIManager.ShowUI<ManageUI>().SetData(level);
    }
}
