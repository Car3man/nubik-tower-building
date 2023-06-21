using Cysharp.Threading.Tasks;
using KlopoffGames.Core.Ads;
using NubikTowerBuilding.Managers;
using NubikTowerBuilding.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace NubikTowerBuilding.Ui.Windows
{
    public class GameOverWindow : DefaultPopUpWindow
    {
        [Inject] private IAdvertisement _ads;
        [Inject] private GameManager _game;
        [Inject] private UserCoinsService _userCoinsService;
        [SerializeField] private TextMeshProUGUI towerHeightText;
        [SerializeField] private TextMeshProUGUI towerPopulationText;
        [SerializeField] private TextMeshProUGUI earnedCoinsText;
        [SerializeField] private Button buttonAgain;
        [SerializeField] private Button buttonHome;
        [SerializeField] private Button buttonCoinsX2;
        
        private bool _rewardedAdWatched;

        public override UniTask OnCreate()
        {
            _rewardedAdWatched = false;
            
            buttonAgain.onClick.AddListener(OnAgainButtonClick);
            buttonHome.onClick.AddListener(OnHomeButtonClick);
            buttonCoinsX2.onClick.AddListener(OnHomeButtonCoinsX2);

            towerHeightText.text = _game.ReachedHeight.ToString();
            towerPopulationText.text = _game.ReachedPopulation.ToString();
            earnedCoinsText.text = _game.EarnedCoins.ToString();

            UpdateCoinsX2Button();
            
            return base.OnCreate();
        }

        public override UniTask OnHide()
        {
            buttonAgain.onClick.RemoveAllListeners();
            buttonHome.onClick.RemoveAllListeners();
            buttonCoinsX2.onClick.RemoveAllListeners();
            
            return base.OnHide();
        }

        private void UpdateCoinsX2Button()
        {
            buttonCoinsX2.gameObject.SetActive(_game.EarnedCoins > 0 && !_rewardedAdWatched);
        }

        private void OnAgainButtonClick()
        {
            Hide();
            _game.PlayAgain();
        }

        private void OnHomeButtonClick()
        {
            Hide();
            _game.BackToLobby();
        }

        private void OnHomeButtonCoinsX2()
        {
            _ads.OnRewardedAdClose += RewardedAdClose;
            _ads.ShowRewardedAd();

            void RewardedAdClose(bool rewarded)
            {
                _ads.OnRewardedAdClose -= RewardedAdClose;

                _rewardedAdWatched = true;
                
                earnedCoinsText.text = (_game.EarnedCoins * 2).ToString();
                _userCoinsService.AddCoins(_game.EarnedCoins);
                
                UpdateCoinsX2Button();
            }
        }
    }
}
