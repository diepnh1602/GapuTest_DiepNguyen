using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelHelper
{
    public static string levelTxtPath = Path.Combine(Application.streamingAssetsPath, "Configs/Level.txt");
    public static void CreateNew()
    {
        var list = ConfigManager.Instance.levelConfigs;
        var newLevel = new LevelConfig();
        newLevel.level = list.Count + 1;
        newLevel.heigh = 8;
        newLevel.width = 8;
        newLevel.colors = new int[newLevel.heigh, newLevel.width];
        list.Add(newLevel);
        Save(list);
        GameEventManager.Instance.TriggerEvent(EventName.OnDataUpdated);
    }

    public static void Save(List<LevelConfig> data)
    {
        FileService.Save(levelTxtPath, data);
    }

    public static List<LevelConfig> GetAll()
    {
        var list = FileService.Get<List<LevelConfig>>(levelTxtPath);
        if (list == null)
        {
            list = new List<LevelConfig>();
        }
        return list;
    }
}
