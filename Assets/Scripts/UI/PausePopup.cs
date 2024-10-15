using Events.Sound;
using Interfaces;
using QFramework;
using UIEvent;
using UnityEngine;
using UnityEngine.UI;

public class PausePopup : MonoBehaviour, IController, ICanSendEvent
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderSfx;

    private IGameModel _gameModel;

    public void Start()
    {
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
        this.RegisterEvent<PausePopupEvent>(e=> gameObject.SetActive(true));

        gameObject.SetActive(false);
    }

    private void OnHomeBtnClick()
    {
        Time.timeScale = 1;
        // ModalContainer.Find(ContainerKey.Modals).Pop(true);
        // ScreenContainer.Find(ContainerKey.Screens).Pop(true);
        gameObject.SetActive(false);
        this.SendEvent<HomeScreenEvent>();
    }

    private void OnCloseBtnClick()
    {
        Time.timeScale = 1;
        // ModalContainer.Find(ContainerKey.Modals).Pop(true);
        gameObject.SetActive(false);
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
