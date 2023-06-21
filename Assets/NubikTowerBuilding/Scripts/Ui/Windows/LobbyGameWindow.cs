using Cysharp.Threading.Tasks;
using KlopoffGames.Core.Windows;
#if VK_GAMES
using KlopoffGames.WebPlatforms.VK;
#endif
using NubikTowerBuilding.Managers;
using NubikTowerBuilding.Models;
using NubikTowerBuilding.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace NubikTowerBuilding.Ui.Windows
{
    public class LobbyGameWindow : BaseWindow
    {
        [Inject] private WindowManager _windows;
        [Inject] private GameManager _game;
        [Inject] private BuildManager _buildManager;
        [Inject] private BuildingPurchaseManager _buildingPurchaseManager;
        [Inject] private UserCoinsService _userCoinsService;
#if VK_GAMES
        [Inject] private VKManager _vk;
#endif
        [SerializeField] private Button buttonSettings;
        [SerializeField] private Button buttonLeaderboard;
        [SerializeField] private Button buttonPlay;
        [SerializeField] private Button buttonChangeBlockLeft;
        [SerializeField] private Button buttonChangeBlockRight;
        [SerializeField] private Button buttonPurchaseBlock;
        [SerializeField] private TextMeshProUGUI buttonPurchasePriceText;
        [SerializeField] private TextMeshProUGUI coinsText;

        public override UniTask OnCreate()
        {
#if VK_GAMES
            buttonLeaderboard.gameObject.SetActive(true);
#else
            buttonLeaderboard.gameObject.SetActive(false);
#endif
            
            buttonSettings.onClick.AddListener(OnButtonSettingsClick);
            buttonLeaderboard.onClick.AddListener(OnButtonLeaderboardClick);
            buttonPlay.onClick.AddListener(OnButtonPlayClick);
            buttonChangeBlockLeft.onClick.AddListener(OnButtonChangeBlockLeftClick);
            buttonChangeBlockRight.onClick.AddListener(OnButtonChangeBlockRightClick);
            buttonPurchaseBlock.onClick.AddListener(OnButtonPurchaseBlockClick);
            _userCoinsService.OnCoinsChange += OnCoinsChange;

            UpdatePurchaseButton();
            UpdateStartButton();
            
            return UniTask.CompletedTask;
        }

        public override UniTask OnHide()
        {
            buttonSettings.onClick.RemoveAllListeners();
            buttonLeaderboard.onClick.RemoveAllListeners();
            buttonPlay.onClick.RemoveAllListeners();
            buttonChangeBlockLeft.onClick.RemoveAllListeners();
            buttonChangeBlockRight.onClick.RemoveAllListeners();
            buttonPurchaseBlock.onClick.RemoveAllListeners();
            _userCoinsService.OnCoinsChange -= OnCoinsChange;
            
            return UniTask.CompletedTask;
        }

        private void UpdatePurchaseButton()
        {
            var currBuilding = _buildManager.GetBuildingBlockType();
            var isCurrBuildingPurchased = _buildingPurchaseManager.IsPurchased(currBuilding);
            buttonPurchaseBlock.gameObject.SetActive(!isCurrBuildingPurchased);
            buttonPurchaseBlock.interactable = _buildingPurchaseManager.EnoughCoinsToPurchase(currBuilding);
            buttonPurchasePriceText.text = _buildingPurchaseManager.GetPrice(currBuilding).ToString();
        }

        private void UpdateStartButton()
        {
            var currBuilding = _buildManager.GetBuildingBlockType();
            var isCurrBuildingPurchased = _buildingPurchaseManager.IsPurchased(currBuilding);
            buttonPlay.interactable = isCurrBuildingPurchased;
        }
        
        private void OnCoinsChange(int coins)
        {
            coinsText.text = _userCoinsService.GetCoins().ToString();
        }
        
        private void OnButtonSettingsClick()
        {
            _windows.CreateWindow<SettingsWindow>("Windows/Game");
        }

        private void OnButtonLeaderboardClick()
        {
#if VK_GAMES
            _vk.ShowLeaderboardBox();
#endif
        }

        private void OnButtonPlayClick()
        {
            Hide();
            _game.StartGame();
        }

        private void OnButtonChangeBlockLeftClick()
        {
            var currBlockTypeVal = (int)_buildManager.GetBuildingBlockType();
            currBlockTypeVal--;
            if (currBlockTypeVal < 0)
            {
                currBlockTypeVal = (int)BuildingBlockType.MaxNbr - 1;
            }
            _buildManager.SetBuildingBlockType((BuildingBlockType)currBlockTypeVal);
            
            UpdatePurchaseButton();
            UpdateStartButton();
        }

        private void OnButtonChangeBlockRightClick()
        {
            var currBlockTypeVal = (int)_buildManager.GetBuildingBlockType();
            currBlockTypeVal++;
            if (currBlockTypeVal >= (int)BuildingBlockType.MaxNbr)
            {
                currBlockTypeVal = 0;
            }
            _buildManager.SetBuildingBlockType((BuildingBlockType)currBlockTypeVal);
            
            UpdatePurchaseButton();
            UpdateStartButton();
        }

        private void OnButtonPurchaseBlockClick()
        {
            _buildingPurchaseManager.Purchase(_buildManager.GetBuildingBlockType());
            
            UpdatePurchaseButton();
            UpdateStartButton();
        }
    }
}
