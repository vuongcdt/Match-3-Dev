using System;
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
    
    
    public struct SettingsGrid
    {
        public int Width;
        public int Height;
        public float CellSize;

        public SettingsGrid(int width, int height, float cellSize)
        {
            this.Width = width;
            this.Height = height;
            this.CellSize = cellSize;
        }
    }
}