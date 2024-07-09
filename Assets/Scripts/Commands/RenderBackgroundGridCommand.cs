using GameControllers;
using QFramework;
using Queries;

namespace Commands
{
    public class RenderBackgroundGridCommand : AbstractCommand
    {
        private Cell[,] _grid;
        private ConfigGame _configGame;

        protected override void OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            _configGame = ConfigGame.Instance;
            RenderBackgroundGrid();
        }

        private void RenderBackgroundGrid()
        {
            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                for (int y = 0; y < _grid.GetLength(1); y++)
                {
                    _configGame.Cell.Create(
                        new Utils.GridPos(x, y),
                        _configGame.BackgroundBlock,
                        _configGame.BackgroundSize,
                        CONSTANTS.CellType.Background);
                }
            }
        }
    }
}