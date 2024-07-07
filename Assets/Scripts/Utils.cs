using System;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    [Serializable]
    public struct GridPos
    {
        public int x;
        public int y;
        public CONSTANTS.CellType type;

        public GridPos(int x, int y) : this()
        {
            this.x = x;
            this.y = y;
        }

        public GridPos(Vector2 pos) : this()
        {
            this.x = (int)pos.x;
            this.y = (int)pos.y;
        }

        public GridPos(int x, int y, CONSTANTS.CellType type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
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
    public struct SettingsGrid
    {
        public int Width;
        public int Height;
        public float CellSize;

        // private Cell cell;
        // private Transform backgroundBlock;
        // private Transform gridBlock;
        // private Vector2[] obstacles;
        // private float avatarSize;
        // private float backgroundSize;
        // private float fillTime;
        // private List<Utils.GridPos> cellTypes;
        // private bool isProcessing;
        //
        // private Cell[,] _grid;
        // private bool _isRevertFill;

        public SettingsGrid(int width, int height, float cellSize) : this()
        {
            this.Width = width;
            this.Height = height;
            this.CellSize = cellSize;
        }
    }

    public static Vector2 GetPositionCell(int x, int y, int width, int height, float cellSize)
    {
        return new Vector2(x - (width - 1) * 0.5f, y - (height - 1) * 0.5f) * cellSize;
    }
}