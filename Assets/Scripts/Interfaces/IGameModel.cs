using BaseScripts;
using QFramework;

namespace Interfaces
{
    public interface IGameModel : IModel
    {
        BindableProperty<Cell[,]> GridArray { get; }
        BindableProperty<int> Count { get; }
    }
}