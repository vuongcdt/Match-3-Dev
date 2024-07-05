using BaseScripts;
using QFramework;
using UnityEngine;
using Grid = BaseScripts.Grid;

namespace Commands
{
    public class InitGridCommand : AbstractCommand
    {
        private CellBase _cell;
        private Grid _celGrid;
        private Sprite _backgroundSprite;
        private Sprite _obstacleSprite;
        private Transform _backgroundBlock;
        private Transform _gridBlock;
        private Vector2[] _obstacles;

        protected override void OnExecute()
        {
            // var background = new Grid(10, 8, 1.2f, _cell, _backgroundBlock, _backgroundSprite, 0.9f);
            _celGrid = new Grid(10, 8, 1.2f, _cell, _gridBlock, 0.6f);

            foreach (var obstacle in _obstacles)
            {
                _celGrid.SetValue((int)obstacle.x, (int)obstacle.y, _obstacleSprite, CONSTANTS.CellType.Obstacle);
            }
        }
    }
}