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
            // SpawnPool();
            this.SendCommand<RenderBackgroundGridCommand>();

            InitGame();
        }

        private void SpawnPool()
        {
            for (int i = 0; i < _configGame.Width * (_configGame.Height + 1) * 2; i++)
            {
                var cell = _configGame.Cell.Create(
                    new Utils.GridPos(0,_configGame.Height),
                    this.transform,
                    _configGame.BackgroundSize,
                    CONSTANTS.CellType.None);
                _configGame.Pool.Push(cell);
            }
        }

        private void InitGame()
        {
            this.SendCommand<RenderCellGridCommand>();
            this.SendCommand<RenderRandomObstaclesCommand>();

            StartCoroutine(ProcessingGrid());
        }

        private IEnumerator ProcessingGrid()
        {
            if (_configGame.ObstaclesTotal == 0)
            {
                StartCoroutine(ResetGame());
                yield return new WaitForSeconds(_configGame.FillTime);
            }
            
            this.SendCommand<FillCommand>();
            var isAdd = this.SendCommand(new AddCellToGridCommand());
            
            if (isAdd)
            {
                yield return new WaitForSeconds(_configGame.FillTime);
                StartCoroutine(ProcessingGrid());
            }
            else
            {
                var isMatch = this.SendCommand(new MatchGridCommand());
                yield return new WaitForSeconds(_configGame.MatchTime);
                
                if (isMatch)
                {
                    StartCoroutine(ProcessingGrid());
                }
            }
        }

        private IEnumerator ResetGame()
        {
            Debug.Log("GAME WIN");
            yield return new WaitForSeconds(2);
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