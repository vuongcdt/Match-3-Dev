using System.Collections.Generic;
using GameControllers;
using Interfaces;
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
        private IGameModel _gameModel;

        protected override void OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            _configGame = ConfigGame.Instance;
            _gameModel = this.GetModel<IGameModel>();
            RenderRandomObstacles();
            // RenderTestCell();
        }

        private void RenderTestCell()
        {
            List<CellTest> cellList = new()
            {
                new CellTest(0, 0, CellType.Blue),
                new CellTest(0, 1, CellType.Blue),
                new CellTest(0, 2, CellType.Red),
                new CellTest(0, 3, CellType.Blue),

                // new CellTest(1,0,CellType.Yellow,CellSpecialType.Row),
                new CellTest(1, 0, CellType.Yellow),
                new CellTest(1, 1, CellType.Yellow),
                new CellTest(1, 2, CellType.Blue),
                new CellTest(1, 3, CellType.Yellow),

                new CellTest(2, 0, CellType.Yellow),
                new CellTest(3, 0, CellType.Red),
                new CellTest(4, 0, CellType.Yellow),
                new CellTest(5, 0, CellType.Obstacle),
                // new CellTest(5,0,CellType.Yellow),
                new CellTest(6, 0, CellType.Rainbow),

                new CellTest(2, 1, CellType.Obstacle),
                new CellTest(3, 1, CellType.Yellow),

                new CellTest(3, 2, CellType.Yellow),
                new CellTest(3, 3, CellType.Obstacle),
                new CellTest(4, 1, CellType.Blue),
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

            public CellTest(int x, int y, CellType cellType,
                CellSpecialType specialType = 0)
            {
                GridPos = new Utils.GridPos(x, y);
                CellType = cellType;
                SpecialType = specialType;
            }
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

                var obstacle = _grid[gridPos.x, gridPos.y];
                obstacle.Type = CellType.Obstacle;
                obstacle.name = CellType.Obstacle.ToString();
                obstacle.GridPosition = gridPos;
                obstacle.GetComponent<BoxCollider2D>().enabled = false;
            }
        }

        private List<Utils.GridPos> RandomObstacleGridPosList()
        {
            int count = 0;
            List<Utils.GridPos> obstacleGridPosList = new();

            // var isNotNextTo = _configGame.Level % 2 == 0;
            var isNotNextTo = _gameModel.LevelSelect.Value % 2 == 0;
            var randomObstaclesTotal = Utils.GetObstaclesTotal(_gameModel.LevelSelect.Value);

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
                    break;
                }
            }

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