using System.Collections.Generic;
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
            List<Utils.GridPos> cellPosList = new();
            var count = 0;

            while (cellPosList.Count < _configGame.ObstaclesTotal)
            {
                count++;
                var randomX = Random.Range(0, _configGame.Width);
                var randomY = Random.Range(0, _configGame.Height - 2);
                var cellPos = new Utils.GridPos(randomX, randomY);
                // var cellPos = new Utils.GridPos(randomX, 3);
                var isNoInList = cellPosList.IndexOf(cellPos) == -1;

                if (isNoInList)
                {
                    cellPosList.Add(cellPos);
                }
            }

            foreach (var obstacle in cellPosList)
            {
                if (obstacle.x < 0 || obstacle.x > _configGame.Width - 1 || obstacle.y < 0 ||
                    obstacle.y > _configGame.Height - 1)
                {
                    continue;
                }

                _grid[obstacle.x, obstacle.y].Type = CONSTANTS.CellType.Obstacle;
                // _grid[obstacle.x, obstacle.y].ReSetAvatar();
            }
        }
    }
}