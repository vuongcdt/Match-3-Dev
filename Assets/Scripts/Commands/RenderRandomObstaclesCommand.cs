using System.Collections.Generic;
using GameControllers;
using QFramework;
using Queries;
using UnityEngine;

namespace Commands
{
    public class RenderRandomObstaclesCommand : AbstractCommand
    {
        private Cell[,] _grid;
        private ConfigGame _configGame;

        protected override void OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            _configGame = ConfigGame.Instance;
            RenderRandomObstacles();
        }

        private void RenderRandomObstacles()
        {
            var obstacleGridPosList = RandomObstacleGridPosList();

            foreach (var gridPos in obstacleGridPosList)
            {
                if (gridPos.x < 0 || gridPos.x > _configGame.Width - 1 || gridPos.y < 0 ||
                    gridPos.y > _configGame.Height - 1)
                {
                    continue;
                }

                _grid[gridPos.x, gridPos.y].Type = CONSTANTS.CellType.Obstacle;
                _grid[gridPos.x, gridPos.y].name = CONSTANTS.CellType.Obstacle.ToString();
                _grid[gridPos.x, gridPos.y].GridPosition = gridPos;
            }
        }

        private List<Utils.GridPos> RandomObstacleGridPosList()
        {
            List<Utils.GridPos> obstacleGridPosList = new();
            var count = 0;
            var randomObstaclesTotal = Random.Range(5, 20);

            // while (obstacleGridPosList.Count < _configGame.ObstaclesTotal)
            while (obstacleGridPosList.Count < randomObstaclesTotal)
            {
                count++;
                var randomX = Random.Range(0, _configGame.Width);
                var randomY = Random.Range(0, _configGame.Height - 2);
                var cellPos = new Utils.GridPos(randomX, randomY);
                // var cellPos = new Utils.GridPos(randomX, 3);
                var isNoInList = obstacleGridPosList.IndexOf(cellPos) == -1;

                if (isNoInList)
                {
                    obstacleGridPosList.Add(cellPos);
                }
            }

            return obstacleGridPosList;
        }
    }
}