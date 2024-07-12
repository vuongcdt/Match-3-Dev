using GameControllers;
using Interfaces;
using QFramework;

namespace Commands
{
    public class InitGridModelCommand:AbstractCommand
    {
        private Cell[,] _grid;

        public InitGridModelCommand(Cell[,] grid)
        {
            _grid = grid;
        }

        protected override void OnExecute()
        {
            this.GetModel<IGameModel>().GridArray = new BindableProperty<Cell[,]>(_grid);
        }
    }
}