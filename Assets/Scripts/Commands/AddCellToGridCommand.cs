using GameControllers;
using QFramework;
using Queries;
using UnityEngine;

namespace Commands
{
    public class AddCellToGridCommand : AbstractCommand
    {
        private Cell[,] _grid;
        private static readonly int DefaultAnimator = Animator.StringToHash("Default");

        protected override void OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            AddCellToGrid();
        }

        private void AddCellToGrid()
        {
            ConfigGame configGame = ConfigGame.Instance;
            for (int x = 0; x < configGame.Width; x++)
            {
                var cellBelow = _grid[x, configGame.Height - 1];
                
                if (cellBelow.Type == CONSTANTS.CellType.None)
                {
                    var random = Random.Range(3, configGame.MaxListImage);
                    configGame.IsProcessing = true;
                    
                    var newCell = configGame.Cell.Create(
                        new Utils.GridPos(x, configGame.Height),
                        configGame.GridBlock,
                        configGame.AvatarSize,
                        (CONSTANTS.CellType)random);

                    newCell.GridPosition = new Utils.GridPos(x, configGame.Height - 1);
                    _grid[x, configGame.Height - 1] = newCell;
                    
                    ReturnPool(cellBelow, configGame);
                }
            }
        }

        private static void ReturnPool(Cell cellBelow, ConfigGame configGame)
        {
            cellBelow.GetComponentInChildren<Animator>().SetTrigger(DefaultAnimator);
            cellBelow.SpecialType = CONSTANTS.CellSpecialType.Normal;
            configGame.Pool.Push(cellBelow);
        }
    }
}