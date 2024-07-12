using GameControllers;
using QFramework;
using Queries;
using UnityEngine;

namespace Commands
{
    public class RenderCellGridCommand : AbstractCommand
    {
        private Cell[,] _grid;
        private ConfigGame _configGame;
        private static readonly int DefaultAnimator = Animator.StringToHash("Default");

        protected override void OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            _configGame = ConfigGame.Instance;
            RenderCellGrid();
        }

        private void RenderCellGrid()
        {
            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                for (int y = 0; y < _grid.GetLength(1); y++)
                {
                    var cell = _grid[x, y];
                    if (cell is null)
                    {
                        _configGame.Cell.Type = CONSTANTS.CellType.None;

                        var newCell = Pool.Instance.Create(
                            new Utils.GridPos(x, y),
                            _configGame.GridBlock,
                            _configGame.AvatarSize,
                            CONSTANTS.CellType.None);

                        _grid[x, y] = newCell;
                    }
                    else
                    {
                        cell.Type = CONSTANTS.CellType.None;
                        cell.GetComponentInChildren<Animator>().SetTrigger(DefaultAnimator);
                        cell.SpecialType = CONSTANTS.CellSpecialType.Normal;
                    }
                }
            }
        }
    }
}