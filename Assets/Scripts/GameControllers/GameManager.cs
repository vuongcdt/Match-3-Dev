using BaseScripts;
using Commands;
using QFramework;
using Queries;
using UnityEngine;

namespace GameControllers
{
    public class GameManager : MonoBehaviour, IController
    {
        [SerializeField] private Cell cell;
        [SerializeField] private Transform backgroundBlock;
        [SerializeField] private Transform gridBlock;
        [SerializeField] private Vector2[] obstacles;
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private float cellSize;
        [SerializeField] private float fillTime;
        
        private Cell[,] _grid;

        private void Start()
        {
            this.SendCommand(new InitGridCommand(width, height, obstacles));
            _grid = this.SendQuery(new GetGridQuery());
            _grid = new Cell[width, height];
            RenderBackgroundGrid();
            RenderCellGrid();
            Shoot();
        }

        private void Shoot()
        {
            for (int x = 0; x < width; x++)
            {
                if (_grid[x, height - 1].Type == CONSTANTS.CellType.None)
                {
                    cell.Type = CONSTANTS.CellType.Normal;
                    var newCell = cell.Create(GetPositionCell(x, height), gridBlock, 
                        cellSize * 0.6f, CONSTANTS.CellType.Normal);
                    newCell.Move(GetPositionCell(x, height - 2), fillTime);
                }
            }
        }

        private void RenderBackgroundGrid()
        {
            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                for (int y = 0; y < _grid.GetLength(1); y++)
                {
                    cell.Type = CONSTANTS.CellType.Background;
                    cell.Create(GetPositionCell(x, y), backgroundBlock, cellSize * 0.9f,CONSTANTS.CellType.Background);
                }
            }
        }
        
        private void RenderCellGrid()
        {
            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                for (int y = 0; y < _grid.GetLength(1); y++)
                {
                    cell.Type = CONSTANTS.CellType.None;
                    var newCell = cell.Create(GetPositionCell(x, y), gridBlock, cellSize * 0.6f,CONSTANTS.CellType.None);
                    _grid[x, y] = newCell;
                }
            }
            
            foreach (var obstacle in obstacles)
            {
                _grid[(int)obstacle.x,(int)obstacle.y].Type = CONSTANTS.CellType.Obstacle;
                _grid[(int)obstacle.x, (int)obstacle.y].ReSetAvatar();
            }
        }

        private Vector2 GetPositionCell(int x, int y)
        {
            return new Vector2(x - (width - 1) * 0.5f, y - (height - 1) * 0.5f) * cellSize;
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}