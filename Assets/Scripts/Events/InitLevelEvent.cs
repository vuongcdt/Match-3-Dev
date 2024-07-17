namespace Events
{
    public struct InitLevelEvent
    {
        public int Level;

        public InitLevelEvent(int level)
        {
            Level = level;
        }
    }
}