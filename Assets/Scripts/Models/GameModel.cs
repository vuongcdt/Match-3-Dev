using System.Collections.Generic;
using GameControllers;
using Interfaces;
using QFramework;
using UnityEngine;

namespace Models
{
    public class GameModel : AbstractModel, IGameModel
    {
        public BindableProperty<Cell[,]> GridArray { get; set; } = new();
        public BindableProperty<int> StepsTotal { get; set; } = new(-1);
        public BindableProperty<int> ObstaclesTotal { get; set; } = new(-1);
        public BindableProperty<int> ScoreTotal { get; set; } = new();
        public BindableProperty<int> LevelSelect { get; set; } = new(1);
        public BindableProperty<int> StarsTotal { get; set; } = new();
        public BindableProperty<List<Utils.LevelData>> LevelsData { get; set; } = new(new());
        public BindableProperty<float> MusicSetting { get; set; } = new(0.5f);
        public BindableProperty<float> SfxSetting { get; set; } = new(0.5f);

        public BindableProperty<int> Count { get; } = new();

        protected override void OnInit()
        {
            var storage = this.GetUtility<IGameStorage>();

            Count.Register(newCount => storage.SaveInt(nameof(Count), newCount));
            var loadInt = storage.LoadInt(nameof(Count));
            Count.SetValueWithoutEvent(loadInt);

            // _levelsData.Register(newValue =>
            // {
            //     storage.SaveString(nameof(_levelsData), newValue);
            // });
            //
            // var loadLevelsData = storage.LoadString(nameof(_levelsData));
            // _levelsData.SetValueWithoutEvent(loadLevelsData);
        }

        // private void SetLevelsData(List<Utils.LevelData> value)
        // {
        //     _levelsData.Value = JsonUtility.ToJson(new Utils.JsonHelper<Utils.LevelData>(value));
        // }
        //
        // private List<Utils.LevelData> GetLevelsData()
        // {
        //     var levelsData = string.IsNullOrEmpty(_levelsData.Value)
        //         ? new List<Utils.LevelData>()
        //         : JsonUtility.FromJson<Utils.JsonHelper<Utils.LevelData>>(_levelsData.Value).ListData;
        //     return levelsData;
        // }

        public void ResetValueTextUI()
        {
            ScoreTotal.Value = 0;
            StepsTotal.Value = Utils.GetStepsMove(LevelSelect.Value);
            ObstaclesTotal.Value = Utils.GetObstaclesTotal(LevelSelect.Value);
        }
    }
}