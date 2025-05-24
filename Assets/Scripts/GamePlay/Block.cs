using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    [SerializeField] private SpriteRenderer img;
    [SerializeField] private GameObject lightobj;
    public Action<Block> OnMouseDownBlock;
    public Action<Block> OnMouseEnterBlock;
    public Action<Block> OnMouseUpBlock;
    public bool canChoose;
    public int id;
    private void OnMouseDown()
    {
        OnMouseDownBlock?.Invoke(this);
    }
    private void OnMouseEnter()
    {
        OnMouseEnterBlock?.Invoke(this);
    }

    private void OnMouseUp()
    {
        OnMouseUpBlock?.Invoke(this);
    }

    public void SetColor(int colorID)
    {
        this.id = colorID;
        var colorrr = ConfigManager.Instance.blockColorConfigs.Find(x => x.id == colorID);
        img.color = colorrr.color;
        RefreshCanchooseByID();
    }

    public void SetCanChoose(bool canChoose)
    {
        this.canChoose = canChoose;
        Color c = img.color;
        c.a = 1;
        if (!canChoose && id == 0)
        {
            c.a = 0.5f;
        }
        img.color = c;
    }

    public void RefreshCanchooseByID()
    {
        SetCanChoose(id == 0);
        SetLight(false);
    }

    public void SetLight(bool value)
    {
        lightobj.gameObject.SetActive(value);
    }
}
