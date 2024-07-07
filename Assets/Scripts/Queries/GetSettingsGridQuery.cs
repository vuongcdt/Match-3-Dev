using Interfaces;
using QFramework;

namespace Queries
{
    public class GetSettingsGridQuery:AbstractQuery<Utils.SettingsGrid>
    {
        protected override Utils.SettingsGrid OnDo()
        {
            return this.GetModel<IGameModel>().SettingsGrid.Value;
        }
    }
}