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

            _configGame.ObstaclesTotal = obstacleGridPosList.Count;
            _configGame.ObstaclesTotalText.text = obstacleGridPosList.Count.ToString();
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
            int count = 0;
            List<Utils.GridPos> obstacleGridPosList = new();

            var isNotNextTo = Random.value > 0.5f;

            // var randomObstaclesTotal = Random.Range(15, 22);
            // var randomObstaclesTotal = _configGame.ObstaclesTotal;
            var randomObstaclesTotal = 15;

            while (obstacleGridPosList.Count < randomObstaclesTotal)
            {
                count++;
                var randomX = Random.Range(0, _configGame.Width);
                var randomY = Random.Range(0, _configGame.Height - 1);

                var cellPos = new Utils.GridPos(randomX, randomY);
                var isNoInList = obstacleGridPosList.IndexOf(cellPos) == -1;
                
                var isNextTo = IsNextTo(obstacleGridPosList, cellPos);
                if (isNoInList && !isNextTo && isNotNextTo)
                {
                    obstacleGridPosList.Add(cellPos);
                }   
                
                if (isNoInList && !isNotNextTo)
                {
                    obstacleGridPosList.Add(cellPos);
                }

                if (count > 100)
                {
                    Debug.Log("loop");
                    break;
                }
            }

            // Debug.Log($"count {count}");
            return obstacleGridPosList;
        }

        private static bool IsNextTo(List<Utils.GridPos> obstacleGridPosList, Utils.GridPos cellPos)
        {
            bool isNextTo = false;
            foreach (var gridPos in obstacleGridPosList)
            {
                var isSameRow = Mathf.Abs(cellPos.x - gridPos.x) == 1 && cellPos.y == gridPos.y;
                var isSameColumn = Mathf.Abs(cellPos.y - gridPos.y) == 1 && cellPos.x == gridPos.x;
                if (isSameRow || isSameColumn)
                {
                    isNextTo = true;
                    break;
                }
            }

            return isNextTo;
        }
    }
}