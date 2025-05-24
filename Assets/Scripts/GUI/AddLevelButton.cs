using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddLevelButton : MonoBehaviour
{
    public void AddLevel()
    {
        LevelHelper.CreateNew();
    }
}
