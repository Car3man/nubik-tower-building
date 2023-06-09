namespace KlopoffGames.Core.Ads
{
    public class DummyAdvertisement : IAdvertisement
    {
        public bool IsAdShowing => false;
        
        public event IAdvertisement.InterstitialAdOpenDelegate OnInterstitialAdOpen;
        public event IAdvertisement.InterstitialAdvCloseDelegate OnInterstitialAdClose;
        public event IAdvertisement.RewardedAdOpenDelegate OnRewardedAdOpen;
        public event IAdvertisement.RewardedAdCloseDelegate OnRewardedAdClose;

        public void ShowInterstitialAd()
        {
            OnInterstitialAdOpen?.Invoke();
            OnInterstitialAdClose?.Invoke();
        }

        public void ShowRewardedAd()
        {
            OnRewardedAdOpen?.Invoke();
            OnRewardedAdClose?.Invoke(true);
        }
    }
}