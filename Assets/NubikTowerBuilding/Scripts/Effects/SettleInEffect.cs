using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NubikTowerBuilding.Effects
{
    public class SettleInEffect : MonoBehaviour
    {
        [SerializeField] private MeshRenderer umbrellaMeshRenderer;
        [SerializeField] private Material redUmbrellaMaterial;
        [SerializeField] private Material greenUmbrellaMaterial;
        [SerializeField] private Material blueUmbrellaMaterial;
        
        public async void Play(Vector3 flyFrom, Transform flyTo,
            float flyDuration, System.Action<SettleInEffect> onEnd)
        {
            RandomizeUmbrella(); 
            
            var scaleFromT = 0.95f;
            var flyToTrans = flyTo.transform;
            var time = 0f;
            
            transform.position = flyFrom;
            while (time < flyDuration)
            {
                await UniTask.WaitForEndOfFrame(this);
                if (flyTo == null || flyTo.gameObject == null)
                {
                    break;
                }
                var t = time / flyDuration;
                transform.position = Vector3.Lerp(flyFrom, flyToTrans.position, t);
                if (t > scaleFromT)
                {
                    float scaleT = 1f - (1f - t) / (1f - scaleFromT);
                    transform.localScale = Vector3.Slerp(Vector3.one, Vector3.zero, scaleT);
                }
                transform.rotation = Quaternion.LookRotation(flyToTrans.position - transform.position);
                time += Time.deltaTime;
            }
            
            onEnd?.Invoke(this);
        }

        private void RandomizeUmbrella()
        {
            int random = Random.Range(0, 3);
            umbrellaMeshRenderer.material = random switch
            {
                0 => redUmbrellaMaterial,
                1 => greenUmbrellaMaterial,
                2 => blueUmbrellaMaterial,
                _ => redUmbrellaMaterial
            };
        }
    }
}