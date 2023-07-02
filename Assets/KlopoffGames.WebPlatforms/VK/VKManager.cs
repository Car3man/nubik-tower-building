using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace KlopoffGames.WebPlatforms.VK
{
    public class VKManager
    {
        private readonly UnityVKBridge _bridge;

        private bool _isSdkInit;
        private string _lang;
        private string _platform;
        
        private bool _prevMusicMute;
        private bool _prevSoundMute;

        public bool IsSdkInit => _isSdkInit;

        public string Lang => _lang;

        public string Platform => _platform;

        public delegate void SdkInitDelegate(string lang, string platform);
        private SdkInitDelegate _onSdkInit;
        public event SdkInitDelegate OnSdkInit
        {
            add {
                _onSdkInit += value;
                
                if (_isSdkInit)
                {
                    _onSdkInit?.Invoke(_lang, _platform);
                }
            }
            remove => _onSdkInit -= value;
        }

        public delegate void ViewHideDelegate();
        public event ViewHideDelegate OnViewHide;
        
        public delegate void ViewRestoreDelegate();
        public event ViewRestoreDelegate OnViewRestore;
        
        public delegate void NativeAdsOpenRequestDelegate(string adFormat);
        public event NativeAdsOpenRequestDelegate OnNativeAdsOpenRequest;

        public delegate void NativeAdsOpenResultDelegate(string adFormat, bool result);
        public event NativeAdsOpenResultDelegate OnNativeAdsOpenResult;

        public delegate void PlayerDataReceiveDelegate(Dictionary<string, object> playerData);
        public event PlayerDataReceiveDelegate OnPlayerDataReceive;
        
        public delegate void FocusStateReceivedDelegate(bool hasFocus);
        public event FocusStateReceivedDelegate OnFocusStateReceived;

        public VKManager(
            UnityVKBridge bridge
            )
        {
            Application.runInBackground = true;
            
            _bridge = bridge;
            
            _bridge.OnSdkInit += (lang, platform) =>
            {
                _isSdkInit = true;
                _lang = lang;
                _platform = platform;
                _onSdkInit?.Invoke(lang, platform);
            };
            _bridge.OnViewHide += () => { OnViewHide?.Invoke(); };
            _bridge.OnViewRestore += () => { OnViewRestore?.Invoke(); };
            _bridge.OnNativeAdsOpenRequest += adFormat => { OnNativeAdsOpenRequest?.Invoke(adFormat); };
            _bridge.OnNativeAdsOpenResult += (adFormat, result) => { OnNativeAdsOpenResult?.Invoke(adFormat, result); };
            _bridge.OnPlayerDataReceive += playerData => { OnPlayerDataReceive?.Invoke(playerData); };
            _bridge.OnFocusStateReceived += hasFocus => { OnFocusStateReceived?.Invoke(hasFocus); };
        }

        public void ShowNativeAds(string adFormat)
        {
            if (!_isSdkInit)
            {
                Debug.LogWarning("Need to wait for VK sdk initialization.");
                return;
            }
            
            UnityVKBridge.Unity2VK_ShowNativeAds(adFormat);
        }

        public void SetPlayerData(string key, string value)
        {
            UnityVKBridge.Unity2VK_SetPlayerData(key, value);
        }

        public void GetPlayerData(string[] keys)
        {
            UnityVKBridge.Unity2VK_GetPlayerData(JsonConvert.SerializeObject(keys));
        }
        
        public void ShowLeaderboardBox()
        {
            if (_platform != "web")
            {
                UnityVKBridge.Unity2VK_ShowLeaderboardBox();
            }
        }
        
        public void UpdateAndShowLeaderboardBox(int userResult)
        {
            if (_platform != "web")
            {
                UnityVKBridge.Unity2VK_UpdateAndShowLeaderboardBox(userResult);
            }
        }
        
        public void AddToFavorites()
        {
            if (_platform != "web")
            {
                UnityVKBridge.Unity2VK_AddToFavorites();
            }
        }
        
        public void ShowInviteBox()
        {
            UnityVKBridge.Unity2VK_ShowInviteBox();
        }
    }
}