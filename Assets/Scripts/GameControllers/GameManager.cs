using System.Collections;
using Commands;
using Events;
using QFramework;
using UnityEngine;

namespace GameControllers
{
    public class GameManager : MonoBehaviour, IController
    {
        private Cell[,] _grid;
        private ConfigGame _configGame;

        private void Start()
        {
            Application.targetFrameRate = 60;

            this.RegisterEvent<ProcessingGridEvent>(e => StartCoroutine(ProcessingGrid()))
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            _configGame = ConfigGame.Instance;
            _configGame.ButtonReset.onClick.RemoveAllListeners();
            _configGame.ButtonReset.onClick.AddListener(OnRestartClick);

            _grid = new Cell[_configGame.Width, _configGame.Height];
            this.SendCommand(new InitGridModelCommand(_grid));
            this.SendCommand<RenderBackgroundGridCommand>();

            InitGame();
        }

        private void InitGame()
        {
            this.SendCommand<RenderCellGridCommand>();
            this.SendCommand<RenderRandomObstaclesCommand>();
            _configGame.StepsTotal = _configGame.ObstaclesTotal;
            this.SendCommand<SetStepsTotalCommand>();
            StartCoroutine(ProcessingGrid());
        }

        private IEnumerator ProcessingGrid()
        {
            _configGame.IsProcessing = true;

            if (_configGame.ObstaclesTotal == 0)
            {
                Debug.Log("GAME WIN");
                StartCoroutine(ResetGame());
                yield break;
            }

            if (_configGame.ObstaclesTotal > 0 && _configGame.StepsTotal == 0)
            {
                Debug.Log("GAME OVER");
                StartCoroutine(ResetGame());
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
            yield return new WaitForSeconds(_configGame.MatchTime * 5);
            InitGame();
        }

        public void OnRestartClick()
        {
            InitGame();
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}