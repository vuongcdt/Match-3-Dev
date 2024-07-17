namespace Events.Sound
{
    public struct SetVolumeMusicEvent
    {
        public float Value;

        public SetVolumeMusicEvent(float value)
        {
            Value = value;
        }
    }
}