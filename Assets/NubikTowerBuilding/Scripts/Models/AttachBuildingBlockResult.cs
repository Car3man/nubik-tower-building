using NubikTowerBuilding.Behaviours;

namespace NubikTowerBuilding.Models
{
    public struct AttachBuildingBlockResult
    {
        public BuildingBlock AnchorBlock;
        public BuildingBlock ConnectedBlock;
        public bool IsRootBlock;
        public bool IsSuccess;
        public bool IsPerfect;
    }
}
