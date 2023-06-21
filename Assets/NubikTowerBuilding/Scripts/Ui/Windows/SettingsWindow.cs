using Cysharp.Threading.Tasks;
using KlopoffGames.Core.Audio;
using NubikTowerBuilding.Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace NubikTowerBuilding.Ui.Windows
{
    public class SettingsWindow : DefaultPopUpWindow
    {
        [Inject] private AudioManager _audio;
        [Inject] private SavingService _savingService;
        [SerializeField] private Button buttonDone;
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Toggle soundsToggle;
        
        public override UniTask OnCreate()
        {
            buttonDone.onClick.AddListener(OnDoneButtonClick);
            musicToggle.onValueChanged.AddListener(OnMusicToggleChange);
            soundsToggle.onValueChanged.AddListener(OnSoundToggleChange);
            
            _audio.OnMusicMuteChange += OnMusicMuteChanged;
            _audio.OnSoundMuteChange += OnSoundMuteChanged;
            
            UpdateToggles();
            
            return base.OnCreate();
        }

        public override UniTask OnHide()
        {
            buttonDone.onClick.RemoveAllListeners();
            musicToggle.onValueChanged.RemoveAllListeners();
            soundsToggle.onValueChanged.RemoveAllListeners();
            
            return base.OnHide();
        }
        
        private void UpdateToggles()
        {
            musicToggle.SetIsOnWithoutNotify(!_audio.MusicMute);
            soundsToggle.SetIsOnWithoutNotify(!_audio.SoundMute);
        }
        
        private void OnMusicMuteChanged(bool val)
        {
            UpdateToggles();
        }

        private void OnSoundMuteChanged(bool val)
        {
            UpdateToggles();
        }

        private void OnDoneButtonClick()
        {
            Hide();
        }

        private void OnMusicToggleChange(bool val)
        {
            _audio.MusicMute = !val;
            _savingService.SetMusicVolume(_audio.MusicMute ? 0f : 1f);
        }

        private void OnSoundToggleChange(bool val)
        {
            _audio.SoundMute = !val;
            _savingService.SetSoundVolume(_audio.SoundMute ? 0f : 1f);
        }
    }
}