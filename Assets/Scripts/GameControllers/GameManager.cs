using System;
using System.Collections;
using System.Collections.Generic;
using BaseScripts;
using Commands;
using QFramework;
using Queries;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace GameControllers
{
    public class GameManager : MonoBehaviour, IController
    {
        [SerializeField] private Cell cell;
        [SerializeField] private Transform backgroundBlock;
        [SerializeField] private Transform gridBlock;
        [SerializeField] private Vector2[] obstacles;
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private float cellSize;
        [SerializeField] private float avatarSize;
        [SerializeField] private float backgroundSize;
        [SerializeField] private float fillTime;
        [SerializeField] private List<CellStruct> cellTypes;
        [SerializeField] private bool isProcessing;

        private Cell[,] _grid;

        private void Start()
        {
            this.SendCommand(new InitGridCommand(width, height, obstacles));
            _grid = this.SendQuery(new GetGridQuery());
            _grid = new Cell[width, height];
            RenderBackgroundGrid();
            RenderCellGrid();

            StartCoroutine(FillIE());
        }

        private IEnumerator FillIE()
        {
            do
            {
                isProcessing = false;
                yield return new WaitForSeconds(fillTime);
                Shoot();
                StartCoroutine(Fill());
            } while (isProcessing);
        }

        private void Shoot()
        {
            for (int x = 0; x < width; x++)
            {
                var cellBelow = _grid[x, height - 1];
                if (cellBelow.Type == CONSTANTS.CellType.None)
                {
                    var random = Random.Range(3, 9);
                    isProcessing = true;
                    var newCell = 
                        cell.Create(GetPositionCell(x, height), gridBlock, avatarSize, (CONSTANTS.CellType)random);
                    newCell.Move(GetPositionCell(x, height - 1), fillTime);
                    _grid[x, height - 1] = newCell;
                }
            }
        }

        private IEnumerator Fill()
        {
            for (int y = height - 1; y > 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    var cellCurrent = _grid[x, y];
                    var cellBelow = _grid[x, y - 1];

                    var isBelowLeftEmpty = x > 0 && _grid[x - 1, y - 1].Type == CONSTANTS.CellType.None;
                    var isBelowRightEmpty = x < width - 1 && _grid[x + 1, y - 1].Type == CONSTANTS.CellType.None;
                    var isBelowEmpty = cellBelow.Type == CONSTANTS.CellType.None;
                    var isCellCurrentNormal = cellCurrent.Type != CONSTANTS.CellType.None && cellCurrent.Type != CONSTANTS.CellType.Obstacle;
                    var isBelowNotEmpty = cellBelow.Type != CONSTANTS.CellType.None;

                    if (isBelowEmpty && isCellCurrentNormal)
                    {
                        isProcessing = true;
                        cellCurrent.Move(GetPositionCell(x, y - 1), fillTime);
                        _grid[x, y - 1] = cellCurrent;
                        _grid[x, y] = 
                            cell.Create(GetPositionCell(x, y), gridBlock, avatarSize, CONSTANTS.CellType.None);
                    }
                    else if (isBelowNotEmpty && isBelowLeftEmpty && isCellCurrentNormal)
                    {
                        isProcessing = true;
                        cellCurrent.Move(GetPositionCell(x - 1, y - 1), fillTime);
                        _grid[x - 1, y - 1] = cellCurrent;
                        _grid[x, y] = 
                            cell.Create(GetPositionCell(x, y), gridBlock, avatarSize, CONSTANTS.CellType.None);
                    }
                    else if (isBelowNotEmpty && isBelowRightEmpty && isCellCurrentNormal)
                    {
                        isProcessing = true;
                        cellCurrent.Move(GetPositionCell(x + 1, y - 1), fillTime);
                        _grid[x + 1, y - 1] = cellCurrent;
                        _grid[x, y] = 
                            cell.Create(GetPositionCell(x, y), gridBlock, avatarSize, CONSTANTS.CellType.None);
                    }
                }

                yield return new WaitForSeconds(fillTime);
            }
        }

        private void FillCell(Vector2 source, Vector2 target)
        {
            isProcessing = true;
            var cellSource = _grid[(int)source.x, (int)source.y];
            cellSource.Move(GetPositionCell(target), fillTime);
            _grid[(int)target.x, (int)target.y - 1] = cellSource;
            _grid[(int)source.x, (int)source.y] = cell.Create(GetPositionCell(source), gridBlock,
                avatarSize, CONSTANTS.CellType.None);
        }

        [Serializable]
        private struct CellStruct
        {
            public int X;
            public int Y;
            public CONSTANTS.CellType Type;

            public CellStruct(int x, int y, CONSTANTS.CellType type)
            {
                X = x;
                Y = y;
                Type = type;
            }
        }

        private void PrintGrint()
        {
            List<CellStruct> cells = new();
            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                for (int y = 0; y < _grid.GetLength(1); y++)
                {
                    cells.Add(new CellStruct(x, y, _grid[x, y].Type));
                }
            }

            cells.Reverse();
            cellTypes = cells;
        }

        private void RenderBackgroundGrid()
        {
            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                for (int y = 0; y < _grid.GetLength(1); y++)
                {
                    var background = cell.Create(GetPositionCell(x, y), backgroundBlock, backgroundSize,
                        CONSTANTS.CellType.Background);
                    background.name = "Background";
                }
            }
        }

        private void RenderCellGrid()
        {
            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                for (int y = 0; y < _grid.GetLength(1); y++)
                {
                    cell.Type = CONSTANTS.CellType.None;
                    var newCell = cell.Create(GetPositionCell(x, y), gridBlock, avatarSize,
                        CONSTANTS.CellType.None);
                    _grid[x, y] = newCell;
                }
            }

            foreach (var obstacle in obstacles)
            {
                if (obstacle.x < 0 || obstacle.x > width - 1 || obstacle.y < 0 || obstacle.y > height - 1)
                {
                    continue;
                }

                _grid[(int)obstacle.x, (int)obstacle.y].Type = CONSTANTS.CellType.Obstacle;
                _grid[(int)obstacle.x, (int)obstacle.y].ReSetAvatar();
            }
        }

        private Vector2 GetPositionCell(int x, int y)
        {
            return new Vector2(x - (width - 1) * 0.5f, y - (height - 1) * 0.5f) * cellSize;
        }
        private Vector2 GetPositionCell(Vector2 pos)
        {
            return new Vector2((int)pos.x - (width - 1) * 0.5f, (int)pos.y - (height - 1) * 0.5f) * cellSize;
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}