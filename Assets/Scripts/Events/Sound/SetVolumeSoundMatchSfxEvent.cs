namespace Events.Sound
{
    public struct SetVolumeSoundMatchSfxEvent
    {
        public float Value;

        public SetVolumeSoundMatchSfxEvent(float value)
        {
            Value = value;
        }
    }
}