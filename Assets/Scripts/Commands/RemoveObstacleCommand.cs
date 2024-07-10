using GameControllers;
using QFramework;
using Queries;

namespace Commands
{
    public class RemoveObstacleCommand : AbstractCommand
    {
        private Cell[,] _grid;
        private ConfigGame _configGame;
        private int _x;
        private int _y;

        public RemoveObstacleCommand(int x, int y)
        {
            _x = x;
            _y = y;
        }

        protected override void OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            _configGame = ConfigGame.Instance;

            RemoveObstacle(_x, _y);
        }

        private void RemoveObstacle(int x, int y)
        {
            for (int index = x - 1; index <= x + 1; index++)
            {
                if (index == x || index < 0 || index >= _configGame.Width)
                {
                    continue;
                }

                var obstacle = _grid[index, y];
                if (obstacle.Type == CONSTANTS.CellType.Obstacle)
                {
                    obstacle.Type = CONSTANTS.CellType.None;
                }
            }

            for (int index = y - 1; index <= y + 1; index++)
            {
                if (index == y || index < 0 || index >= _configGame.Height)
                {
                    continue;
                }

                var obstacle = _grid[x, index];
                if (obstacle.Type == CONSTANTS.CellType.Obstacle)
                {
                    obstacle.Type = CONSTANTS.CellType.None;
                }
            }
        }
    }
}