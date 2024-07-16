using System;
using System.Linq;
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
            Debug.Log("Initialize HomeScreen");
            _gameModel = this.GetModel<IGameModel>();

            this.RegisterEvent<LevelSelectEvent>(e => OnClickPlayLevel(e.Level))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<UserDataEvent>(e => GetUserData())
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            GetUserData();
            return UniTask.CompletedTask;
        }

        private void GetUserData()
        {
            var userData = _gameModel.LevelsData;

            // if (userData.Count == 0)
            // {
            //     userData.Add(new Utils.LevelData(1, 1));
            // }

            var data = userData.Select(e => $"Level:{e.Level}- Star:{e.Star}").ToList();
            Debug.Log($"user data {string.Join(", ", data)}");

            // Utils.LevelData[] userData = new Utils.LevelData[12];
            // for (var index = 0; index < userData.Count; index++)
            // {
            //     var random = Random.Range(1, 4);
            //     userData[index] = new Utils.LevelData(index + 1, random);
            // }

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
            Debug.Log("OnClickPlayLevel");
            // ScreenContainer.Of(transform).Push(new ScreenOptions(ResourceKey.PlayScreenPrefab()));
            ScreenContainer.Find(ContainerKey.Screens).Push(new ScreenOptions(ResourceKey.PlayScreenPrefab()));
            this.SendEvent(new InitGridEvent(level));
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}