using GameControllers;
using QFramework;
using Queries;
using UnityEngine;

namespace Commands
{
    public class FillSpecialPositionCommand : AbstractCommand
    {
        private Cell[,] _grid;
        private ConfigGame _configGame;

        protected override void OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            FillSpecialPosition();
        }

        private void FillSpecialPosition()
        {
            foreach (var cell in _grid)
            {
                if (cell.Type == CONSTANTS.CellType.None)
                {
                    var random = Random.Range(3, _configGame.MaxListImage);
                    cell.Type = (CONSTANTS.CellType)random;
                }
            }
        }
    }
}