using Interfaces;
using QFramework;

namespace Queries
{
    public class SetIsRevertFillQuery:AbstractQuery<bool>
    {
        private bool _isRevertFill;

        public SetIsRevertFillQuery(bool isRevertFill)
        {
            _isRevertFill = isRevertFill;
        }

        protected override bool OnDo()
        {
            this.GetModel<IGameModel>().IsRevertFill.Value = _isRevertFill;
            return _isRevertFill;
        }
    }
}