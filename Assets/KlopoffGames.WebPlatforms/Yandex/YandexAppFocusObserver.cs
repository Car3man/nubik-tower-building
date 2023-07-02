using KlopoffGames.Core.Ads;
using KlopoffGames.Core.Audio;
using Zenject;

namespace KlopoffGames.WebPlatforms.Yandex
{
    public class YandexAppFocusObserver : ITickable
    {
        private readonly YandexManager _yandex;
        private readonly IAdvertisement _ads;
        private readonly AudioManager _audio;
        
        private bool _hasFocus;
        private bool _isAd;

        public YandexAppFocusObserver(
            YandexManager yandex,
            IAdvertisement ads,
            AudioManager audio
            )
        {
            _yandex = yandex;
            _ads = ads;
            _audio = audio;
            
            _hasFocus = false;
            _isAd = false;
            
            _yandex.OnFocusStateReceived += OnFocusStateReceived;
            _ads.OnInterstitialAdOpen += OnAdsOpen;
            _ads.OnRewardedAdOpen += OnAdsOpen;
            _ads.OnInterstitialAdClose += OnAdsClose;
            _ads.OnRewardedAdClose += _ => { OnAdsClose(); };
        }

        private void OnFocusStateReceived(bool hasFocus)
        {
            _hasFocus = hasFocus;
        }

        private void OnAdsOpen()
        {
            _isAd = true;
        }

        private void OnAdsClose()
        {
            _isAd = false;
        }

        public void Tick()
        {
            _audio.Mute = !_hasFocus || _isAd;
        }
    }
}