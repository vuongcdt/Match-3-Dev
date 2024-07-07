using Interfaces;
using QFramework;

namespace Queries
{
    public class GetIsRevertFillQuery:AbstractQuery<bool>
    {
        protected override bool OnDo()
        {
            return this.GetModel<IGameModel>().IsRevertFill.Value;
        }
    }
}