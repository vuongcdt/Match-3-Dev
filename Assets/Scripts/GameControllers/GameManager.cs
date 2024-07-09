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
            var configGame = ConfigGame.Instance;
            do
            {
                configGame.IsProcessing = false;
                this.SendCommand(new AddCellToGridCommand());

                yield return new WaitForSeconds(configGame.FillTime);
                StartCoroutine(this.SendCommand(new FillCommandIE()));
            } while (configGame.IsProcessing);

            this.SendCommand(new MatchGridCommand());
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