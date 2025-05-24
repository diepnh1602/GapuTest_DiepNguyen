using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorWait
{
    public int id;
    public int amount;

    public ColorWait(List<BlockColorConfig> configs)
    {
        int randIDx = Random.Range(1, configs.Count);
        id = configs[randIDx].id;
        amount = Random.Range(2, 6);
    }
}
