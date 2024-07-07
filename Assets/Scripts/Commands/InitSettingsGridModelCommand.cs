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
        private float _fillTime;
        private List<Utils.GridPos> cellTypes;

        public InitSettingsGridModelCommand(int width, int height, float cellSize, float fillTime)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _fillTime = fillTime;
        }

        protected override void OnExecute()
        {
            this.GetModel<IGameModel>().SettingsGrid.Value = new Utils.SettingsGrid(_width, _height, _cellSize, _fillTime);
            this.GetModel<IGameModel>().IsRevertFill.Value = false;
            this.GetModel<IGameModel>().IsProcessing.Value = false;
        }
    }
}