using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NubikTowerBuilding.Effects
{
    public class PerfectBuildSplashEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem fxParticles;

        public async void Play(System.Action<PerfectBuildSplashEffect> onEnd)
        {
            fxParticles.Play();
            await UniTask.WaitUntil(
                () => !fxParticles.isPlaying, 
                PlayerLoopTiming.Update,
                fxParticles.GetCancellationTokenOnDestroy()
            );
            onEnd?.Invoke(this);
        }

        public void Stop()
        {
            fxParticles.Stop(true);
        }
    }
}