using System.Collections.Generic;
using KlopoffGames.Core.Saving;
using NubikTowerBuilding.Models;

namespace NubikTowerBuilding.Services
{
    public class SavingService
    {
        private readonly ISavingManager _saving;

        public SavingService(
            ISavingManager savings
            )
        {
            _saving = savings;
            _saving.SetAvailableKeys(GetAvailableKeys());
        }

        private string[] GetAvailableKeys()
        {
            var keys = new List<string>();
            keys.Add("is_app_share_disabled");
            keys.Add("is_ftue");
            keys.Add("music_volume");
            keys.Add("sound_volume");
            keys.Add("last_block_played");
            keys.Add("coins");
            for (int i = 0; i < (int)BuildingBlockType.MaxNbr; i++)
            {
                keys.Add($"purchased_building_block_{i}");
            }
            return keys.ToArray();
        }
        
        public bool GetIsAppShareDisabled()
        {
            return _saving.GetBool("is_app_share_disabled", false);
        }

        public void SetIsAppShareDisabled(bool value)
        {
            _saving.SetBool("is_app_share_disabled", value);
        }

        public bool GetIsFTUE()
        {
            return _saving.GetBool("is_ftue", true);
        }

        public void SetIsFTUE(bool value)
        {
            _saving.SetBool("is_ftue", value);
        }

        public float GetMusicVolume()
        {
            return _saving.GetFloat("music_volume", 1f);
        }

        public void SetMusicVolume(float volume)
        {
            _saving.SetFloat("music_volume", volume);
        }

        public float GetSoundVolume()
        {
            return _saving.GetFloat("sound_volume", 1f);
        }

        public void SetSoundVolume(float volume)
        {
            _saving.SetFloat("sound_volume", volume);
        }

        public int GetCoins()
        {
            return _saving.GetInt("coins", 0);
        }

        public void SetCoins(int coins)
        {
            _saving.SetInt("coins", coins);
        }
        
        public bool GetIsBuildingBlockPurchased(BuildingBlockType buildingBlockType)
        {
            return _saving.GetBool($"purchased_building_block_{(int)buildingBlockType}", false);
        }

        public void SetIsBuildingBlockPurchased(BuildingBlockType buildingBlockType, bool purchased)
        {
            _saving.SetBool($"purchased_building_block_{(int)buildingBlockType}", purchased);
        }
        
        public BuildingBlockType GetLastBlockPlayed()
        {
            return (BuildingBlockType)_saving.GetInt($"last_block_played", 0);
        }

        public void SetLastBlockPlayed(BuildingBlockType buildingBlockType)
        {
            _saving.SetInt($"last_block_played", (int)buildingBlockType);
        }
    }
}