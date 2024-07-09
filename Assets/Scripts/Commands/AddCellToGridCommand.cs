using GameControllers;
using QFramework;
using Queries;
using UnityEngine;

namespace Commands
{
    public class AddCellToGridCommand : AbstractCommand
    {
        private Cell[,] _grid;

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
                }
            }
        }
    }
}