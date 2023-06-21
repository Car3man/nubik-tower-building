using NubikTowerBuilding.Effects;
using Zenject;

namespace NubikTowerBuilding.Pools
{
    public class PerfectBuildIdleEffectPool : MonoMemoryPool<PerfectBuildIdleEffect>
    {
        protected override void Reinitialize(PerfectBuildIdleEffect item)
        {
            item.Stop();
        }
    }
}
