using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : Singleton<ConfigManager>
{
    public List<BlockColorConfig> blockColorConfigs;
    public List<LevelConfig> levelConfigs;

    private void Awake()
    {
        levelConfigs = LevelHelper.GetAll();
    }

    private void Start()
    {

    }
}
