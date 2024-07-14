using System;
using System.Collections.Generic;
using GameControllers;
using UnityEngine;

public static class Utils
{
    [Serializable]
    public struct GridPos
    {
        public int x;
        public int y;

        public GridPos(int x, int y) : this()
        {
            this.x = x;
            this.y = y;
        }


        public new string ToString()
        {
            return $"x: {x} y: {y}";
        }
    }

    public struct UserData
    {
        public int Level;
        public int Star;

        public UserData(int level, int star)
        {
            Level = level;
            Star = star;
        }
    }
    
    public struct MatchCell
    {
        public readonly List<Cell> CellList;
        public readonly CONSTANTS.GridType Type;

        public MatchCell(List<Cell> cellList, CONSTANTS.GridType type)
        {
            CellList = cellList;
            Type = type;
        }
    }

    public static Vector2 GetWorldPosition(int x, int y, int width, int height, float cellSize)
    {
        return new Vector2(x - (width - 1) * 0.5f, y - (height - 1) * 0.5f) * cellSize;
    }

    public static GridPos GetGridPos(float x, float y, int width, int height, float cellSize)
    {
        var gridX = (-x / cellSize + (width - 1) * 0.5f);
        var gridY = (-y / cellSize + (height - 1) * 0.5f);

        var gridWidth = width - 1 - (int)gridX;
        var gridHeight = height - 1 - (int)gridY;

        return new GridPos(gridWidth, gridHeight);
    }

    public static bool IsNotInverted(Cell sourceCell, Cell targetCell)
    {
        var isObstacle = sourceCell.Type == CONSTANTS.CellType.Obstacle ||
                         targetCell.Type == CONSTANTS.CellType.Obstacle;
        var isEmpty = sourceCell.Type == CONSTANTS.CellType.None || targetCell.Type == CONSTANTS.CellType.None;

        return isObstacle || isEmpty;
    }
}