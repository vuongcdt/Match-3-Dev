using System;
using UnityEngine;

namespace BaseScripts
{
    [Serializable]
    public class Grid
    {
        private int _width;
        private int _height;
        private float _cellSize;
        private CellBase _cellBase;

        private int[,] _gridArray;
        private CellBase[,] _cellArray;


        //Init Grid
        public Grid(int width, int height, float cellSize)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _gridArray = new int[width, height];

            for (int x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < _gridArray.GetLength(1); y++)
                {
                    _gridArray[x, y] = (int)CONSTANTS.CellType.None;
                }
            }
        }

        //Render Cell
        public Grid(int width, int height, float cellSize, CellBase cellBase,
            Transform transformParent, float avatarSize)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _cellBase = cellBase;

            _gridArray = new int[width, height];
            _cellArray = new CellBase[width, height];

            for (int x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < _gridArray.GetLength(1); y++)
                {
                    var cell = _cellBase.Create(GetPositionCell(x, y), transformParent, cellSize);
                    cell.transform.localScale = Vector2.one * avatarSize * cellSize;
                    cell.name = $"Cell {x}-{y}";
                    cell.SetAll(x, y, default);

                    _cellArray[x, y] = cell;
                    _gridArray[x, y] = (int)CONSTANTS.CellType.None;
                }
            }
        }

        //Render Background
        public Grid(int width, int height, float cellSize, CellBase cellBase,
            Transform transformParent, Sprite backgroundSprite, float backgroundSize)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var background = cellBase.Create(GetPositionCell(x, y), transformParent, cellSize);
                    background.SetAvatar(backgroundSprite);
                    background.SetType(CONSTANTS.CellType.Background);
                    background.transform.localScale = Vector2.one * 0.9f * cellSize;
                    background.name = "Background";

                    Debug.DrawLine(GetPosition(x, y), GetPosition(x, y + 1), Color.black, 5000);
                    Debug.DrawLine(GetPosition(x, y), GetPosition(x + 1, y), Color.black, 5000);
                }
            }

            Debug.DrawLine(GetPosition(0, height), GetPosition(width, height), Color.black, 5000);
            Debug.DrawLine(GetPosition(width, 0), GetPosition(width, height), Color.black, 5000);
        }

        private Vector2 GetPositionCell(int x, int y)
        {
            return new Vector2(x - (_width - 1) * 0.5f, y - (_height - 1) * 0.5f) * _cellSize;
        }

        private Vector2 GetPosition(int x, int y)
        {
            return new Vector2(x - (_width - 1) * 0.5f, y - (_height - 1) * 0.5f) * _cellSize -
                   new Vector2(_cellSize, _cellSize) * 0.5f;
        }

        public void SetValue(int x, int y, Color color, int value)
        {
            if (x >= 0 && y >= 0 && x < _width && y < _height)
            {
                _gridArray[x, y] = value;
                _cellArray[x, y].SetText(value.ToString());
                _cellArray[x, y].SetAvatarColor(color);
            }
        }

        public void SetValue(int x, int y, Sprite sprite, CONSTANTS.CellType type)
        {
            if (x >= 0 && y >= 0 && x < _width && y < _height)
            {
                _cellArray[x, y].SetAvatar(sprite);
                _cellArray[x, y].SetType(type);
                _gridArray[x, y] = (int)type;
            }
        }

        public int[,] GetGridArray()
        {
            return _gridArray;
        }

        public CellBase[,] GetCellArray()
        {
            return _cellArray;
        }
    }
}