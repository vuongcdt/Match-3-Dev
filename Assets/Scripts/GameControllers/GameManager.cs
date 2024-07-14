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

        private int _level;

        private void Start()
        {
            Application.targetFrameRate = 60;

            this.RegisterEvent<ProcessingGridEvent>(e => StartCoroutine(ProcessingGrid()))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<InitGridEvent>(e => InitLevel(e.Level))
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            _configGame = ConfigGame.Instance;
            _configGame.ButtonReset.onClick.RemoveAllListeners();
            _configGame.ButtonReset.onClick.AddListener(OnRestartClick);

            _grid = new Cell[_configGame.Width, _configGame.Height];
            this.SendCommand(new InitGridModelCommand(_grid));
            this.SendCommand<RenderBackgroundGridCommand>();

            // InitGame();
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
            
            _configGame.StepsTotal = _configGame.ObstaclesTotal;
            
            this.SendCommand<SetObstaclesTotalCommand>();
            this.SendCommand<SetStepsTotalCommand>();
            
            yield return new WaitForSeconds(1);
            // StartCoroutine(ProcessingGrid());
        }

        private IEnumerator ProcessingGrid()
        {
            _configGame.IsProcessing = true;

            if (_configGame.ObstaclesTotal == 0)
            {
                StartCoroutine(ResetGame());
                yield break;
            }

            if (_configGame.ObstaclesTotal > 0 && _configGame.StepsTotal == 0)
            {
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
            yield return new WaitForSeconds(_configGame.MatchTime * 2);
            StartCoroutine(InitGame());
        }

        public void OnRestartClick()
        {
            StartCoroutine(InitGame());
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}