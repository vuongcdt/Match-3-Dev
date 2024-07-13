using System;
using Commands;
using Cysharp.Threading.Tasks;
using QFramework;
using UnityEngine;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Screens;
using Screen = ZBase.UnityScreenNavigator.Core.Screens.Screen;

namespace UIGame.Scripts
{
    public class HomeScreen : Screen,IController
    {
        [SerializeField] private Button leverButton;
        
        public override UniTask Initialize(Memory<object> args)
        {
            leverButton.onClick.RemoveAllListeners();
            leverButton.onClick.AddListener(OnClickPlay);
        
            return UniTask.CompletedTask;
        }
        
        private void OnClickPlay()
        {
            this.SendCommand<ProcessingGridEventCommand>();
            ScreenContainer.Of(transform).Push(new ScreenOptions(ResourceKey.PlayScreenPrefab()));
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}