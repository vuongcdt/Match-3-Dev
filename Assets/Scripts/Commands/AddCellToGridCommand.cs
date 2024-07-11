using System.Collections;
using Events;
using GameControllers;
using QFramework;
using Queries;
using UnityEngine;

namespace Commands
{
    public class AddCellToGridCommand : AbstractCommand<bool>
    {
        private Cell[,] _grid;
        private static readonly int DefaultAnimator = Animator.StringToHash("Default");

        protected override bool OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            return AddCellToGrid();
        }

        private bool AddCellToGrid()
        {
            var isAdd = false;
            ConfigGame configGame = ConfigGame.Instance;
            for (int x = 0; x < configGame.Width; x++)
            {
                var cellBelow = _grid[x, configGame.Height - 1];
                
                if (cellBelow.Type == CONSTANTS.CellType.None)
                {
                    isAdd = true;
                    var random = Random.Range(3, configGame.MaxListImage);
                    configGame.IsProcessing = true;
                    
                    var newCell = configGame.Cell.Create(
                        new Utils.GridPos(x, configGame.Height),
                        configGame.GridBlock,
                        configGame.AvatarSize,
                        (CONSTANTS.CellType)random);

                    _grid[x, configGame.Height - 1] = newCell;
                    newCell.GridPosition = new Utils.GridPos(x, configGame.Height - 1);
                    
                    ReturnPool(cellBelow, configGame);
                }
            }

            // yield return new WaitForSeconds(configGame.FillTime);
            return isAdd;
            // if (isAdd)
            // {
            //     this.SendEvent<ProcessingGridEvent>();
            // }
            // else
            // {
            //     this.SendCommand(new MatchGridCommand());
            // }
        }

        private static void ReturnPool(Cell cellBelow, ConfigGame configGame)
        {
            cellBelow.GetComponentInChildren<Animator>().SetTrigger(DefaultAnimator);
            cellBelow.SpecialType = CONSTANTS.CellSpecialType.Normal;
            configGame.Pool.Push(cellBelow);
        }
    }
}