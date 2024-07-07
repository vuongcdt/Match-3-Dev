using System.Collections;
using Commands;
using Events;
using QFramework;
using Queries;
using UnityEngine;

namespace GameControllers
{
    public class GameManager : MonoBehaviour, IController
    {
        [SerializeField] private Cell cell;
        [SerializeField] private Transform backgroundBlock;
        [SerializeField] private Transform gridBlock;
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private float cellSize;
        [SerializeField] private float avatarSize;
        [SerializeField] private float backgroundSize;
        [SerializeField] private float fillTime;
        [SerializeField] private bool isProcessing;

        private Cell[,] _grid;
        private bool _isRevertFill;

        private void Start()
        {
            this.RegisterEvent<ProcessingGridEvent>(e => { StartCoroutine(ProcessingGridIE()); })
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.SendCommand(new InitSettingsGridModelCommand(width, height, cellSize, fillTime));
            _grid = new Cell[width, height];

            this.SendCommand(new InitGridModelCommand(_grid));
            this.SendCommand(new RenderBackgroundGridCommand(cell, cellSize, backgroundBlock, backgroundSize));
            this.SendCommand(new RenderCellGridCommand(cell, cellSize, gridBlock, avatarSize));
            this.SendCommand<RenderRandomObstaclesCommand>();

            StartCoroutine(ProcessingGridIE());
        }

        private IEnumerator ProcessingGridIE()
        {
            do
            {
                this.SendQuery(new SetIsProcessingQuery(false));
                yield return new WaitForSeconds(fillTime);

                this.SendCommand(new AddCellToGridCommand(cell, avatarSize, gridBlock));

                StartCoroutine(FillIE());

                isProcessing = this.SendQuery(new GetIsProcessingQuery());
            } while (isProcessing);

            // this.SendCommand(new FillSpecialPositionCommand());

            StartCoroutine(MatchGridIE());
        }

        private IEnumerator MatchGridIE()
        {
            yield return new WaitForSeconds(fillTime);
            this.SendCommand<MatchGridCommand>();
        }

        private IEnumerator FillIE()
        {
            for (int y = height - 1; y > 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    this.SendCommand(new CheckFillCommand(x, y));
                }

                _isRevertFill = this.SendQuery(new SetIsRevertFillQuery(!_isRevertFill));

                yield return new WaitForSeconds(fillTime * 2f);
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}