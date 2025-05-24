using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameplayController : Singleton<GameplayController>
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Block[,] blocks;
    private GamePlayState state;
    private List<Block> drawBlocks = new List<Block>();
    private LevelConfig level;
    public ColorWait current;
    public ColorWait nextWait;
    public int score;
    public void Play(LevelConfig levelConfig)
    {
        this.level = levelConfig;
        state = GamePlayState.Wait;
        blocks = gridManager.GenGrid(levelConfig);
        drawBlocks.Clear();
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
        SetScore(0);
        current = new ColorWait(ConfigManager.Instance.blockColorConfigs);
        nextWait = new ColorWait(ConfigManager.Instance.blockColorConfigs);
        GameEventManager.Instance.TriggerEvent(EventName.ColorWaitUpdate);
    }

    private void OnMouseDownBlock(Block block)
    {
        if (IsPointerOverUI()) return;
        if (state == GamePlayState.Wait && block.canChoose)
        {
            state = GamePlayState.Draw;
            AddDrawBlocks(block);
        }
    }

    private void OnMouseEnterBlock(Block block)
    {
        if (IsPointerOverUI()) return;
        if (state == GamePlayState.Draw && block.canChoose)
        {
            AddDrawBlocks(block);
        }
    }

    private void OnMouseUpBlock(Block block)
    {
        if (IsPointerOverUI()) return;
        if (state == GamePlayState.Draw)
        {
            Check();
        }
    }

    public void AddDrawBlocks(Block block)
    {
        if (!drawBlocks.Contains(block) && drawBlocks.Count < current.amount)
        {
            block.SetColor(current.id);
            drawBlocks.Add(block);

            foreach (var blockUI in blocks)
            {
                blockUI.SetCanChoose(false);
            }
            if (drawBlocks.Count < current.amount)
            {
                for (int i = 0; i < blocks.GetLength(0); i++)
                {
                    for (int j = 0; j < blocks.GetLength(1); j++)
                    {

                        if (blocks[i, j] == block)
                        {
                            //
                            var top = i - 1;
                            if (top >= 0 && blocks[top, j].id == 0)
                            {
                                blocks[top, j].SetCanChoose(true);
                            }
                            //
                            var bot = i + 1;
                            if (bot < level.heigh && blocks[bot, j].id == 0)
                            {
                                blocks[bot, j].SetCanChoose(true);
                            }
                            //
                            var left = j - 1;
                            if (left >= 0 && blocks[i, left].id == 0)
                            {
                                blocks[i, left].SetCanChoose(true);
                            }
                            //
                            var right = j + 1;
                            if (right < level.width && blocks[i, right].id == 0)
                            {
                                blocks[i, right].SetCanChoose(true);
                            }
                        }
                    }
                }
            }
            foreach(var b in drawBlocks)
            {
                b.SetLight(true);
            }
        }
    }

    public void Check()
    {
        state = GamePlayState.Check;

        if (drawBlocks.Count == current.amount)
        {
            var listEarnScore = new List<Block>();
            //check hang
            var bblock = new List<Block>();
            bool check = false;
            for (int i = 0; i < blocks.GetLength(0); i++)
            {
                bblock.Clear();
                check = true;
                for(int j = 0; j < blocks.GetLength(1); j++)
                {
                    bblock.Add(blocks[i, j]);
                    if (blocks[i, j].id == 0)
                    {
                        check = false; break;
                    }
                }
                if (check)
                {
                    foreach (var block in bblock)
                    {
                        if (!listEarnScore.Contains(block))
                        {
                            listEarnScore.Add(block);
                        }
                    }
                }
            }
            //check cot
            for (int i = 0; i < blocks.GetLength(1); i++)
            {
                bblock.Clear();
                check = true;
                for (int j = 0; j < blocks.GetLength(0); j++)
                {
                    bblock.Add(blocks[j, i]);
                    if (blocks[j, i].id == 0)
                    {
                        check = false; break;
                    }
                }
                if (check)
                {
                    foreach (var block in bblock)
                    {
                        if (!listEarnScore.Contains(block))
                        {
                            listEarnScore.Add(block);
                        }
                    }
                }
            }
            if(listEarnScore.Count > 0)
            {
                SetScore(score + listEarnScore.Count);
                for (int i = 0;i < listEarnScore.Count; i++)
                {
                    listEarnScore[i].SetColor(0);
                }
            }
            //
            drawBlocks.Clear();
            current = nextWait;
            nextWait = new ColorWait(ConfigManager.Instance.blockColorConfigs);
            GameEventManager.Instance.TriggerEvent(EventName.ColorWaitUpdate);
        }
        else
        {
            foreach (var block in drawBlocks)
            {
                block.SetColor(0);
            }
            drawBlocks.Clear();
        }
        foreach (var block in blocks)
        {
            block.RefreshCanchooseByID();
        }
        state = GamePlayState.Wait;
    }

    public void GameEnd()
    {
        state = GamePlayState.None;
        drawBlocks.Clear();
        foreach (var block in blocks)
        {
            Destroy(block.gameObject);
        }
    }

    public void SetScore(int score)
    {
        this.score = score;
        GameEventManager.Instance.TriggerEvent(EventName.UpdateScore);
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public enum GamePlayState
    {
        None, Wait, Draw, Check
    }
}