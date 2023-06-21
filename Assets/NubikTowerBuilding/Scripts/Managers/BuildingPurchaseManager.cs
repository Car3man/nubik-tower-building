using System.Collections.Generic;
using NubikTowerBuilding.Models;
using NubikTowerBuilding.Services;
using Zenject;

namespace NubikTowerBuilding.Managers
{
    public class BuildingPurchaseManager
    {
        [Inject] private SavingService _saving;
        [Inject] private UserCoinsService _userCoinsService;
        
        private readonly Dictionary<BuildingBlockType, int> _prices = new()
        {
            { BuildingBlockType.House1, 0 },
            { BuildingBlockType.House2, 150 },
            { BuildingBlockType.House3, 300 },
            { BuildingBlockType.House4, 600 },
            { BuildingBlockType.House5, 1200 }
        };

        public bool EnoughCoinsToPurchase(BuildingBlockType buildingBlockType)
        {
            return _userCoinsService.GetCoins() >= GetPrice(buildingBlockType);
        }

        public bool IsPurchased(BuildingBlockType buildingBlockType)
        {
            return GetPrice(buildingBlockType) == 0 ||
                   _saving.GetIsBuildingBlockPurchased(buildingBlockType);
        }
        
        public int GetPrice(BuildingBlockType buildingBlockType)
        {
            return _prices[buildingBlockType];
        }

        public void Purchase(BuildingBlockType buildingBlockType)
        {
            if (!EnoughCoinsToPurchase(buildingBlockType))
            {
                return;
            }
            
            _userCoinsService.SubtractCoins(GetPrice(buildingBlockType));
            _saving.SetIsBuildingBlockPurchased(buildingBlockType, true);
        }
    }
}
