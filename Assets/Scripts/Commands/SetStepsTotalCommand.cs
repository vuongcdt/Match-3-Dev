using Interfaces;
using QFramework;

namespace Commands
{
    public class SetStepsTotalCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var gameModel = this.GetModel<IGameModel>();
            gameModel.StepsTotal.Value--;
        }
    }
}