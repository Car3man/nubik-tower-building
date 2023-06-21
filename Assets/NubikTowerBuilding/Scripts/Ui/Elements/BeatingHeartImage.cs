using DG.Tweening;
using UnityEngine;

namespace NubikTowerBuilding.Ui.Elements
{
    public class BeatingHeartImage : MonoBehaviour
    {
        private void Start()
        {
            var scaleTo = new Vector3(1.1f, 1.1f, 1f);
            var scaleFrom = Vector3.one;
            var sequence = DOTween.Sequence(gameObject);
            sequence.Append(transform
                .DOScale(scaleTo, 0.2f)
                .From(scaleFrom));
            sequence.Append(transform
                .DOScale(scaleFrom, 0.2f)
                .From(scaleTo));
            sequence.Append(transform
                .DOScale(scaleTo, 0.2f)
                .From(scaleFrom));
            sequence.Append(transform
                .DOScale(scaleFrom, 0.2f)
                .From(scaleTo));
            sequence
                .SetDelay(1f)
                .SetLink(gameObject)
                .SetLoops(-1, LoopType.Restart);
        }
    }
}
