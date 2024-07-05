namespace BaseScripts
{
    public abstract class CommandBase 
    {
        public abstract void Init();
        public abstract void Execute();
        public abstract void Undo();
    }
}