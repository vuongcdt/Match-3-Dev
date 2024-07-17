using System;
using Commands;
using Cysharp.Threading.Tasks;
using Events;
using Interfaces;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Modals;
using ZBase.UnityScreenNavigator.Core.Screens;

namespace UIGame.Scripts
{
    public class EndGameModal : Modal, IController, ICanSendEvent
    {
        [SerializeField] private Button replayButton;
        [SerializeField] private Button homeButton;
        [SerializeField] private GameObject rightButton;

        [SerializeField] private TMP_Text replayText;
        [SerializeField] private GameObject gameOver;
        [SerializeField] private GameObject stars;
        [SerializeField] private Image[] starIcons;
        [SerializeField] private Sprite starIconActive;
        [SerializeField] private Sprite starIconDeActive;

        private IGameModel _gameModel;

        public override UniTask Initialize(Memory<object> args)
        {
            _gameModel = this.GetModel<IGameModel>();

            replayButton.onClick.RemoveAllListeners();
            replayButton.onClick.AddListener(OnReplayBtnClick);

            homeButton.onClick.RemoveAllListeners();
            homeButton.onClick.AddListener(OnHomeBtnClick);
            if (_gameModel.LevelSelect.Value == 32)
            {
                rightButton.SetActive(false);
            }

            if (_gameModel.ObstaclesTotal.Value != 0)
            {
                return UniTask.CompletedTask;
            }
            replayText.text = "Next Level";
            gameOver.SetActive(false);
            stars.SetActive(true);

            SetStarIcons();

            replayButton.onClick.RemoveAllListeners();
            replayButton.onClick.AddListener(OnNextLevelBtnClick);

            return UniTask.CompletedTask;
        }

        private void SetStarIcons()
        {
            for (var index = 0; index < starIcons.Length; index++)
            {
                starIcons[index].sprite = _gameModel.StarsTotal.Value > index ? starIconActive : starIconDeActive;
            }
        }

        private void OnNextLevelBtnClick()
        {
            Time.timeScale = 1;
            _gameModel.LevelSelect.Value++;
            _gameModel.ResetValueTextUI();
            ModalContainer.Find(ContainerKey.Modals).Pop(true);
            this.SendEvent(new InitLevelEvent(_gameModel.LevelSelect.Value));
        }

        private void OnReplayBtnClick()
        {
            Time.timeScale = 1;
            _gameModel.ResetValueTextUI();
            ModalContainer.Find(ContainerKey.Modals).Pop(true);
            this.SendEvent<ResetGameEvent>();
        }

        private void OnHomeBtnClick()
        {
            ScreenContainer.Find(ContainerKey.Screens).Pop(true);
            ModalContainer.Find(ContainerKey.Modals).Pop(true);
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}