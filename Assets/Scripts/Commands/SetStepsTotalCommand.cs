using QFramework;

namespace Commands
{
    public class SetStepsTotalCommand:AbstractCommand
    {
        private ConfigGame _configGame;
        protected override void OnExecute()
        {
            _configGame = ConfigGame.Instance;
            _configGame.StepsTotal--;
            _configGame.StepsTotalText.text = _configGame.StepsTotal.ToString();
        }
    }
}