using System.Collections;
using Commands;
using Events;
using QFramework;
using Queries;
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
                yield return new WaitForSeconds(_configGame.FillTime);

                this.SendCommand(new AddCellToGridCommand());

                StartCoroutine(FillIE());
                Debug.Log($"IsProcessing {_configGame.IsProcessing}");

            } while (_configGame.IsProcessing);

            // this.SendCommand(new FillSpecialPositionCommand());

            StartCoroutine(MatchGridIE());
        }

        private IEnumerator MatchGridIE()
        {
            yield return new WaitForSeconds(_configGame.FillTime);
            this.SendCommand<MatchGridCommand>();
        }

        private IEnumerator FillIE()
        {
            for (int y = _configGame.Height - 1; y > 0; y--)
            {
                for (int x = 0; x < _configGame.Width; x++)
                {
                    this.SendCommand(new CheckFillCommand(x, y));
                }

                _configGame.IsRevertFill = !_configGame.IsRevertFill;

                yield return new WaitForSeconds(_configGame.FillTime * 2f);
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}