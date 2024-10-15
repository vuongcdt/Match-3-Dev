using Events;
using Interfaces;
using QFramework;
using TMPro;
using UIEvent;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPopup : MonoBehaviour, IController, ICanSendEvent
{
    [SerializeField] private Button replayButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private GameObject rightButton;

    [SerializeField] private TMP_Text replayText;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject stars;
    [SerializeField] private Image[] starIcons;
    [SerializeField] private Sprite starIconActive;
    [SerializeField] private Sprite starIconDeActive;

    private IGameModel _gameModel;

    public void OnEnable()
    {
        _gameModel = this.GetModel<IGameModel>();

        if (_gameModel.LevelSelect.Value == 32)
        {
            rightButton.SetActive(false);
        }

        if (_gameModel.ObstaclesTotal.Value != 0)
        {
            replayButton.onClick.RemoveAllListeners();
            replayButton.onClick.AddListener(OnReplayBtnClick);
            replayText.text = "Replay";
            gameOver.SetActive(true);
            stars.SetActive(false);
            return;
        }

        replayButton.onClick.RemoveAllListeners();
        replayButton.onClick.AddListener(OnNextLevelBtnClick);

        replayText.text = "Next Level";
        gameOver.SetActive(false);
        stars.SetActive(true);

        SetStarIcons();
    }

    public void Start()
    {
        homeButton.onClick.RemoveAllListeners();
        homeButton.onClick.AddListener(OnHomeBtnClick);

        this.RegisterEvent<GameOverPopupEvent>(e => gameObject.SetActive(true));
        gameObject.SetActive(false);
    }

    private void SetStarIcons()
    {
        for (var index = 0; index < starIcons.Length; index++)
        {
            starIcons[index].sprite = _gameModel.StarsTotal.Value > index ? starIconActive : starIconDeActive;
        }
    }

    private void OnNextLevelBtnClick()
    {
        Time.timeScale = 1;
        _gameModel.LevelSelect.Value++;
        _gameModel.ResetValueTextUI();
        // ModalContainer.Find(ContainerKey.Modals).Pop(true);
        this.SendEvent(new InitLevelEvent(_gameModel.LevelSelect.Value));
        gameObject.SetActive(false);
    }

    private void OnReplayBtnClick()
    {
        Time.timeScale = 1;
        _gameModel.ResetValueTextUI();
        // ModalContainer.Find(ContainerKey.Modals).Pop(true);
        gameObject.SetActive(false);
        this.SendEvent<ResetGameEvent>();
    }

    private void OnHomeBtnClick()
    {
        // ScreenContainer.Find(ContainerKey.Screens).Pop(true);
        // ModalContainer.Find(ContainerKey.Modals).Pop(true);
        gameObject.SetActive(false);
        this.SendEvent<HomeScreenEvent>();
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}
