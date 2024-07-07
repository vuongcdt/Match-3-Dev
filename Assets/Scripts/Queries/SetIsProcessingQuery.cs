using Interfaces;
using QFramework;

namespace Queries
{
    public class SetIsProcessingQuery:AbstractQuery<bool>
    {
        private bool _isProcessing;

        public SetIsProcessingQuery(bool isProcessing)
        {
            _isProcessing = isProcessing;
        }

        protected override bool OnDo()
        {
            this.GetModel<IGameModel>().IsProcessing.Value = _isProcessing;
            return _isProcessing;
        }
    }
}