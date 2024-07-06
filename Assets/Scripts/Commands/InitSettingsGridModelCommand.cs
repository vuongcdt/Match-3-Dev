using Interfaces;
using QFramework;

namespace Commands
{
    public class InitSettingsGridModelCommand : AbstractCommand
    {
        private int _width;
        private int _height;
        private float _cellSize;

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