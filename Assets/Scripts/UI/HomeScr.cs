using System.Collections.Generic;
using Events;
using Events.Sound;
using Interfaces;
using QFramework;
using UIEvent;
using UIGame.Scripts;
using UnityEngine;

public class HomeScr : MonoBehaviour, IController, ICanSendEvent
{
    [SerializeField] private CardItem[] cardItems;

    private IGameModel _gameModel;
    private bool _isClick;

    protected void OnEnable()
    {
        _isClick = false;
        if (_gameModel != null)
        {
            GetUserData();
        }
    }

    void Start()
    {
        this.RegisterEvent<HomeScreenEvent>(e => showHomeScreen()).UnRegisterWhenGameObjectDestroyed(gameObject);
        gameObject.SetActive(false);

        this.SendEvent<PlaySoundMusicEvent>();
        _gameModel = this.GetModel<IGameModel>();
        this.RegisterEvent<LevelSelectEvent>(e => OnClickPlayLevel(e.Level)).UnRegisterWhenGameObjectDestroyed(gameObject);

        GetUserData();
    }

    private void GetUserData()
    {
        var levelsData = _gameModel.LevelsData.Value ?? new List<Utils.LevelData>();

        for (var index = 0; index <= levelsData.Count; index++)
        {
            if (index >= cardItems.Length)
            {
                break;
            }

            var cardItem = cardItems[index];

            if (index == levelsData.Count)
            {
                cardItem.SetCardItem(CONSTANTS.TypeCard.UnLock, levelsData.Count + 1, 0);
                break;
            }

            var dataLevel = levelsData[index];

            cardItem.SetCardItem(CONSTANTS.TypeCard.Checked, dataLevel.Level, dataLevel.Star);
        }

        for (var index = levelsData.Count; index < cardItems.Length; index++)
        {
            cardItems[index].SetLevelCardItem(index + 1);
        }
    }

    private void OnClickPlayLevel(int level)
    {
        if (_isClick)
        {
            return;
        }
        _isClick = true;
        _gameModel.LevelSelect.Value = level;
        _gameModel.ResetValueTextUI();

        gameObject.SetActive(false);

        this.SendEvent(new InitLevelEvent(level));
        this.SendEvent<GamePlayScreenEvent>();
    }

    private void showHomeScreen()
    {
        gameObject.SetActive(true);
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }

}
