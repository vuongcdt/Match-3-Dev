using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Interfaces;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Modals;
using Screen = ZBase.UnityScreenNavigator.Core.Screens.Screen;

namespace UIGame.Scripts
{
    public class GamePlayScreen : Screen, IController
    {
        [SerializeField] internal TMP_Text obstaclesTotalText;
        [SerializeField] internal TMP_Text stepsTotalText;
        [SerializeField] internal TMP_Text scoreText;
        [SerializeField] internal TMP_Text levelText;
        [SerializeField] private Button pauseBtn;
        
        private IGameModel _gameModel;

        public override UniTask Initialize(Memory<object> args)
        {
            base.OnEnable();

            pauseBtn.onClick.RemoveAllListeners();
            pauseBtn.onClick.AddListener(OnPauseBtnClick);

            _gameModel = this.GetModel<IGameModel>();
            
            _gameModel.ObstaclesTotal.RegisterWithInitValue(SetObstacleText)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            _gameModel.StepsTotal.RegisterWithInitValue(SetStepMoveText)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            _gameModel.ScoreTotal.RegisterWithInitValue(SetScoreText)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            _gameModel.Level.RegisterWithInitValue(SetLevelText)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            return UniTask.CompletedTask;
        }

        private void SetObstacleText(int value)
        {
            if (value == 0)
            {
                ShowGameOverPopup();
            }
            obstaclesTotalText.text = value.ToString();
        }

        private void SetScoreText(int value)
        {
            scoreText.text = value.ToString();
        }

        private void SetStepMoveText(int value)
        {
            if (value == 0)
            {
                ShowGameOverPopup();
            }
            stepsTotalText.text = value.ToString();
        }

        private void SetLevelText(int value)
        {
            levelText.text = $"Level {value}";
        }
        

        private void OnPauseBtnClick()
        {
            Time.timeScale = 0;
            var options = new ModalOptions(ResourceKey.PauseModalPrefab());
            ModalContainer.Find(ContainerKey.Modals).Push(options);
        }

        private void ShowGameOverPopup()
        {
            var options = new ModalOptions(ResourceKey.GameOverModalPrefab());
            ModalContainer.Find(ContainerKey.Modals).Push(options);
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}