using KlopoffGames.Core.Ads;

namespace KlopoffGames.WebPlatforms.Yandex
{
    public class YandexAdvertisement : IAdvertisement
    {
        private readonly YandexManager _yandex;

        public bool IsAdShowing { get; private set; }

        public event IAdvertisement.InterstitialAdOpenDelegate OnInterstitialAdOpen;
        public event IAdvertisement.InterstitialAdvCloseDelegate OnInterstitialAdClose;
        public event IAdvertisement.RewardedAdOpenDelegate OnRewardedAdOpen;
        public event IAdvertisement.RewardedAdCloseDelegate OnRewardedAdClose;

        public YandexAdvertisement(
            YandexManager yandex
            )
        {
            _yandex = yandex;
            
            _yandex.OnFullscreenAdvOpen += Yandex_OnFullscreenAdvOpen;
            _yandex.OnFullscreenAdvClose += Yandex_OnFullscreenAdvClose;
            _yandex.OnRewardedAdvOpen += Yandex_OnRewardedAdvOpen;
            _yandex.OnRewardedAdvClose += Yandex_OnRewardedAdvClose;
        }

        public void ShowInterstitialAd()
        {
            _yandex.ShowFullscreenAdv();
        }

        public void ShowRewardedAd()
        {
            _yandex.ShowRewardedAdv();
        }
        
        private void Yandex_OnFullscreenAdvOpen()
        {
            IsAdShowing = true;
            OnInterstitialAdOpen?.Invoke();
        }

        private void Yandex_OnFullscreenAdvClose()
        {
            IsAdShowing = false;
            OnInterstitialAdClose?.Invoke();
        }
        
        private void Yandex_OnRewardedAdvOpen()
        {
            IsAdShowing = true;
            OnRewardedAdOpen?.Invoke();
        }

        private void Yandex_OnRewardedAdvClose(bool rewarded)
        {
            IsAdShowing = false;
            OnRewardedAdClose?.Invoke(rewarded);
        }
    }
}