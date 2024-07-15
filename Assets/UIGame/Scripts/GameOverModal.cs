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
    public class GameOverModal : Modal, IController
    {
        [SerializeField] private Button replayButton;
        [SerializeField] private Button homeButton;
        [SerializeField] private Button closeButton;

        [SerializeField] private TMP_Text replayText;
        [SerializeField] private GameObject gameOver;
        [SerializeField] private GameObject stars;
        [SerializeField] private Image[] startIcons;
        
        private IGameModel _gameModel;
        public override UniTask Initialize(Memory<object> args)
        {
            _gameModel = this.GetModel<IGameModel>();

            replayButton.onClick.RemoveAllListeners();
            replayButton.onClick.AddListener(OnReplayBtnClick);
            homeButton.onClick.RemoveAllListeners();
            homeButton.onClick.AddListener(OnHomeBtnClick);
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(OnCloseBtnClick);

            if (_gameModel.ObstaclesTotal.Value == 0)
            {
                replayText.text = "Next Level";
                gameOver.SetActive(false);
                stars.SetActive(true);
            }

            return UniTask.CompletedTask;
        }

        private void OnReplayBtnClick()
        {
            ModalContainer.Find(ContainerKey.Modals).Pop(true);
            this.SendCommand<ResetGameEventCommand>();
        }

        private void OnHomeBtnClick()
        {
            ModalContainer.Find(ContainerKey.Modals).Pop(true);
            ScreenContainer.Find(ContainerKey.Screens).Pop(true);
        }

        private void OnCloseBtnClick()
        {
            ModalContainer.Find(ContainerKey.Modals).Pop(true);
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}