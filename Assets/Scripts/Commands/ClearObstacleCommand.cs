using GameControllers;
using QFramework;
using Queries;

namespace Commands
{
    public class ClearObstacleCommand : AbstractCommand
    {
        private Cell[,] _grid;
        private ConfigGame _configGame;
        private int _x;
        private int _y;

        public ClearObstacleCommand(int x, int y)
        {
            _x = x;
            _y = y;
        }

        protected override void OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            _configGame = ConfigGame.Instance;

            ClearObstacles(_x, _y);
        }

        private void ClearObstacles(int x, int y)
        {
            for (int newX = x - 1; newX <= x + 1; newX++)
            {
                if (newX == x || newX < 0 || newX >= _configGame.Width)
                {
                    continue;
                }

                var obstacle = _grid[newX, y];
                RemoveObstacle(obstacle);
            }

            for (int newY = y - 1; newY <= y + 1; newY++)
            {
                if (newY == y || newY < 0 || newY >= _configGame.Height)
                {
                    continue;
                }

                var obstacle = _grid[x, newY];
                RemoveObstacle(obstacle);
            }
        }

        private void RemoveObstacle(Cell obstacle)
        {
            if (obstacle.Type == CONSTANTS.CellType.Obstacle)
            {
                obstacle.Type = CONSTANTS.CellType.None;
                _configGame.ObstaclesTotal--;
                _configGame.ObstaclesTotalText.text = _configGame.ObstaclesTotal.ToString();
            }
        }
    }
}