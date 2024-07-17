using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Events;
using Interfaces;
using QFramework;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core.Screens;
using Screen = ZBase.UnityScreenNavigator.Core.Screens.Screen;

namespace UIGame.Scripts
{
    public class HomeScreen : Screen, IController, ICanSendEvent
    {
        [SerializeField] private CardItem[] cardItems;

        private IGameModel _gameModel;

        public override UniTask Initialize(Memory<object> args)
        {
            base.OnEnable();
           

            this.RegisterEvent<LevelSelectEvent>(e => OnClickPlayLevel(e.Level).Forget())
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            
            // _gameModel._levelsData.RegisterWithInitValue(value => GetUserData());
            // _gameModel.LevelsData.RegisterWithInitValue(value => GetUserData());

            Debug.Log("Initialize HomeScreen");

            return UniTask.CompletedTask;
        }

        protected override void OnEnable()
        {
            Debug.Log("OnEnable");
            base.OnEnable();
            _gameModel = this.GetModel<IGameModel>();
            GetUserData();
        }

        private void GetUserData()
        {
            Debug.Log("GetUserData");
            var userData = _gameModel.LevelsData.Value ?? new List<Utils.LevelData>();

            for (var index = 0; index <= userData.Count; index++)
            {
                if (index >= cardItems.Length)
                {
                    break;
                }

                var cardItem = cardItems[index];

                if (index == userData.Count)
                {
                    cardItem.SetCardItem(CONSTANTS.TypeCard.Checking, userData.Count + 1, 0);
                    break;
                }

                var dataLevel = userData[index];

                cardItem.SetCardItem(CONSTANTS.TypeCard.Checked, dataLevel.Level, dataLevel.Star);
            }

            for (var index = userData.Count; index < cardItems.Length; index++)
            {
                cardItems[index].SetLevelCardItem(index + 1);
            }
        }

        private async UniTask OnClickPlayLevel(int level)
        {
            _gameModel.LevelSelect.Value = level;
            _gameModel.ResetValueTextUI();

            await ScreenContainer.Of(transform).PushAsync(new ScreenOptions(ResourceKey.PlayScreenPrefab()));
            this.SendEvent(new InitLevelEvent(level));
        }

        // private void ResetValueUI()
        // {
        //     _gameModel.ScoreTotal.Value = 0;
        //     _gameModel.StepsTotal.Value = Utils.GetStepsMove(_gameModel.LevelSelect.Value);
        //     _gameModel.ObstaclesTotal.Value = Utils.GetObstaclesTotal(_gameModel.LevelSelect.Value);
        // }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}