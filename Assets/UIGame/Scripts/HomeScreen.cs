using System;
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
            _gameModel = this.GetModel<IGameModel>();

            this.RegisterEvent<LevelSelectEvent>(e => OnClickPlayLevel(e.Level))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            
            _gameModel._levelsData.RegisterWithInitValue(value => GetUserData());

            Debug.Log("Initialize HomeScreen");

            return UniTask.CompletedTask;
        }

        private void GetUserData()
        {
            var userData = _gameModel.LevelsData;

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

        private void OnClickPlayLevel(int level)
        {
            _gameModel.LevelSelect.Value = level;
            _gameModel.ResetValueTextUI();

            ScreenContainer.Of(transform).Push(new ScreenOptions(ResourceKey.PlayScreenPrefab()));
            this.SendEvent(new InitGridEvent(level));
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