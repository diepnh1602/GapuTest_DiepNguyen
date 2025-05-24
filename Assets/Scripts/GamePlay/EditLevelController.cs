using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditLevelController : Singleton<EditLevelController>
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Block[,] blocks;
    private LevelConfig level;
    private BlockColorConfig currentColor;
    private DrawState state;
    public void OnSelectColor(BlockColorConfig obj)
    {
        currentColor = obj;
    }

    public void Init(LevelConfig levelConfig)
    {
        this.level = levelConfig;
        blocks = gridManager.GenGrid(levelConfig);
        for (int i = 0; i < blocks.GetLength(0); i++)
        {
            for (int j = 0; j < blocks.GetLength(1); j++)
            {
                blocks[i, j].OnMouseDownBlock += OnMouseDownBlock;
                blocks[i, j].OnMouseEnterBlock += OnMouseEnterBlock;
                blocks[i, j].OnMouseUpBlock += OnMouseUpBlock;
                int level = 0;
                if (i < levelConfig.colors.Length)
                {
                    level = levelConfig.colors[i, j];
                }
                blocks[i, j].SetColor(level);
            }
        }
        state = DrawState.Wait;
        GameEventManager.Instance.TriggerEvent(EventName.ColorWaitUpdate);
    }
    private void OnMouseDownBlock(Block block)
    {
        if (state == DrawState.Wait)
        {
            state = DrawState.Draw;
            block.SetColor(currentColor.id);
        }
    }

    private void OnMouseEnterBlock(Block block)
    {
        if (state == DrawState.Draw)
        {
            block.SetColor(currentColor.id);
        }
    }

    private void OnMouseUpBlock(Block block)
    {
        state = DrawState.Wait;
    }
    public void Clear()
    {
        state = DrawState.None;
        foreach (var block in blocks)
        {
            Destroy(block.gameObject);
        }
    }

    public void SaveLevel()
    {

        for (int i = 0; i < blocks.GetLength(0); i++)
        {
            for (int j = 0; j < blocks.GetLength(1); j++)
            {
                level.colors[i, j] = blocks[i, j].id;
            }
        }
        LevelHelper.Save(ConfigManager.Instance.levelConfigs);
    }

    public enum DrawState
    {
        None, Wait, Draw
    }
}
