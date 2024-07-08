using System.Collections;
using Commands;
using Events;
using QFramework;
using UnityEngine;

namespace GameControllers
{
    public class GameManager : Singleton<GameManager>, IController
    {
        private Cell[,] _grid;
        private ConfigGame _configGame;

        private void Start()
        {
            this.RegisterEvent<ProcessingGridEvent>(e => { StartCoroutine(ProcessingGridIE()); })
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            _configGame = ConfigGame.Instance;
            _configGame.ButtonReset.onClick.RemoveAllListeners();
            _configGame.ButtonReset.onClick.AddListener(OnRestartClick);

            InitGame();
        }

        private void InitGame()
        {
            _grid = new Cell[_configGame.Width, _configGame.Height];
            this.SendCommand(new InitGridModelCommand(_grid));
            this.SendCommand(new RenderBackgroundGridCommand());
            this.SendCommand(new RenderCellGridCommand());
            this.SendCommand<RenderRandomObstaclesCommand>();

            StartCoroutine(ProcessingGridIE());
        }

        private IEnumerator ProcessingGridIE()
        {
            do
            {
                _configGame.IsProcessing = false;
                this.SendCommand(new AddCellToGridCommand());

                var fillCommandIE = this.SendCommand(new FillCommandIE());

                StartCoroutine(fillCommandIE);
                yield return new WaitForSeconds(_configGame.FillTime);
            } while (_configGame.IsProcessing);

            // this.SendCommand(new FillSpecialPositionCommand());

            var matchGridCommandIE = this.SendCommand(new MatchGridCommandIE());
            StartCoroutine(matchGridCommandIE);
        }

        public void OnRestartClick()
        {
            // InitGame();
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}