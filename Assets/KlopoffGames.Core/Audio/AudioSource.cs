using UnityEngine;

namespace KlopoffGames.Core.Audio
{
    public class AudioSource : MonoBehaviour
    {
        public float volume;
        public float inactiveTime;
        public UnityEngine.AudioSource unityAudioSource;

        public void ResetUnityAudioSource()
        {
            volume = 1f;
            inactiveTime = 0f;
            unityAudioSource.playOnAwake = false;
            unityAudioSource.clip = null;
            unityAudioSource.loop = false;
            unityAudioSource.mute = false;
            unityAudioSource.pitch = 1f;
            unityAudioSource.priority = 128;
            unityAudioSource.volume = 1f;
        }
    }
}