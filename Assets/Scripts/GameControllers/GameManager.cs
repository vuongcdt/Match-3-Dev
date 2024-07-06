using System.Collections;
using System.Collections.Generic;
using Commands;
using QFramework;
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
        [SerializeField] private List<Utils.GridPos> cellTypes;
        [SerializeField] private bool isProcessing;

        private Cell[,] _grid;
        private bool _isRevertFill;

        private void Start()
        {
            this.SendCommand(new InitSettingsGridModelCommand(width, height, cellSize));
            _grid = new Cell[width, height];

            RenderBackgroundGrid();
            RenderCellGrid();
            RenderRandomObstacles();
            StartCoroutine(ProcessingIE());

            this.SendCommand(new InitGridModelCommand(_grid));
        }

        private IEnumerator ProcessingIE()
        {
            do
            {
                isProcessing = false;
                yield return new WaitForSeconds(fillTime);
                AddCellToGrid();
                StartCoroutine(FillIE());
            } while (isProcessing);

            MatchGrid();
        }

        private int _tempCount = 0;

        private void MatchGrid()
        {
            List<List<Cell>> cellsList = new();
            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                int index = 0;
                for (int y = 0; y < _grid.GetLength(1) - 2; y++)
                {
                    if (index >= y)
                    {
                        continue;
                    }

                    var currentCell = _grid[x, y];
                    index = MatchCellX(x, y, currentCell, cellsList);
                }
            }

            for (int y = 0; y < _grid.GetLength(1); y++)
            {
                int index = 0;
                for (int x = 0; x < _grid.GetLength(0) - 2; x++)
                {
                    if (index >= x)
                    {
                        continue;
                    }

                    var currentCell = _grid[x, y];
                    index = MatchCellY(x, y, currentCell, cellsList);
                }
            }

            MergeCells(cellsList);

            print($"temp count {_tempCount}");
        }

        private void MergeCells(List<List<Cell>> cellsList)
        {
            foreach (var cells in cellsList)
            {
                foreach (var cell in cells)
                {
                    cell.gameObject.SetActive(false);
                    cell.Type = CONSTANTS.CellType.None;
                }
            }

            if (cellsList.Count > 0)
            {
                StartCoroutine(ProcessingIE());
            }
        }

        private int MatchCellX(int x, int y, Cell currentCell, List<List<Cell>> cellsList)
        {
            List<Cell> cells = new();
            for (int newY = y + 1; newY < _grid.GetLength(1); newY++)
            {
                _tempCount++;
                var upCell = _grid[x, newY];
                if (upCell.Type == currentCell.Type)
                {
                    cells.Add(upCell);
                }
                else
                {
                    break;
                }
            }

            if (cells.Count >= 2)
            {
                cells.Add(currentCell);
                cellsList.Add(cells);
            }

            return cells.Count + y;
        }

        private int MatchCellY(int x, int y, Cell currentCell, List<List<Cell>> cellsList)
        {
            List<Cell> cells = new();
            for (int newX = x + 1; newX < _grid.GetLength(0); newX++)
            {
                _tempCount++;
                var upCell = _grid[newX, y];
                if (upCell.Type == currentCell.Type)
                {
                    cells.Add(upCell);
                }
                else
                {
                    break;
                }
            }

            if (cells.Count >= 2)
            {
                cells.Add(currentCell);
                cellsList.Add(cells);
            }

            return cells.Count + x;
        }

        private void AddCellToGrid()
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

        private IEnumerator FillIE()
        {
            for (int y = height - 1; y > 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    CheckFill(x, y);
                }

                _isRevertFill = !_isRevertFill;

                yield return new WaitForSeconds(fillTime);
            }
        }

        private void CheckFill(int x, int y)
        {
            int[] checkArr = _isRevertFill ? new[] { 0, 1, -1 } : new[] { 0, -1, 1 };

            foreach (var index in checkArr)
            {
                if (x + index < 0 || x + index >= width)
                {
                    continue;
                }

                var source = _grid[x, y];
                var target = _grid[x + index, y - 1];
                var isSourceFish = source.Type != CONSTANTS.CellType.None &&
                                   source.Type != CONSTANTS.CellType.Obstacle;
                var isTargetEmpty = target.Type == CONSTANTS.CellType.None;

                if (isSourceFish && isTargetEmpty)
                {
                    MoveToBelow(source, target, x, y, index);
                    break;
                }
            }
        }

        private void MoveToBelow(Cell cellSource, Cell cellTarget, int x, int y, int index = 0)
        {
            isProcessing = true;
            cellSource.Move(GetPositionCell(x + index, y - 1), fillTime);
            _grid[x + index, y - 1] = cellSource;
            _grid[x, y] = cellTarget;
            cellTarget.gameObject.SetActive(false);
        }

        private void RenderBackgroundGrid()
        {
            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                for (int y = 0; y < _grid.GetLength(1); y++)
                {
                    var background = cell.Create(GetPositionCell(x, y), backgroundBlock, backgroundSize,
                        CONSTANTS.CellType.Background);
                    // background.name = nameof(CONSTANTS.CellType.Background);
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
            var obstaclesTotal = 5;
            List<Utils.GridPos> cellPosList = new();
            var count = 0;

            while (cellPosList.Count < obstaclesTotal)
            {
                count++;
                var randomX = Random.Range(0, width);
                var randomY = Random.Range(0, height - 2);
                var cellPos = new Utils.GridPos(randomX, randomY);
                var isNoInList = cellPosList.IndexOf(cellPos) == -1;
                // var isNearByInList =
                //     (cellPosList.IndexOf(new CellPos(randomX + 1, randomY)) > -1 && randomX == width - 2) ||
                //     (cellPosList.IndexOf(new CellPos(randomX - 1, randomY)) > -1 && randomX == 1);
                if (isNoInList)
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
            List<Utils.GridPos> cells = new();
            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                for (int y = 0; y < _grid.GetLength(1); y++)
                {
                    cells.Add(new Utils.GridPos(x, y, _grid[x, y].Type));
                }
            }

            cells.Reverse();
            cellTypes = cells;
        }
    }
}