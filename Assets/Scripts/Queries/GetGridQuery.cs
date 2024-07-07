using Interfaces;
using QFramework;

namespace Queries
{
    public class GetGridQuery:AbstractQuery<Cell[,]>
    {
        protected override Cell[,] OnDo()
        {
            return this.GetModel<IGameModel>().GridArray.Value;
        }
    }
}