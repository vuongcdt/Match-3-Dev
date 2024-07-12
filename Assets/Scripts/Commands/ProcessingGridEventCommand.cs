using Events;
using QFramework;

namespace Commands
{
    public class ProcessingGridEventCommand:AbstractCommand
    {
        protected override void OnExecute()
        {
            this.SendEvent<ProcessingGridEvent>();
        }
    }
}