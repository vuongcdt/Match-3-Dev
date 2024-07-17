namespace Events
{
    public struct LevelSelectEvent
    {
        public int Level;

        public LevelSelectEvent(int level)
        {
            Level = level;
        }
    }
}