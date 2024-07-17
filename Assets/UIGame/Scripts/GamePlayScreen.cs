using System;
using Cysharp.Threading.Tasks;
using Events.Sound;
using Interfaces;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Modals;
using Screen = ZBase.UnityScreenNavigator.Core.Screens.Screen;

namespace UIGame.Scripts
{
    public class GamePlayScreen : Screen, IController, ICanSendEvent
    {
        [SerializeField] private TMP_Text obstaclesTotalText;
        [SerializeField] private TMP_Text stepsTotalText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private Button pauseBtn;
        [SerializeField] private Image[] starIcons;
        [SerializeField] private Sprite starIconActive;
        [SerializeField] private Sprite starIconDeActive;

        private IGameModel _gameModel;

        public override UniTask Initialize(Memory<object> args)
        {
            base.OnEnable();
            Time.timeScale = 1;

            pauseBtn.onClick.RemoveAllListeners();
            pauseBtn.onClick.AddListener(OnPauseBtnClick);
            _gameModel = this.GetModel<IGameModel>();

            _gameModel.ObstaclesTotal.RegisterWithInitValue(SetObstacleText)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            _gameModel.StepsTotal.RegisterWithInitValue(SetStepMoveText)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            _gameModel.ScoreTotal.RegisterWithInitValue(SetScoreText)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            _gameModel.LevelSelect.RegisterWithInitValue(SetLevelText)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            return UniTask.CompletedTask;
        }

        private void SetObstacleText(int obstacleTotal)
        {
            CheckGameWin(obstacleTotal);

            obstaclesTotalText.text = obstacleTotal.ToString();
        }

        private void CheckGameWin(int obstacleTotal)
        {
            if (obstacleTotal != 0)
            {
                return;
            }

            this.SendEvent<PlaySoundGameWinSfxEvent>();
            ShowEndGamePopup().Forget();
            UpdateLevelData();
        }

        private void UpdateLevelData()
        {
            bool isHasUserData = false;
            var newData = new Utils.LevelData(_gameModel.LevelSelect.Value, _gameModel.StarsTotal.Value);

            for (var index = 0; index < _gameModel.LevelsData.Value.Count; index++)
            {
                var levelData = _gameModel.LevelsData.Value[index];

                if (levelData.Level != newData.Level)
                {
                    continue;
                }

                isHasUserData = true;

                if (newData.Star > levelData.Star)
                {
                    _gameModel.LevelsData.Value[index] = newData;
                }
            }

            if (!isHasUserData)
            {
                _gameModel.LevelsData.Value.Add(newData);
            }

            _gameModel.SaveLevelsData(); //vuong
        }

        private void SetScoreText(int value)
        {
            SetStarIcons(value);
            scoreText.text = (value * 10).ToString();
        }

        private void SetStarIcons(int score)
        {
            var starTotal = 0;
            for (var index = 0; index < starIcons.Length; index++)
            {
                var obstacles = Utils.GetObstaclesTotal(_gameModel.LevelSelect.Value);
                if (score >= obstacles * (index + 2))
                {
                    starTotal = index + 1;
                    starIcons[index].sprite = starIconActive;
                }
                else
                {
                    starIcons[index].sprite = starIconDeActive;
                }
            }

            _gameModel.StarsTotal.Value = starTotal;
        }

        private void SetStepMoveText(int stepMove)
        {
            CheckGameOver(stepMove);

            stepsTotalText.text = stepMove.ToString();
        }

        private void CheckGameOver(int stepMove)
        {
            if (stepMove != 0 || _gameModel.ObstaclesTotal.Value == 0)
            {
                return;
            }

            this.SendEvent<PlaySoundGameOverSfxEvent>();
            ShowEndGamePopup().Forget();
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

        private async UniTask ShowEndGamePopup()
        {
            await UniTask.WaitForSeconds(1);
            var options = new ModalOptions(ResourceKey.GameOverModalPrefab());
            await ModalContainer.Find(ContainerKey.Modals).PushAsync(options);
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}