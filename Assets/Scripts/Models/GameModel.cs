using System.Collections.Generic;
using GameControllers;
using Interfaces;
using QFramework;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.VisualScripting;
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
        public BindableProperty<int> StarsTotal { get; set; } = new(0);
        
        private BindableProperty<string> _levelsData;
        public BindableProperty<List<Utils.LevelData>> LevelsData { get; set; } = new(new List<Utils.LevelData>());
        public BindableProperty<float> MusicSetting { get; set; } = new(0.5f);
        public BindableProperty<float> SfxSetting { get; set; } = new(0.5f);

        public BindableProperty<int> Count { get; } = new();

        protected override void OnInit()
        {
            var storage = this.GetUtility<IGameStorage>();

            Count.Register(newCount => storage.SaveInt(nameof(Count), newCount));
            Count.SetValueWithoutEvent(storage.LoadInt(nameof(Count)));

            var userDataJson = JsonUtility.ToJson(new Utils.JsonHelper<Utils.LevelData>(LevelsData.Value));
            LevelsData.Register(value =>
            {
                Debug.Log($"UserData {LevelsData.Value.Count}");
                Debug.Log($"userDataJson {userDataJson}");
                storage.SaveString(nameof(LevelsData), userDataJson);
            });
            
            List<Utils.LevelData> userData = new();
            var userDataString = storage.LoadString(nameof(LevelsData));
            
            if (!string.IsNullOrEmpty(userDataString))
            {
                Utils.JsonHelper<Utils.LevelData> jsonHelper =
                    JsonUtility.FromJson<Utils.JsonHelper<Utils.LevelData>>(userDataString);
                userData = jsonHelper.ListData;
            }
            LevelsData.SetValueWithoutEvent(userData);
        }
    }
}