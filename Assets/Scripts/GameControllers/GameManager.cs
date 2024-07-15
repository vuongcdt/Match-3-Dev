using System.Collections;
using Commands;
using Events;
using Interfaces;
using QFramework;
using UnityEngine;

namespace GameControllers
{
    public class GameManager : MonoBehaviour, IController
    {
        private Cell[,] _grid;
        private ConfigGame _configGame;
        private IGameModel _gameModel;
        private int _level;

        private void Start()
        {
            Application.targetFrameRate = 60;
            _gameModel = this.GetModel<IGameModel>();

            this.RegisterEvent<ProcessingGridEvent>(e => StartCoroutine(ProcessingGrid()))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<InitGridEvent>(e => InitLevel(e.Level))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<ResetGameEvent>(e => StartCoroutine(ResetGame()))
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            _configGame = ConfigGame.Instance;

            _grid = new Cell[_configGame.Width, _configGame.Height];
            this.SendCommand(new InitGridModelCommand(_grid));
            this.SendCommand<RenderBackgroundGridCommand>();
        }

        private void InitLevel(int level)
        {
            _configGame.Level = level;
            StartCoroutine(InitGame());
        }

        private IEnumerator InitGame()
        {
            this.SendCommand<RenderCellGridCommand>();
            this.SendCommand<RenderRandomObstaclesCommand>();

            _gameModel.ScoreTotal.Value = 0;
            _gameModel.StepsTotal.Value = Utils.GetStepsMove(_gameModel.ObstaclesTotal.Value);

            this.SendCommand<SetObstaclesTotalCommand>();
            this.SendCommand<SetStepsTotalCommand>();

            yield return new WaitForSeconds(1);
            StartCoroutine(ProcessingGrid());
        }

        private IEnumerator ProcessingGrid()
        {
            _configGame.IsProcessing = true;
            if (_gameModel.ObstaclesTotal.Value == 0 || _gameModel.StepsTotal.Value == 0)
            {
                yield break;
            }

            this.SendCommand<FillCommand>();
            var isAdd = this.SendCommand(new AddCellToGridCommand());

            if (isAdd)
            {
                yield return new WaitForSeconds(_configGame.FillTime);
                StartCoroutine(ProcessingGrid());
                yield break;
            }

            var isMatch = this.SendCommand(new MatchGridCommand());

            if (isMatch)
            {
                yield return new WaitForSeconds(_configGame.MatchTime);
                StartCoroutine(ProcessingGrid());
                yield break;
            }

            _configGame.IsProcessing = false;
        }

        private IEnumerator ResetGame()
        {
            yield return new WaitForSeconds(_configGame.MatchTime);
            StartCoroutine(InitGame());
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}