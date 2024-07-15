using System;
using Commands;
using Cysharp.Threading.Tasks;
using Interfaces;
using QFramework;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core.Screens;
using Screen = ZBase.UnityScreenNavigator.Core.Screens.Screen;

namespace UIGame.Scripts
{
    public class HomeScreen : Screen, IController
    {
        [SerializeField] private CardItem[] cardItems;

        private IGameModel _gameModel;

        public override UniTask Initialize(Memory<object> args)
        {
            base.OnEnable();
            _gameModel = this.GetModel<IGameModel>();

            _gameModel.Level.RegisterWithInitValue(level => GetUserData());

            // GetUserData();

            return UniTask.CompletedTask;
        }

        private void GetUserData()
        {
            var userData = _gameModel.UserData.Value;
            Debug.Log($"user data2 {userData.Count}");

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


        public void OnClickPlayLevel(int level)
        {
            ScreenContainer.Of(transform).Push(new ScreenOptions(ResourceKey.PlayScreenPrefab()));
            this.SendCommand(new InitGridEventCommand(level));
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}