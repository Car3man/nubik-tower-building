using NubikTowerBuilding.Effects;
using NubikTowerBuilding.Pools;
using UnityEngine;

namespace NubikTowerBuilding.Spawners
{
    public class PerfectBuildIdleEffectSpawner
    {
        private readonly PerfectBuildIdleEffectPool _pool;

        public PerfectBuildIdleEffectSpawner(
            PerfectBuildIdleEffectPool pool
        )
        {
            _pool = pool;
        }

        public PerfectBuildIdleEffect Spawn(Vector3 position, float scale)
        {
            var instance = _pool.Spawn();
            var instanceTrans = instance.transform;
            instanceTrans.position = position;
            instanceTrans.localScale = Vector3.one * scale;
            instance.Play(null);
            return instance;
        }
        
        public void Despawn(PerfectBuildIdleEffect perfectBuildIdleEffect)
        {
            _pool.Despawn(perfectBuildIdleEffect);
        }
    }
}