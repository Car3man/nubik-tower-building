using System.Collections.Generic;
using UnityEngine;

namespace NubikTowerBuilding.Behaviours
{
    public class Tower : MonoBehaviour
    {
        [SerializeField] private Rigidbody platform;
        
        private readonly List<BuildingBlock> _buildingBlocks = new();

        public bool IsEmpty()
        {
            return GetHeight() == 0;
        }

        public int GetHeight()
        {
            return _buildingBlocks.Count;
        }
        
        public void AddBuildingBlock(BuildingBlock buildingBlock)
        {
            var wasEmpty = IsEmpty();
            _buildingBlocks.Add(buildingBlock);
            
            if (wasEmpty)
            {
                buildingBlock.ConnectToPlatform(platform);
            }
            else
            {
                buildingBlock.ConnectToBlock(_buildingBlocks[^2]);
            }
        }
    }
}
