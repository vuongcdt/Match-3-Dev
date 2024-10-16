﻿using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Events;
using Events.Sound;
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
        private bool _isClick;

        protected override void OnEnable()
        {
            base.OnEnable();
            _isClick = false;
            if (_gameModel != null)
            {
                GetUserData();
            }
        }

        public override UniTask Initialize(Memory<object> args)
        {
            this.SendEvent<PlaySoundMusicEvent>();
            _gameModel = this.GetModel<IGameModel>();
            this.RegisterEvent<LevelSelectEvent>(e => OnClickPlayLevel(e.Level))
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            GetUserData();

            return UniTask.CompletedTask;
        }

        private void GetUserData()
        {
            var levelsData = _gameModel.LevelsData.Value ?? new List<Utils.LevelData>();

            for (var index = 0; index <= levelsData.Count; index++)
            {
                if (index >= cardItems.Length)
                {
                    break;
                }

                var cardItem = cardItems[index];

                if (index == levelsData.Count)
                {
                    cardItem.SetCardItem(CONSTANTS.TypeCard.UnLock, levelsData.Count + 1, 0);
                    break;
                }

                var dataLevel = levelsData[index];

                cardItem.SetCardItem(CONSTANTS.TypeCard.Checked, dataLevel.Level, dataLevel.Star);
            }

            for (var index = levelsData.Count; index < cardItems.Length; index++)
            {
                cardItems[index].SetLevelCardItem(index + 1);
            }
        }

        private void OnClickPlayLevel(int level)
        {
            if (_isClick)
            {
                return;
            }
            _isClick = true;
            _gameModel.LevelSelect.Value = level;
            _gameModel.ResetValueTextUI();

            ScreenContainer.Find(ContainerKey.Screens).Push(new ScreenOptions(ResourceKey.PlayScreenPrefab()));

            this.SendEvent(new InitLevelEvent(level));
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}