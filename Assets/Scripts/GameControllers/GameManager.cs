using System.Collections;
using Commands;
using Events;
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
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private float cellSize;
        [SerializeField] private float avatarSize;
        [SerializeField] private float backgroundSize;
        [SerializeField] private float fillTime;
        [SerializeField] private bool isProcessing;

        private Cell[,] _grid;
        private bool _isRevertFill;

        private void Start()
        {
            this.SendCommand(new InitSettingsGridModelCommand(width, height, cellSize));
            _grid = new Cell[width, height];

            this.RegisterEvent<ProcessingEvent>(e => { StartCoroutine(ProcessingIE()); })
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.SendCommand(new InitGridModelCommand(_grid));
            this.SendCommand(new RenderBackgroundGridCommand(cell, cellSize, backgroundBlock, backgroundSize));
            this.SendCommand(new RenderCellGridCommand(cell, cellSize, gridBlock, avatarSize));
            this.SendCommand<RenderRandomObstaclesCommand>();
            
            StartCoroutine(ProcessingIE());
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

            this.SendCommand<MatchGridCommand>();
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
                    var newCell = cell.Create(
                        Utils.GetPositionCell(x, height, width, height, cellSize),
                        gridBlock,
                        avatarSize,
                        (CONSTANTS.CellType)random);
                    newCell.Move(Utils.GetPositionCell(x, height - 1, width, height, cellSize), fillTime);
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
                var isNextToObstacle = _grid[x + index, y].Type == CONSTANTS.CellType.Obstacle;
                if (index != 0 && !isNextToObstacle)
                {
                    continue;
                }

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
            cellSource.Move(Utils.GetPositionCell(x + index, y - 1, width, height, cellSize), fillTime);
            _grid[x + index, y - 1] = cellSource;
            _grid[x, y] = cellTarget;
            cellTarget.gameObject.SetActive(false);
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}