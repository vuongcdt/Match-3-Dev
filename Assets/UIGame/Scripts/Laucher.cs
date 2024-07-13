using Cysharp.Threading.Tasks;
using ZBase.UnityScreenNavigator.Core;
using ZBase.UnityScreenNavigator.Core.Modals;
using ZBase.UnityScreenNavigator.Core.Screens;
using ZBase.UnityScreenNavigator.Core.Views;
using ZBase.UnityScreenNavigator.Core.Windows;

namespace UIGame.Scripts
{
    public class Launcher : UnityScreenNavigatorLauncher
    {
        private static WindowContainerManager ContainerManager { get; set; }

        private ScreenContainer _screenContainer;
        private ModalContainer _modalContainer;

        protected override void OnAwake()
        {
            base.OnAwake();
            ContainerManager = this;
        }

        protected override void OnPostCreateContainers()
        {
            base.OnPostCreateContainers();
            _screenContainer = ContainerManager.Find<ScreenContainer>(ContainerKey.Screens);
            _modalContainer = ModalContainer.Find(ContainerKey.Modals);

            PreloadingScreen();
            ShowLoadingPage().Forget();
        }

        private void PreloadingScreen()
        {
            _screenContainer.Preload(ResourceKey.LoadingScreenPrefab());
            _screenContainer.Preload(ResourceKey.HomeScreenPrefab());
            _screenContainer.Preload(ResourceKey.PlayScreenPrefab());
            
            _modalContainer.Preload(ResourceKey.PauseModalPrefab());
            // _modalContainer.Preload(ResourceKey.GameOverModalPrefab());

        }

        private async UniTaskVoid ShowLoadingPage()
        {
            var options = new ViewOptions(ResourceKey.LoadingScreenPrefab(), false, loadAsync: false);
            await _screenContainer.PushAsync(options);
            Invoke(nameof(ShowHomePage), 1);
        }

        private async UniTaskVoid ShowHomePage()
        {
            var options = new ViewOptions(ResourceKey.HomeScreenPrefab(), false, loadAsync: false);
            await _screenContainer.PushAsync(options);
        }
    }
}