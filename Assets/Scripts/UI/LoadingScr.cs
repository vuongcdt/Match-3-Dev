using Cysharp.Threading.Tasks;
using QFramework;
using TMPro;
using UIEvent;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour, ICanSendEvent
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text textLoading;
    private readonly float _timeDelay = 0.7f;


    void Start()
    {
        gameObject.SetActive(true);
        Loading().Forget();
    }

    private async UniTaskVoid Loading()
    {
        for (float time = 0; time < _timeDelay; time += Time.deltaTime)
        {
            slider.value = time / _timeDelay * 100;
            textLoading.text = $"{Mathf.Ceil(time / _timeDelay * 100)}%";
            await UniTask.Yield();
        }
        gameObject.SetActive(false);
        this.SendEvent<HomeScreenEvent>();
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}
