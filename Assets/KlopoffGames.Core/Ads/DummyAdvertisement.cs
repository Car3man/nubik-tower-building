using UnityEngine;

namespace KlopoffGames.Core.Ads
{
    public class DummyAdvertisement : IAdvertisement
    {
        public float LastAdRequestTime { get; private set; }
        public bool IsAdShowing => false;
        
        public event IAdvertisement.InterstitialAdOpenDelegate OnInterstitialAdOpen;
        public event IAdvertisement.InterstitialAdvCloseDelegate OnInterstitialAdClose;
        public event IAdvertisement.RewardedAdOpenDelegate OnRewardedAdOpen;
        public event IAdvertisement.RewardedAdCloseDelegate OnRewardedAdClose;

        public void ShowInterstitialAd()
        {
            LastAdRequestTime = Time.time;
            OnInterstitialAdOpen?.Invoke();
            OnInterstitialAdClose?.Invoke();
        }

        public void ShowRewardedAd()
        {
            LastAdRequestTime = Time.time;
            OnRewardedAdOpen?.Invoke();
            OnRewardedAdClose?.Invoke(true);
        }
    }
}