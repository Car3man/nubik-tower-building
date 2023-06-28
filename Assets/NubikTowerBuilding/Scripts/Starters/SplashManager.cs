using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using KlopoffGames.Core.Analytics;
using KlopoffGames.Core.Audio;
using KlopoffGames.Core.Localization;
using KlopoffGames.Core.Saving;
using NubikTowerBuilding.Scenes;
using NubikTowerBuilding.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
#if YANDEX_GAMES && !UNITY_EDITOR
using KlopoffGames.WebPlatforms.Yandex;
#endif
#if VK_GAMES && !UNITY_EDITOR
using KlopoffGames.WebPlatforms.VK;
#endif

namespace NubikTowerBuilding.Starters
{
    public class SplashManager : MonoBehaviour
    {
        [Inject] private IAnalytics _analytics;
        [Inject] private LocalizationManager _localization;
        [Inject] private ISavingManager _saving;
        [Inject] private SavingService _savingService;
        [Inject] private AudioManager _audio;
        [Inject] private GameSceneRef _gameSceneRef;
#if YANDEX_GAMES && !UNITY_EDITOR
        [Inject] private YandexManager _yandex;
#endif
#if VK_GAMES && !UNITY_EDITOR
        [Inject] private VKManager _vk;
#endif
        [SerializeField] private Image loadingImage;
        [SerializeField] private TextMeshProUGUI loadingText;

        private float _loaderTextDots;
        
        private async void Start()
        {
            AnimateLoadingIcon();
            
            loadingText.gameObject.SetActive(false);
            
#if YANDEX_GAMES && !UNITY_EDITOR
            await UniTask.WaitUntil(() => _yandex.IsSdkInit);
            _yandex.OnGameReady();
            
            await UniTask.WaitUntil(() => _yandex.IsSdkReady);
            _localization.SetLanguage(_yandex.Lang);
#endif
            
#if VK_GAMES && !UNITY_EDITOR
            await UniTask.WaitUntil(() => _vk.IsSdkInit);
            _localization.SetLanguage(_vk.Lang);
#endif
            
            AnimateLoadingText();
            loadingText.gameObject.SetActive(true);

            await _analytics.Initialize();
            await LoadSaves();
            
            ApplySavedSettings();
            
#if UNITY_EDITOR
            await UniTask.Delay(System.TimeSpan.FromSeconds(4f));
#endif
            
            _gameSceneRef.Load();
        }
                
        private async UniTask LoadSaves()
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            _saving.Load(() =>
            {
                taskCompletionSource.SetResult(true);
            });
            await taskCompletionSource.Task;
        }

        private void ApplySavedSettings()
        {
            var musicVolume = _savingService.GetMusicVolume();
            var soundVolume = _savingService.GetSoundVolume();
            
            _audio.MusicMute = musicVolume < 1f;
            _audio.SoundMute = soundVolume < 1f;
        }

        private void AnimateLoadingIcon()
        {
            var rotateFrom = Vector3.back * 10f;
            var rotateTo = Vector3.forward * 10f;

            loadingImage.rectTransform
                .DORotate(rotateTo, 2f)
                .From(rotateFrom)
                .SetLink(loadingImage.gameObject)
                .SetLoops(-1, LoopType.Yoyo);

            var scaleFrom = 1f;
            var scaleTo = 1.05f;
            
            loadingImage.rectTransform
                .DOScale(scaleTo, 4f)
                .From(scaleFrom)
                .SetLink(loadingImage.gameObject)
                .SetLoops(-1, LoopType.Yoyo);
        }
        
        private void AnimateLoadingText()
        {
            string loadingLocalized = _localization.GetString("lbl_loading");

            DOTween
                .To(
                    () => _loaderTextDots,
                    dots =>
                    {
                        _loaderTextDots = dots;
                        loadingText.text =
                            $"{loadingLocalized}{string.Concat(Enumerable.Repeat(".", Mathf.FloorToInt(_loaderTextDots)))}";
                    },
                    4f,
                    1.5f)
                .From(0f)
                .SetLink(loadingText.gameObject)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear);
        }
    }
}
