using QFramework;
using Queries;

namespace Commands
{
    public class RenderCellGridCommand : AbstractCommand
    {
        private Cell[,] _grid;
        private ConfigGame _configGame;

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
                    _configGame.Cell.Type = CONSTANTS.CellType.None;
                    
                    var newCell = _configGame.Cell.Create(
                        new Utils.GridPos(x,y),
                        _configGame.GridBlock,
                        _configGame.AvatarSize,
                        CONSTANTS.CellType.None);
                    
                    _grid[x, y] = newCell;
                }
            }
        }
    }
}