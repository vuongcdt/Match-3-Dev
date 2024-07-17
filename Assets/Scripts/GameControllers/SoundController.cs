using Events.Sound;
using Interfaces;
using QFramework;
using UnityEngine;

namespace GameControllers
{
    public class SoundController : MonoBehaviour, IController
    {
        [SerializeField] private AudioSource audioSourceMusic;
        [SerializeField] private AudioSource audioSourceFill;
        [SerializeField] private AudioSource audioSourceInvertedSfx;
        [SerializeField] private AudioSource audioSourceMatchSfx;
        [SerializeField] private AudioSource audioSourceNoInvertedSfx;
        [SerializeField] private AudioSource audioSourceGameOverSfx;
        [SerializeField] private AudioSource audioSourceGameWinSfx;
        [SerializeField] private AudioSource audioSourceClickSfx;

        private IGameModel _gameModel;

        private void Start()
        {
            _gameModel = this.GetModel<IGameModel>();
            
            this.RegisterEvent<PlaySoundMusicEvent>(e => PlaySoundMusic());
            this.RegisterEvent<PlaySoundFillSfxEvent>(e => PlaySoundFillSfx());
            this.RegisterEvent<PlaySoundInvertedSfxEvent>(e => PlaySoundInvertedSfx());
            this.RegisterEvent<PlaySoundMatchSfxEvent>(e => PlaySoundMatchSfx());
            this.RegisterEvent<PlaySoundNoInvertedSfxEvent>(e => PlaySoundNoInvertedSfx());
            this.RegisterEvent<PlaySoundGameOverSfxEvent>(e => PlaySoundGameOverSfx());
            this.RegisterEvent<PlaySoundGameWinSfxEvent>(e => PlaySoundGameWinSfx());
            this.RegisterEvent<PlaySoundClickSfxEvent>(e => PlaySoundClickSfx());
            this.RegisterEvent<SetVolumeMusicEvent>(e => SetVolumeMusic(e.Value));
            this.RegisterEvent<SetVolumeSoundMatchSfxEvent>(e => SetVolumeSoundMatchSfx(e.Value));
        }

        private void PlaySoundMusic()
        {
            audioSourceMusic.volume = _gameModel.MusicSetting.Value;
            audioSourceMusic.Play();
        }

        private void PlaySoundFillSfx()
        {
            audioSourceFill.volume = _gameModel.SfxSetting.Value * 0.25f;
            audioSourceFill.Play();
        }

        private void PlaySoundInvertedSfx()
        {
            audioSourceInvertedSfx.volume = _gameModel.SfxSetting.Value;
            audioSourceInvertedSfx.Play();
        }

        private void PlaySoundMatchSfx()
        {
            audioSourceMatchSfx.volume = _gameModel.SfxSetting.Value;
            audioSourceMatchSfx.Play();
        }

        private void PlaySoundNoInvertedSfx()
        {
            audioSourceNoInvertedSfx.volume = _gameModel.SfxSetting.Value;
            audioSourceNoInvertedSfx.Play();
        }

        private void PlaySoundGameWinSfx()
        {
            audioSourceGameWinSfx.volume = _gameModel.SfxSetting.Value;
            audioSourceGameWinSfx.Play();
        }
        private void PlaySoundGameOverSfx()
        {
            audioSourceGameOverSfx.volume = _gameModel.SfxSetting.Value;
            audioSourceGameOverSfx.Play();
        }

        private void PlaySoundClickSfx()
        {
            audioSourceClickSfx.volume = _gameModel.SfxSetting.Value;
            audioSourceClickSfx.Play();
        }

        private void SetVolumeMusic(float value)
        {
            _gameModel.MusicSetting.Value = value;
            audioSourceMusic.volume = value;
        }

        private void SetVolumeSoundMatchSfx(float value)
        {
            _gameModel.SfxSetting.Value = value;
            PlaySoundMatchSfx();
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}