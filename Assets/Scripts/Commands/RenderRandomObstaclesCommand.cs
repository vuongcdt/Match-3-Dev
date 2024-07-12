using System.Collections.Generic;
using GameControllers;
using QFramework;
using Queries;
using UnityEngine;
using CellType = CONSTANTS.CellType;
using CellSpecialType = CONSTANTS.CellSpecialType;

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
            RenderTestCell();
        }

        private void RenderTestCell()
        {
            List<CellTest> cellList = new()
            {
                new CellTest(0,0,CellType.Blue),
                new CellTest(0,1,CellType.Blue),
                new CellTest(0,2,CellType.Red),
                new CellTest(0,3,CellType.Blue),  
                
                new CellTest(1,0,CellType.Yellow,CellSpecialType.Row),
                // new CellTest(1,0,CellType.Yellow),
                new CellTest(1,1,CellType.Yellow),
                new CellTest(1,2,CellType.Blue),
                new CellTest(1,3,CellType.Yellow),
                
                new CellTest(2,0,CellType.Yellow),
                new CellTest(3,0,CellType.Red),
                new CellTest(4,0,CellType.Yellow),
                new CellTest(5,0,CellType.Obstacle),
                // new CellTest(5,0,CellType.Yellow),
                new CellTest(6,0,CellType.Rainbow),
                
                new CellTest(2,1,CellType.Obstacle),
                new CellTest(3,1,CellType.Yellow),
                
                new CellTest(3,2,CellType.Obstacle),
            };
            
            foreach (var cellTest in cellList)
            {
                var cell = _grid[cellTest.GridPos.x, cellTest.GridPos.y];
                
                cell.Type = cellTest.CellType;
                cell.SpecialType = cellTest.SpecialType;
                
                cell.name = cellTest.CellType.ToString();
                cell.GridPosition = cellTest.GridPos;
                
                cell.GetComponent<BoxCollider2D>().enabled = true;
            }
        }

        private class CellTest
        {
            public Utils.GridPos GridPos;
            public CellType CellType;
            public CellSpecialType SpecialType;

            public CellTest(int x,int y, CellType cellType,
                CellSpecialType specialType = 0)
            {
                GridPos = new Utils.GridPos(x,y);
                CellType = cellType;
                SpecialType = specialType;
            }
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

                _grid[gridPos.x, gridPos.y].Type = CellType.Obstacle;
                _grid[gridPos.x, gridPos.y].name = CellType.Obstacle.ToString();
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