﻿using KlopoffGames.Core.Ads;
using KlopoffGames.Core.Audio;

namespace KlopoffGames.WebPlatforms.VK
{
    public class VKAppFocusObserver
    {
        private readonly VKManager _vk;
        private readonly IAdvertisement _ads;
        private readonly AudioManager _audio;
        
        private bool _isFocus;
        private bool _isAd;
        private bool _savedMusicMute;
        private bool _savedSoundMute;

        public VKAppFocusObserver(
            VKManager vk,
            IAdvertisement ads,
            AudioManager audio
        )
        {
            _vk = vk;
            _ads = ads;
            _audio = audio;
            
            _savedMusicMute = _audio.MusicMute;
            _savedSoundMute = _audio.SoundMute;
            _isFocus = false;
            _isAd = false;
            UpdateMute();
            
            _vk.OnViewHide += OnViewHide;
            _vk.OnViewRestore += OnViewRestore;
            _ads.OnInterstitialAdOpen += OnAdsOpen;
            _ads.OnRewardedAdOpen += OnAdsOpen;
            _ads.OnInterstitialAdClose += OnAdsClose;
            _ads.OnRewardedAdClose += _ => { OnAdsClose(); };
        }

        private void OnViewHide()
        {
            if (_isFocus && !_isAd)
            {
                _savedMusicMute = _audio.MusicMute;
                _savedSoundMute = _audio.SoundMute;
            }

            _isFocus = false;
            UpdateMute();
        }

        private void OnViewRestore()
        {
            _isFocus = true;
            UpdateMute();
        }

        private void OnAdsOpen()
        {
            if (_isFocus && !_isAd)
            {
                _savedMusicMute = _audio.MusicMute;
                _savedSoundMute = _audio.SoundMute;
            }

            _isAd = true;
            UpdateMute();
        }

        private void OnAdsClose()
        {
            _isAd = false;
            UpdateMute();
        }

        private void UpdateMute()
        {
            if (!_isFocus || _isAd)
            {
                _audio.MusicMute = true;
                _audio.SoundMute = true;
            }
            else
            {
                _audio.MusicMute = _savedMusicMute;
                _audio.SoundMute = _savedSoundMute;
            }
        }
    }
}