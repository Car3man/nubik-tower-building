using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace KlopoffGames.Core.Audio
{
    public class AudioManager : ITickable
    {
        private readonly AudioResourceProvider _resourceProvider;
        private readonly AudioSourcePool _pool;
        private readonly List<AudioSource> _musicAudioSources = new(1);
        private readonly List<AudioSource> _soundAudioSources = new(32);

        private bool _mute;
        private bool _musicMute;
        private bool _soundMute;
        private float _musicVolume = 1f;
        private float _soundVolume = 1f;

        public bool Mute
        {
            get => _mute;
            set
            {
                _mute = value;
                OnMuteUpdate();
                OnMuteChange?.Invoke(_mute);
            }
        }
        
        public bool MusicMute
        {
            get => _musicMute;
            set
            {
                _musicMute = value;
                OnMuteUpdate();
                OnMusicMuteChange?.Invoke(_musicMute);
            }
        }
        
        public bool SoundMute
        {
            get => _soundMute;
            set
            {
                _soundMute = value;
                OnMuteUpdate();
                OnSoundMuteChange?.Invoke(_soundMute);
            }
        }
        
        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = value;
                OnVolumeUpdate();
                OnMusicVolumeChange?.Invoke(_musicVolume);
            }
        }
        
        public float SoundVolume
        {
            get => _soundVolume;
            set
            {
                _soundVolume = value;
                OnVolumeUpdate();
                OnSoundVolumeChange?.Invoke(_soundVolume);
            }
        }
        
        public delegate void MuteChangeDelegate(bool val);
        public event MuteChangeDelegate OnMuteChange;

        public delegate void MusicMuteChangeDelegate(bool val);
        public event MusicMuteChangeDelegate OnMusicMuteChange;
        
        public delegate void SoundMuteChangeDelegate(bool val);
        public event SoundMuteChangeDelegate OnSoundMuteChange;
        
        public delegate void MusicVolumeChangeDelegate(float val);
        public event MusicVolumeChangeDelegate OnMusicVolumeChange;
        
        public delegate void SoundVolumeChangeDelegate(float val);
        public event SoundVolumeChangeDelegate OnSoundVolumeChange;

        public AudioManager(
            AudioResourceProvider resourceProvider,
            AudioSourcePool pool
            )
        {
            _resourceProvider = resourceProvider;
            _pool = pool;
        }
        
        public void Tick()
        {
            var musicAudioSourcesToRemove = new List<AudioSource>();
            for (int i = 0; i < _musicAudioSources.Count; i++)
            {
                if (!_musicAudioSources[i].unityAudioSource.isPlaying)
                {
                    _musicAudioSources[i].inactiveTime += Time.deltaTime;

                    if (_musicAudioSources[i].inactiveTime > 1f)
                    {
                        _pool.Despawn(_musicAudioSources[i]);
                        musicAudioSourcesToRemove.Add(_musicAudioSources[i]);
                    }
                }
            }
            foreach (var source in musicAudioSourcesToRemove)
            {
                _musicAudioSources.Remove(source);
            }
            
            var soundAudioSourcesToRemove = new List<AudioSource>();
            for (int i = 0; i < _soundAudioSources.Count; i++)
            {
                if (!_soundAudioSources[i].unityAudioSource.isPlaying)
                {
                    _soundAudioSources[i].inactiveTime += Time.deltaTime;

                    if (_soundAudioSources[i].inactiveTime > 1f)
                    {
                        _pool.Despawn(_soundAudioSources[i]);
                        soundAudioSourcesToRemove.Add(_soundAudioSources[i]);
                    }
                }
            }
            foreach (var source in soundAudioSourcesToRemove)
            {
                _soundAudioSources.Remove(source);
            }
        }

        public async void PlayMusicIfNotSame(string name, float volume)
        {
            if (_musicAudioSources.Count > 0)
            {
                var audioResource = await _resourceProvider.Get(name, "Music");
                if (_musicAudioSources[0].unityAudioSource.clip == audioResource)
                {
                    return;
                }
            }
            
            PlayMusic(name, volume);
        }

        public async void PlayMusic(string name, float volume)
        {
            for (int i = 0; i < _musicAudioSources.Count; i++)
            {
                _musicAudioSources[i].unityAudioSource.Stop();
            }

            var instance = _pool.Spawn();
            instance.volume = volume;
            instance.unityAudioSource.loop = true;
            instance.unityAudioSource.volume = GetFinalMusicVolume(volume);
            instance.unityAudioSource.clip = await _resourceProvider.Get(name, "Music");
            instance.unityAudioSource.Play();
            
            _musicAudioSources.Add(instance);
        }

        public async void PlaySound(string name, bool loop, float volume, float pitch)
        {
            var instance = _pool.Spawn();
            instance.volume = volume;
            instance.unityAudioSource.clip = await _resourceProvider.Get(name, "Sounds");
            instance.unityAudioSource.loop = loop;
            instance.unityAudioSource.volume = GetFinalSoundVolume(volume);
            instance.unityAudioSource.pitch = pitch;
            instance.unityAudioSource.Play();
            
            _soundAudioSources.Add(instance);
        }
        
        public void StopMusic()
        {
            for (int i = 0; i < _musicAudioSources.Count; i++)
            {
                _musicAudioSources[i].unityAudioSource.Stop();
            }
        }
        
        public async void StopSound(string name)
        {
            var audioClip = await _resourceProvider.Get(name, "Sounds");
            for (int i = 0; i < _soundAudioSources.Count; i++)
            {
                if (_soundAudioSources[i].unityAudioSource.clip == audioClip)
                {
                    _soundAudioSources[i].unityAudioSource.Stop();
                }
            }
        }

        private void OnMuteUpdate()
        {
            UpdateVolumes();
        }

        private void OnVolumeUpdate()
        {
            UpdateVolumes();
        }

        private void UpdateVolumes()
        {
            foreach (var musicAudioSource in _musicAudioSources)
            {
                musicAudioSource.unityAudioSource.volume = GetFinalMusicVolume(musicAudioSource.volume);
            }
            foreach (var soundAudioSource in _soundAudioSources)
            {
                soundAudioSource.unityAudioSource.volume = GetFinalSoundVolume(soundAudioSource.volume);
            }
        }

        private float GetFinalMusicVolume(float volume)
        {
            if (Mute)
            {
                return 0f;
            }
            if (MusicMute)
            {
                return 0f;
            }
            return MusicVolume * volume;
        }
        
        private float GetFinalSoundVolume(float volume)
        {
            if (Mute)
            {
                return 0f;
            }
            if (SoundMute)
            {
                return 0f;
            }
            return SoundVolume * volume;
        }
    }
}
