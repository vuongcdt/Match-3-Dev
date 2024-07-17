using Interfaces;
using QFramework;

namespace Commands
{
    public class SetObstaclesTotalCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var gameModel = this.GetModel<IGameModel>();
           var grid = gameModel.GridArray.Value;

            int count = 0;
            foreach (var cell in grid)
            {
                if (cell.Type == CONSTANTS.CellType.Obstacle)
                {
                    count++;
                }
            }

            gameModel.ObstaclesTotal.Value = count;
        }
    }
}