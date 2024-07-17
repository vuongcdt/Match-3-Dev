using System.Collections.Generic;
using GameControllers;
using QFramework;

namespace Interfaces
{
    public interface IGameModel : IModel
    {
        BindableProperty<Cell[,]> GridArray { get; set; }
        public BindableProperty<int> StepsTotal { get; set; } 
        public BindableProperty<int> ObstaclesTotal { get; set; } 
        public BindableProperty<int> ScoreTotal { get; set; }
        public BindableProperty<int> LevelSelect { get; set; }
        public BindableProperty<int> StarsTotal { get; set; }
        public BindableProperty<string> _levelsData{ get; set; }
        public List<Utils.LevelData> LevelsData { get; set; }
        public BindableProperty<float> MusicSetting { get; set; }
        public BindableProperty<float> SfxSetting { get; set; }
        
        BindableProperty<int> Count { get; }
    }
}