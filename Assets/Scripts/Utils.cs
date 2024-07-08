﻿using System;
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
        public float FillTime;

        public SettingsGrid(int width, int height, float cellSize, float fillTime) : this()
        {
            Width = width;
            Height = height;
            CellSize = cellSize;
            FillTime = fillTime;
        }
    }

    public static Vector2 GetPositionCell(int x, int y, int width, int height, float cellSize)
    {
        return new Vector2(x - (width - 1) * 0.5f, y - (height - 1) * 0.5f) * cellSize;
    }

    public static Utils.GridPos GetGridPos(float x, float y, int width, int height, float cellSize)
    {
        var gridX = (-x / cellSize + (width - 1) * 0.5f);
        var gridY = (-y / cellSize + (height - 1) * 0.5f);

        var gridWidth = width - 1 - (int)gridX;
        var gridHeight = height - 1 - (int)gridY;

        return new Utils.GridPos(gridWidth, gridHeight);
    }
}