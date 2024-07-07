using Interfaces;
using QFramework;

namespace Models
{
    public class GameModel : AbstractModel, IGameModel
    {
        public BindableProperty<Cell[,]> GridArray { get; set; } = new();
        public BindableProperty<Utils.SettingsGrid> SettingsGrid { get; } = new();
        public BindableProperty<bool> IsRevertFill { get; set; } = new();
        public BindableProperty<bool> IsProcessing { get; set; } = new();
        public BindableProperty<int> Count { get; } = new();

        protected override void OnInit()
        {
            var storage = this.GetUtility<IGameStorage>();

            Count.SetValueWithoutEvent(storage.LoadInt(nameof(Count)));
            Count.Register(newCount => storage.SaveInt(nameof(Count), newCount));
        }
    }
}