using System.Collections;
using QFramework;
using UnityEngine;

namespace Commands
{
    public class InvertedAndMatchCommandIE : AbstractCommand<IEnumerator>
    {
        private Utils.GridPos _sourceGridPos;
        private Utils.GridPos _targetGridPos;
        private ConfigGame _configGame;

        public InvertedAndMatchCommandIE(Utils.GridPos sourceGridPos, Utils.GridPos targetGridPos)
        {
            _sourceGridPos = sourceGridPos;
            _targetGridPos = targetGridPos;
        }

        protected override IEnumerator OnExecute()
        {
            _configGame = ConfigGame.Instance;
            return InvertedAndMatch();
        }

        private IEnumerator InvertedAndMatch()
        {
            var isSpecial = this.SendCommand(new InvertedCellCommand(_sourceGridPos, _targetGridPos));

            yield return new WaitForSeconds(ConfigGame.Instance.FillTime);

            var isMatch = this.SendCommand(new MatchGridCommand());

            if (!isMatch && !isSpecial)
            {
                this.SendCommand(new InvertedCellCommand(_targetGridPos, _sourceGridPos));
            }

            yield return new WaitForSeconds(ConfigGame.Instance.MatchTime);
            this.SendCommand<ProcessingGridEventCommand>();

            _configGame.IsDragged = false;
        }
        
    }
}