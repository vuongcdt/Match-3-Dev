using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class Utils
{
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

        public GridPos(int x, int y, CONSTANTS.CellType type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
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
    
    public static IEnumerator ClearConsoleIE()
    {
        yield return new WaitForSeconds(3);
        var assembly = Assembly.GetAssembly(typeof(SceneView));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}