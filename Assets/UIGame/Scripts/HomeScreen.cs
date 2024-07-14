using System;
using System.Collections.Generic;
using Commands;
using Cysharp.Threading.Tasks;
using QFramework;
using UnityEngine;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Screens;
using Random = UnityEngine.Random;
using Screen = ZBase.UnityScreenNavigator.Core.Screens.Screen;

namespace UIGame.Scripts
{
    public class HomeScreen : Screen, IController
    {
        [SerializeField] private CardItem[] cardItems;

        public override UniTask Initialize(Memory<object> args)
        {
            Utils.UserData[] userData = new Utils.UserData[12];
            for (var index = 0; index < userData.Length; index++)
            {
                var random = Random.Range(0, 4);
                userData[index] = new Utils.UserData(index + 1, random);
            }

            for (var index = 0; index <= userData.Length; index++)
            {
                if (index >= cardItems.Length)
                {
                    break;
                }

                var cardItem = cardItems[index];

                if (index == userData.Length)
                {
                    cardItem.SetCardItem(CONSTANTS.TypeCard.Checking, userData.Length + 1, 0);
                    break;
                }

                var dataLevel = userData[index];

                cardItem.SetCardItem(CONSTANTS.TypeCard.Checked, dataLevel.Level, dataLevel.Star);
            }

            for (var index = userData.Length; index < cardItems.Length; index++)
            {
                cardItems[index].SetLevelCardItem(index + 1);
            }

            return UniTask.CompletedTask;
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