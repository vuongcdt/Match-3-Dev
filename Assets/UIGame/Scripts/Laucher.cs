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
        private readonly float _timeDelay = 3;

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
            await UniTask.WaitForSeconds(_timeDelay);
            await ShowHomePage();
        }

        private async UniTask ShowHomePage()
        {
            var options = new ViewOptions(ResourceKey.HomeScreenPrefab(), false, loadAsync: false);
            await _screenContainer.PushAsync(options);
        }
    }
}