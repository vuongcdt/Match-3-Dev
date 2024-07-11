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
            this.RegisterEvent<ProcessingGridEvent>(e => ProcessingGrid())
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

            ProcessingGrid();
        }

        private void ProcessingGrid()
        {
            if (_configGame.ObstaclesTotal == 0)
            {
                StartCoroutine(ResetGame());
                return;
            }

            this.SendCommand<FillCommand>();
            StartCoroutine(this.SendCommand(new AddCellToGridCommand()));
        }

        private IEnumerator ResetGame()
        {
            yield return new WaitForSeconds(4);
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