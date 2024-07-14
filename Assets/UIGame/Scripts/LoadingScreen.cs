using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Screen = ZBase.UnityScreenNavigator.Core.Screens.Screen;

namespace UIGame.Scripts
{
    public class LoadingScreen : Screen
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TMP_Text textLoading;
        private readonly float _timeDelay = 2.5f;

        public override UniTask Initialize(Memory<object> args)
        {
            Loading().Forget();

            return UniTask.CompletedTask;
        }

        private async UniTaskVoid Loading()
        {
            for (float time = 0; time < _timeDelay; time += Time.deltaTime)
            {
                slider.value = time / _timeDelay * 100;
                textLoading.text = $"{Mathf.Ceil(time / _timeDelay * 100)}%";
                await UniTask.Yield();
            }
        }
    }
}