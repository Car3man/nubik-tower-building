using KlopoffGames.Core.Ads;
using KlopoffGames.Core.Audio;
using Zenject;

namespace KlopoffGames.WebPlatforms.VK
{
    public class VKAppFocusObserver : ITickable
    {
        private readonly VKManager _vk;
        private readonly IAdvertisement _ads;
        private readonly AudioManager _audio;

        private bool _hasFocus;
        private bool _isAd;

        public VKAppFocusObserver(
            VKManager vk,
            IAdvertisement ads,
            AudioManager audio
        )
        {
            _vk = vk;
            _ads = ads;
            _audio = audio;
            
            _hasFocus = false;
            _isAd = false;
            
            _vk.OnFocusStateReceived += OnFocusStateReceived;
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