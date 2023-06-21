using KlopoffGames.Core.Ads;

namespace KlopoffGames.WebPlatforms.VK
{
    public class VKAdvertisement : IAdvertisement
    {
        private readonly VKManager _vk;
        
        public bool IsAdShowing { get; private set; }

        public event IAdvertisement.InterstitialAdOpenDelegate OnInterstitialAdOpen;
        public event IAdvertisement.InterstitialAdvCloseDelegate OnInterstitialAdClose;
        public event IAdvertisement.RewardedAdOpenDelegate OnRewardedAdOpen;
        public event IAdvertisement.RewardedAdCloseDelegate OnRewardedAdClose;

        public VKAdvertisement(
            VKManager vk
            )
        {
            _vk = vk;
            
            _vk.OnNativeAdsOpenRequest += VK_OnNativeAdsOpenRequest;
            _vk.OnNativeAdsOpenResult += VK_OnNativeAdsOpenResult;
        }

        public void ShowInterstitialAd()
        {
            _vk.ShowNativeAds("interstitial");
        }

        public void ShowRewardedAd()
        {
            _vk.ShowNativeAds("reward");
        }
        
        private void VK_OnNativeAdsOpenRequest(string adFormat)
        {
            IsAdShowing = true;
            
            if (adFormat == "reward")
            {
                OnRewardedAdOpen?.Invoke();
            }
            else
            {
                OnInterstitialAdOpen?.Invoke();
            }
        }

        private void VK_OnNativeAdsOpenResult(string adFormat, bool result)
        {
            IsAdShowing = false;
            
            if (adFormat == "reward")
            {
                OnRewardedAdClose?.Invoke(result);
            }
            else
            {
                OnInterstitialAdClose?.Invoke();
            }
        }
    }
}