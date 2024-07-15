using Interfaces;
using QFramework;
using UnityEngine;

namespace Commands
{
    public class AddScoreCommand : AbstractCommand
    {
        private int _cellTotal;
    
        public AddScoreCommand(int cellTotal)
        {
            _cellTotal = cellTotal;
        }
    
        protected override void OnExecute()
        {
            var gameModel = this.GetModel<IGameModel>();
            gameModel.ScoreTotal.Value += _cellTotal;
        }
    }
}