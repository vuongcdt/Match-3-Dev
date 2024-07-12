using GameControllers;
using QFramework;
using Queries;

namespace Commands
{
    public class SetObstaclesTotalCommand : AbstractCommand
    {
        private ConfigGame _configGame;
        private Cell[,] _grid;
        

        protected override void OnExecute()
        {
            _configGame = ConfigGame.Instance;
            _grid = this.SendQuery(new GetGridQuery());

            int count = 0;
            foreach (var cell in _grid)
            {
                if (cell.Type == CONSTANTS.CellType.Obstacle)
                {
                    count++;
                }
            }

            _configGame.ObstaclesTotal = count;
            _configGame.ObstaclesTotalText.text = count.ToString();
        }
    }
}