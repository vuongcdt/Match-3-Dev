using BaseScripts;
using QFramework;

namespace Interfaces
{
    public interface IGameModel : IModel
    {
        BindableProperty<Cell[,]> GridArray { get; set; }
        BindableProperty<Utils.SettingsGrid> SettingsGrid { get; }
        BindableProperty<int> Count { get; }
    }
}