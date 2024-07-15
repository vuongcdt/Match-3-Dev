using Events;
using QFramework;

namespace Commands
{
    public class ResetGameEventCommand:AbstractCommand
    {
        protected override void OnExecute()
        {
            this.SendEvent<ResetGameEvent>();
        }
    }
}