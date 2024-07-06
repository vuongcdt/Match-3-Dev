using System.Collections.Generic;
using QFramework;
using Queries;
using UnityEngine;

namespace Commands
{
    public class RenderRandomObstaclesCommand:AbstractCommand
    {
        private Cell[,] _grid;
        private int _width;
        private int _height;
        
        protected override void OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            _width = _grid.GetLength(0);
            _height = _grid.GetLength(1);
            
            RenderRandomObstacles();
        }
        
        private void RenderRandomObstacles()
        {
            var obstaclesTotal = 5;
            List<Utils.GridPos> cellPosList = new();
            var count = 0;

            while (cellPosList.Count < obstaclesTotal)
            {
                count++;
                var randomX = Random.Range(0, _width);
                var randomY = Random.Range(0, _height - 2);
                var cellPos = new Utils.GridPos(randomX, randomY);
                var isNoInList = cellPosList.IndexOf(cellPos) == -1;
                // var isNearByInList =
                //     (cellPosList.IndexOf(new CellPos(randomX + 1, randomY)) > -1 && randomX == width - 2) ||
                //     (cellPosList.IndexOf(new CellPos(randomX - 1, randomY)) > -1 && randomX == 1);
                if (isNoInList)
                {
                    cellPosList.Add(cellPos);
                }

                if (count > 100)
                {
                    Debug.Log("VAR");
                    break;
                }
            }

            foreach (var obstacle in cellPosList)
            {
                if (obstacle.x < 0 || obstacle.x > _width - 1 || obstacle.y < 0 || obstacle.y > _height - 1)
                {
                    continue;
                }

                _grid[obstacle.x, obstacle.y].Type = CONSTANTS.CellType.Obstacle;
                _grid[obstacle.x, obstacle.y].ReSetAvatar();
            }
        }

    }
}