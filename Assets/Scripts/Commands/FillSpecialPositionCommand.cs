using QFramework;
using Queries;
using UnityEngine;

namespace Commands
{
    public class FillSpecialPositionCommand : AbstractCommand
    {
        private Cell[,] _grid;

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
                    var random = Random.Range(3, 9);
                    cell.Type = (CONSTANTS.CellType)random;
                    cell.ReSetAvatar();
                }
            }
        }
    }
}