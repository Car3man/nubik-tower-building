using NubikTowerBuilding.Effects;
using NubikTowerBuilding.Pools;
using UnityEngine;

namespace NubikTowerBuilding.Spawners
{
    public class PerfectBuildSplashEffectSpawner
    {
        private readonly PerfectBuildSplashEffectPool _pool;

        public PerfectBuildSplashEffectSpawner(
            PerfectBuildSplashEffectPool pool
        )
        {
            _pool = pool;
        }

        public PerfectBuildSplashEffect Spawn(Vector3 position, float scale)
        {
            var instance = _pool.Spawn();
            var instanceTrans = instance.transform;
            instanceTrans.position = position;
            instanceTrans.localScale = Vector3.one * scale;
            instance.Play(effect =>
            {
                _pool.Despawn(effect);
            });
            return instance;
        }
    }
}