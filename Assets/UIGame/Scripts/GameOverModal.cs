using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Modals;

namespace UIGame.Scripts
{
    public class GameOverModal : Modal
    {
        // [SerializeField] private Button replayButton;
        //
        //
        // public override UniTask Initialize(Memory<object> args)
        // {
        //     replayButton.onClick.RemoveAllListeners();
        //     replayButton.onClick.AddListener(OnReplayBtnClick);
        //
        //     return UniTask.CompletedTask;
        // }
        //
        // private void OnReplayBtnClick()
        // {
        //     ModalContainer.Of(transform).Pop(true);
        //     // if (Prefs.HighScore < _boardManager.highScore)
        //     // {
        //     //     Prefs.HighScore = _boardManager.highScore;
        //     // }
        //     // _uiManager.ResetGame();
        // }
    }
}