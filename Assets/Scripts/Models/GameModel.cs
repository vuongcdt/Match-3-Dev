using BaseScripts;
using Interfaces;
using QFramework;

namespace Models
{
    public class GameModel : AbstractModel, IGameModel
    {
        public BindableProperty<int[,]> GridArray { get; } = new();
        public BindableProperty<CellBase[,]> CellArray { get; } = new();

        protected override void OnInit()
        {
            var storage = this.GetUtility<IStorage>();
            // Count.SetValueWithoutEvent(storage.LoadInt(nameof(Count)));
            // Count.Register(newCount => storage.SaveInt(nameof(Count), newCount));
        }
    }
}