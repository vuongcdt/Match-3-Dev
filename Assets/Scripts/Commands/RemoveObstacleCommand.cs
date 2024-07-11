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
            for (int newX = x - 1; newX <= x + 1; newX++)
            {
                if (newX == x || newX < 0 || newX >= _configGame.Width)
                {
                    continue;
                }

                var obstacle = _grid[newX, y];
                if (obstacle.Type == CONSTANTS.CellType.Obstacle)
                {
                    obstacle.Type = CONSTANTS.CellType.None;
                    _configGame.ObstaclesTotal--;
                }
            }

            for (int newY = y - 1; newY <= y + 1; newY++)
            {
                if (newY == y || newY < 0 || newY >= _configGame.Height)
                {
                    continue;
                }

                var obstacle = _grid[x, newY];
                if (obstacle.Type == CONSTANTS.CellType.Obstacle)
                {
                    obstacle.Type = CONSTANTS.CellType.None;
                    _configGame.ObstaclesTotal--;
                }
            }
        }
    }
}