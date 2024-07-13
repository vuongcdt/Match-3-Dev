﻿using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Modals;
using ZBase.UnityScreenNavigator.Core.Screens;
using Screen = ZBase.UnityScreenNavigator.Core.Screens.Screen;

namespace UIGame.Scripts
{
    public class GamePlayScreen : Screen
    {
        // [SerializeField] internal TMP_Text scoreText;
        // [SerializeField] internal TMP_Text highScoreText;
        // [SerializeField] internal TMP_Text nextSquareText;
        // [SerializeField] internal Image background;
        //
        [SerializeField] private Button pauseBtn;
        
        public override UniTask Initialize(Memory<object> args)
        {
            base.OnEnable();
        
            pauseBtn.onClick.RemoveAllListeners();
            pauseBtn.onClick.AddListener(OnPauseBtnClick);
        
            return UniTask.CompletedTask;
        }
        
        private void OnPauseBtnClick()
        {
            // var options = new ModalOptions(ResourceKey.PauseModalPrefab());
            // ModalContainer.Find(ContainerKey.Modals).Push(options);
            ScreenContainer.Find(ContainerKey.Screens).Pop(true);
        }
        
        public void ShowGameOverPopup()
        {
            var options = new ModalOptions(ResourceKey.GameOverModalPrefab());
            ModalContainer.Find(ContainerKey.Modals).Push(options);
        }
        
        private IEnumerator DeActiveComboIE()
        {
            yield return new WaitForSeconds(1);
        }
    }
}