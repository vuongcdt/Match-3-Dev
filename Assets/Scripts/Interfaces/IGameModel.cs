using GameControllers;
using QFramework;

namespace Interfaces
{
    public interface IGameModel : IModel
    {
        BindableProperty<Cell[,]> GridArray { get; set; }

        BindableProperty<int> Count { get; }
    }
}