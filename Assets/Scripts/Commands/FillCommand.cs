using BaseScripts;
using QFramework;
using Queries;

namespace Commands
{
    public class FillCommand:AbstractCommand
    {
        private Cell _cell;
        protected override void OnExecute()
        {
            var grid = this.SendQuery(new GetGridQuery());
            for (int x = 0; x < 10; x++)
            {
                // if (grid[x, 0] == CONSTANTS.CellType.None)
                // {
                //     _cell.Create()
                // }
            }
        }
    }
}