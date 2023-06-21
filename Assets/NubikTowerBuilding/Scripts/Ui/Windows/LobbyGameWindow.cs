using Cysharp.Threading.Tasks;
using KlopoffGames.Core.Windows;
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
        [SerializeField] private Button buttonSettings;
        [SerializeField] private Button buttonStart;
        [SerializeField] private Button buttonChangeBlockLeft;
        [SerializeField] private Button buttonChangeBlockRight;
        [SerializeField] private Button buttonPurchaseBlock;
        [SerializeField] private TextMeshProUGUI buttonPurchasePriceText;
        [SerializeField] private TextMeshProUGUI coinsText;

        public override UniTask OnCreate()
        {
            buttonSettings.onClick.AddListener(OnButtonSettingsClick);
            buttonStart.onClick.AddListener(OnButtonStartClick);
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
            buttonStart.onClick.RemoveAllListeners();
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
            buttonStart.interactable = isCurrBuildingPurchased;
        }
        
        private void OnCoinsChange(int coins)
        {
            coinsText.text = _userCoinsService.GetCoins().ToString();
        }
        
        private void OnButtonSettingsClick()
        {
            _windows.CreateWindow<SettingsWindow>("Windows/Game");
        }

        private void OnButtonStartClick()
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
