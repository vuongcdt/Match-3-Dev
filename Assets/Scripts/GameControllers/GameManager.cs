using System;
using System.Collections;
using System.Collections.Generic;
using Commands;
using QFramework;
using Queries;
using UnityEngine;
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
        [SerializeField] private List<CellPos> cellTypes;
        [SerializeField] private bool isProcessing;

        private Cell[,] _grid;
        private bool _isRevertFill;

        private void Start()
        {
            this.SendCommand(new InitGridCommand(width, height, obstacles));
            _grid = this.SendQuery(new GetGridQuery());
            _grid = new Cell[width, height];
            RenderBackgroundGrid();
            RenderCellGrid();
            RenderRandomObstacles();

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
                    int[] checkArr = _isRevertFill ? new[] { 0, 1, -1 } : new[] { 0, -1, 1 };

                    foreach (var index in checkArr)
                    {
                        var source = _grid[x, y];
                        var isSourceFish = source.Type != CONSTANTS.CellType.None &&
                                           source.Type != CONSTANTS.CellType.Obstacle;
                        var isTargetEmpty = x + index >= 0 && x + index < width &&
                                            _grid[x + index, y - 1].Type == CONSTANTS.CellType.None;
                        if (isSourceFish && isTargetEmpty)
                        {
                            MoveToBelow(source, x, y, index);
                            break;
                        }
                    }
                }

                _isRevertFill = !_isRevertFill;

                yield return new WaitForSeconds(fillTime);
            }
        }

        private void MoveToBelow(Cell cellCurrent, int x, int y, int index = 0)
        {
            isProcessing = true;
            cellCurrent.Move(GetPositionCell(x + index, y - 1), fillTime);
            _grid[x + index, y - 1] = cellCurrent;
            _grid[x, y] =
                cell.Create(GetPositionCell(x, y), gridBlock, avatarSize, CONSTANTS.CellType.None);
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
        }

        private void RenderRandomObstacles()
        {
            var obstaclesTotal = 25;
            List<CellPos> cellPosList = new();
            var count = 0;

            while (cellPosList.Count < obstaclesTotal)
            {
                count++;
                var randomX = Random.Range(0, width);
                var randomY = Random.Range(0, height - 2);
                var cellPos = new CellPos(randomX, randomY);
                var isNoInList = cellPosList.IndexOf(cellPos) == -1;
                var isNearByInList =
                    (cellPosList.IndexOf(new CellPos(randomX + 1, randomY)) > -1 && randomX == width - 2) ||
                    (cellPosList.IndexOf(new CellPos(randomX - 1, randomY)) > -1 && randomX == 1);
                if (isNoInList && !isNearByInList)
                {
                    cellPosList.Add(cellPos);
                }

                if (count > 100)
                {
                    Debug.Log("VAR");
                    break;
                }
            }

            foreach (var obstacle in cellPosList)
            {
                if (obstacle.x < 0 || obstacle.x > width - 1 || obstacle.y < 0 || obstacle.y > height - 1)
                {
                    continue;
                }

                _grid[obstacle.x, obstacle.y].Type = CONSTANTS.CellType.Obstacle;
                _grid[obstacle.x, obstacle.y].ReSetAvatar();
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


        private void PrintGrint()
        {
            List<CellPos> cells = new();
            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                for (int y = 0; y < _grid.GetLength(1); y++)
                {
                    cells.Add(new CellPos(x, y, _grid[x, y].Type));
                }
            }

            cells.Reverse();
            cellTypes = cells;
        }
    }

    [Serializable]
    public struct CellPos
    {
        public int x;
        public int y;
        public CONSTANTS.CellType type;

        public CellPos(int x, int y) : this()
        {
            this.x = x;
            this.y = y;
        }

        public CellPos(int x, int y, CONSTANTS.CellType type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
        }
    }
}