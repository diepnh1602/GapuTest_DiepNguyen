using Framework.GUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private void Start()
    {
        GUIManager.ShowUI<HomeUI>();
    }
}
