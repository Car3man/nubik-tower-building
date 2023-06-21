using NubikTowerBuilding.Effects;
using Zenject;

namespace NubikTowerBuilding.Pools
{
    public class BuildPuffEffectPool : MonoMemoryPool<BuildPuffEffect>
    {
        protected override void Reinitialize(BuildPuffEffect item)
        {
            item.Stop();
        }
    }
}