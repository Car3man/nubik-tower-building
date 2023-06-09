using Zenject;

namespace KlopoffGames.Core.Audio
{
    public class AudioSourcePool : MonoMemoryPool<AudioSource>
    {
        protected override void Reinitialize(AudioSource source)
        {
            source.ResetUnityAudioSource();
        }
    }
}