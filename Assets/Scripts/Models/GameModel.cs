using System.Collections.Generic;
using GameControllers;
using Interfaces;
using QFramework;

namespace Models
{
    public class GameModel : AbstractModel, IGameModel
    {
        public BindableProperty<Cell[,]> GridArray { get; set; } = new();
        public BindableProperty<int> StepsTotal { get; set; } = new(-1);
        public BindableProperty<int> ObstaclesTotal { get; set; } = new(-1);
        public BindableProperty<int> ScoreTotal { get; set; } = new();
        public BindableProperty<int> Level { get; set; } = new(1);
        public BindableProperty<int> StarsTotal { get; set; } = new(0);
        public BindableProperty<List<Utils.LevelData>> UserData { get; set; } = new(new List<Utils.LevelData>());
        public BindableProperty<float> MusicSetting { get; set; } = new(0.5f);
        public BindableProperty<float> SfxSetting { get; set; } = new(0.5f);

        public BindableProperty<int> Count { get; } = new();

        protected override void OnInit()
        {
            var storage = this.GetUtility<IGameStorage>();

            Count.SetValueWithoutEvent(storage.LoadInt(nameof(Count)));
            Count.Register(newCount => storage.SaveInt(nameof(Count), newCount));
        }
    }
}