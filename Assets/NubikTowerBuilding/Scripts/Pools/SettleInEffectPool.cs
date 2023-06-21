using NubikTowerBuilding.Effects;
using UnityEngine;
using Zenject;

namespace NubikTowerBuilding.Pools
{
    public class SettleInEffectPool : MonoMemoryPool<SettleInEffect>
    {
        protected override void Reinitialize(SettleInEffect item)
        {
            item.transform.localScale = Vector3.one;
        }
    }
}