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

            // LevelsData.Register(SaveLevelsData);
            LevelsData.SetValueWithoutEvent(LoadLevelsData());
        }

        public void SaveLevelsData()
        {
            var storage = this.GetUtility<IGameStorage>();
            var json = JsonUtility.ToJson(new Utils.JsonHelper<Utils.LevelData>(LevelsData.Value));

            storage.SaveString(nameof(LevelsData), json);
        }

        private List<Utils.LevelData> LoadLevelsData()
        {
            var storage = this.GetUtility<IGameStorage>();
            var json = storage.LoadString(nameof(LevelsData));

            var levelsData = string.IsNullOrEmpty(json)
                ? new List<Utils.LevelData>()
                : JsonUtility.FromJson<Utils.JsonHelper<Utils.LevelData>>(json).ListData;

            return levelsData;
        }

        public void ResetValueTextUI()
        {
            ScoreTotal.Value = 0;
            StepsTotal.Value = Utils.GetStepsMove(LevelSelect.Value);
            ObstaclesTotal.Value = Utils.GetObstaclesTotal(LevelSelect.Value);
        }
    }
}