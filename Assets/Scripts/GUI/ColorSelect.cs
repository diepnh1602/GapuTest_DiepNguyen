using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelect : MonoBehaviour
{
    [SerializeField] private GameObject goSelect;
    [SerializeField] private Image img;
    public Action<ColorSelect> OnClick;
    public BlockColorConfig color;
    public void SetColor(BlockColorConfig colorConfig)
    {
        this.color = colorConfig;
        img.color = colorConfig.color;
    }

    public void Click()
    {
        OnClick?.Invoke(this);
    }

    public void Select()
    {
        goSelect.Show();
    }

    public void UnSelect()
    {
        goSelect.Hide();
    }
}
