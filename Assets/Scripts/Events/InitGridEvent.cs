namespace Events
{
    public struct InitGridEvent
    {
        public int Level;

        public InitGridEvent(int level)
        {
            Level = level;
        }
    }
}