using NubikTowerBuilding.Effects;
using NubikTowerBuilding.Pools;
using UnityEngine;

namespace NubikTowerBuilding.Spawners
{
    public class SettleInEffectSpawner
    {
        private readonly SettleInEffectPool _pool;

        public SettleInEffectSpawner(
            SettleInEffectPool pool
        )
        {
            _pool = pool;
        }

        public SettleInEffect Spawn(Vector3 position, Transform destination, float duration)
        {
            var instance = _pool.Spawn();
            var instanceTrans = instance.transform;
            instanceTrans.position = position;
            instance.Play(position, destination, duration, effect =>
            {
                _pool.Despawn(effect);
            });
            return instance;
        }
    }
}