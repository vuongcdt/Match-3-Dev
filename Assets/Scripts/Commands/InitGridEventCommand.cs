using Events;
using QFramework;

namespace Commands
{
    public class InitGridEventCommand:AbstractCommand
    {
        private int _level;

        public InitGridEventCommand(int level)
        {
            _level = level;
        }

        protected override void OnExecute()
        {
            this.SendEvent( new InitGridEvent(_level));
        }
    }
}