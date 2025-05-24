using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Block blockPrefab;
    [SerializeField] private Vector2 spacing;
    [SerializeField] private Vector2 groupOffset;

    public Block[,] GenGrid(LevelConfig levelConfig)
    {
        Block[,] blocks = new Block[levelConfig.heigh, levelConfig.width];
        float blockScale = 1;
        var startX = -(levelConfig.width - 1) * (blockScale + spacing.x) / 2 + groupOffset.x;
        var startY = (levelConfig.heigh - 1) * (blockScale + spacing.y) / 2 + groupOffset.y;
        for (int i = 0; i < levelConfig.heigh; i++)
        {
            var yPos = startY - i * (blockScale + spacing.y);
            for (int j = 0; j < levelConfig.width; j++)
            {
                var xPos = startX + j * (blockScale + spacing.x);
                var block = Instantiate(blockPrefab, transform).GetComponent<Block>();
                block.transform.position = new Vector3(xPos, yPos);
                blocks[i, j] = block;
            }
        }
        return blocks;
    }

}
