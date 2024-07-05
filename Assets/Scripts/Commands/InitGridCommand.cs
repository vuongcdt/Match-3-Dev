using BaseScripts;
using Interfaces;
using Models;
using QFramework;
using UnityEngine;

namespace Commands
{
    public class InitGridCommand : AbstractCommand
    {
        private CONSTANTS.CellType[,] _gridArray;
        private Vector2[] _obstacles;
        private int _width;
        private int _height;

        public InitGridCommand(int width, int height,Vector2[] obstacles)
        {
            _obstacles = obstacles;
            _width = width;
            _height = height;
        }

        protected override void OnExecute()
        {
            // this.GetModel<IGameModel>().GridArray.Value = new CONSTANTS.CellType[_width, _height];
            // _gridArray = this.GetModel<IGameModel>().GridArray.Value;
            // for (int x = 0; x < _gridArray.GetLength(0); x++)
            // {
            //     for (int y = 0; y < _gridArray.GetLength(1); y++)
            //     {
            //         _gridArray[x, y] = CONSTANTS.CellType.None;
            //     }
            // }
            //
            // foreach (var obstacle in _obstacles)
            // {
            //     _gridArray[(int)obstacle.x, (int)obstacle.y] = CONSTANTS.CellType.Obstacle;
            // }
        }
    }
}