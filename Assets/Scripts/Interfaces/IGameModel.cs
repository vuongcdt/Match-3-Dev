using BaseScripts;
using QFramework;

namespace Interfaces
{
    public interface IGameModel
    {
        public BindableProperty<int[,]> GridArray { get; }
        public BindableProperty<CellBase[,]> CellArray { get; }
    }
}