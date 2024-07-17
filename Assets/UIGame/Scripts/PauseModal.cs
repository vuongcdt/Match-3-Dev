using System;
using Cysharp.Threading.Tasks;
using Events.Sound;
using Interfaces;
using QFramework;
using UnityEngine;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Modals;
using ZBase.UnityScreenNavigator.Core.Screens;

namespace UIGame.Scripts
{
    public class PauseModal : Modal, IController,ICanSendEvent
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private Button homeButton;
        [SerializeField] private Slider sliderMusic;
        [SerializeField] private Slider sliderSfx;

        private IGameModel _gameModel;

        public override UniTask Initialize(Memory<object> args)
        {
            base.OnEnable();
            _gameModel = this.GetModel<IGameModel>();

            sliderMusic.onValueChanged.RemoveAllListeners();
            sliderMusic.onValueChanged.AddListener(OnChangeVolumeMusic);

            sliderSfx.onValueChanged.RemoveAllListeners();
            sliderSfx.onValueChanged.AddListener(OnChangeVolumeSFX);

            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnCloseBtnClick);

            homeButton.onClick.RemoveAllListeners();
            homeButton.onClick.AddListener(OnHomeBtnClick);

            _gameModel.MusicSetting.RegisterWithInitValue(SetVolumeMusic)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            _gameModel.SfxSetting.RegisterWithInitValue(SetVolumeSfx)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            return UniTask.CompletedTask;
        }

        private void OnHomeBtnClick()
        {
            Time.timeScale = 1;
            ModalContainer.Find(ContainerKey.Modals).Pop(true);
            ScreenContainer.Find(ContainerKey.Screens).Pop(true);
        }

        private void OnCloseBtnClick()
        {
            Time.timeScale = 1;
            ModalContainer.Find(ContainerKey.Modals).Pop(true);
        }

        private void SetVolumeMusic(float value)
        {
            sliderMusic.value = _gameModel.MusicSetting.Value;
        }

        private void SetVolumeSfx(float value)
        {
            sliderSfx.value = _gameModel.SfxSetting.Value;
        }

        private void OnChangeVolumeMusic(float value)
        {
            this.SendEvent(new SetVolumeMusicEvent(value));
        }

        private void OnChangeVolumeSFX(float value)
        {
            this.SendEvent(new SetVolumeSoundMatchSfxEvent(value));
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}