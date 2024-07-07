using Interfaces;
using QFramework;

namespace Queries
{
    public class GetIsProcessingQuery:AbstractQuery<bool>
    {
        protected override bool OnDo()
        {
            return this.GetModel<IGameModel>().IsProcessing.Value;
        }
    }
}