using System.Collections.Generic;
using Interfaces;
using QFramework;
using UnityEngine;

namespace Commands
{
    public class InitSettingsGridModelCommand : AbstractCommand
    {
        private int _width;
        private int _height;
        private float _cellSize;
        private Cell cell;
        private Transform backgroundBlock;
        private Transform gridBlock;
        private Vector2[] obstacles;
        private float avatarSize;
        private float backgroundSize;
        private float fillTime;
        private List<Utils.GridPos> cellTypes;
        private bool isProcessing;

        private Cell[,] _grid;
        private bool _isRevertFill;

        public InitSettingsGridModelCommand(int width, int height, float cellSize)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
        }

        protected override void OnExecute()
        {
            this.GetModel<IGameModel>().SettingsGrid.Value = new Utils.SettingsGrid(_width, _height, _cellSize);
        }
    }
}