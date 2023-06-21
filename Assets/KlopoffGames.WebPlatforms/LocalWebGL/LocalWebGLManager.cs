using KlopoffGames.Core.Audio;
using UnityEngine;

namespace KlopoffGames.WebPlatforms.LocalWebGL
{
    public class LocalWebGLManager
    {
        private readonly UnityLocalWebGLBridge _bridge;
        
        private bool _prevMusicMute;
        private bool _prevSoundMute;
        
        public LocalWebGLManager(
            UnityLocalWebGLBridge bridge,
            AudioManager audio
        )
        {
            Application.runInBackground = true;
            
            _bridge = bridge;
            _bridge.OnViewHide += () =>
            {
                _prevMusicMute = audio.MusicMute;
                _prevSoundMute = audio.SoundMute;
            
                audio.MusicMute = true;
                audio.SoundMute = true;
            };
            _bridge.OnViewRestore += () =>
            {
                audio.MusicMute = _prevMusicMute;
                audio.SoundMute = _prevSoundMute;
            };
        }
    }
}
