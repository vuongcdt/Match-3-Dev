using System.Collections;
using QFramework;
using Queries;
using UnityEngine;

namespace Commands
{
    public class FillCommandIE : AbstractCommand<IEnumerator>
    {
        private Cell[,] _grid;
        private ConfigGame _configGame;

        protected override IEnumerator OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            _configGame = ConfigGame.Instance;

            return FillIE();
        }

        private IEnumerator FillIE()
        {
            for (int y = _configGame.Height - 1; y > 0; y--)
            {
                for (int x = 0; x < _configGame.Width; x++)
                {
                    CheckFill(x, y);
                }

                _configGame.IsRevertFill = !_configGame.IsRevertFill;
                
                yield return new WaitForSeconds(_configGame.FillTime);
            }
        }

        private void CheckFill(int x, int y)
        {
            int[] checkArr = _configGame.IsRevertFill
                ? new[] { 0, 1, -1 }
                : new[] { 0, -1, 1 };

            foreach (var index in checkArr)
            {
                if (x + index < 0 || x + index >= _configGame.Width)
                {
                    continue;
                }

                var source = _grid[x, y];
                var target = _grid[x + index, y - 1];

                var isSourceFish = source.Type != CONSTANTS.CellType.None &&
                                   source.Type != CONSTANTS.CellType.Obstacle;
                var isTargetEmpty = target.Type == CONSTANTS.CellType.None;
                var isNextToObstacle = _grid[x + index, y].Type == CONSTANTS.CellType.Obstacle;

                var isSpecial = y + 1 < _configGame.Height &&
                                // _grid[x + index, y + 1].Type == CONSTANTS.CellType.Obstacle &&
                                _grid[x, y + 1].Type == CONSTANTS.CellType.Obstacle;

                if (index != 0 && !isNextToObstacle && !isSpecial)
                {
                    continue;
                }

                if (isSourceFish && isTargetEmpty)
                {
                    MoveToBelow(source, target, x, y, index);
                    break;
                }

                if (index != 0 && isSourceFish && isSpecial && _grid[x + index, y].Type == CONSTANTS.CellType.None)
                {
                    target = _grid[x + index, y];
                    MoveToNextTo(source, target, x, y, index);
                    break;
                }
            }
        }

        private void MoveToNextTo(Cell cellSource, Cell cellTarget, int x, int y, int index)
        {
            _configGame.IsProcessing = true;

            cellSource.Move(
                Utils.GetPositionCell(x + index, y, _configGame.Width, _configGame.Height, _configGame.CellSize),
                _configGame.FillTime);

            _grid[x + index, y] = cellSource;
            _grid[x, y] = cellTarget;

            cellTarget.gameObject.SetActive(false);
        }

        private void MoveToBelow(Cell cellSource, Cell cellTarget, int x, int y, int index = 0)
        {
            _configGame.IsProcessing = true;

            cellSource.Move(
                Utils.GetPositionCell(x + index, y - 1, _configGame.Width, _configGame.Height, _configGame.CellSize),
                _configGame.FillTime);

            _grid[x + index, y - 1] = cellSource;
            _grid[x, y] = cellTarget;

            cellTarget.gameObject.SetActive(false);
        }
    }
}