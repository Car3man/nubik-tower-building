using NubikTowerBuilding.Effects;
using NubikTowerBuilding.Pools;
using UnityEngine;

namespace NubikTowerBuilding.Spawners
{
    public class BuildPuffEffectSpawner
    {
        private readonly BuildPuffEffectPool _pool;

        public BuildPuffEffectSpawner(
            BuildPuffEffectPool pool
        )
        {
            _pool = pool;
        }

        public BuildPuffEffect Spawn(Vector3 position, float scale)
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