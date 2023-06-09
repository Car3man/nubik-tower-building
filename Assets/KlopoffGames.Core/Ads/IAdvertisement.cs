namespace KlopoffGames.Core.Ads
{
    public interface IAdvertisement
    {
        public delegate void InterstitialAdOpenDelegate();
        public event InterstitialAdOpenDelegate OnInterstitialAdOpen;

        public delegate void InterstitialAdvCloseDelegate();
        public event InterstitialAdvCloseDelegate OnInterstitialAdClose;

        public delegate void RewardedAdOpenDelegate();
        public event RewardedAdOpenDelegate OnRewardedAdOpen;

        public delegate void RewardedAdCloseDelegate(bool rewarded);
        public event RewardedAdCloseDelegate OnRewardedAdClose;
        
        bool IsAdShowing { get; }
        void ShowInterstitialAd();
        void ShowRewardedAd();
    }
}