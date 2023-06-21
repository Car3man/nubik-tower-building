using NubikTowerBuilding.Effects;
using Zenject;

namespace NubikTowerBuilding.Pools
{
    public class PerfectBuildSplashEffectPool : MonoMemoryPool<PerfectBuildSplashEffect>
    {
        protected override void Reinitialize(PerfectBuildSplashEffect item)
        {
            item.Stop();
        }
    }
}