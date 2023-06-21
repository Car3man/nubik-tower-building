using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace KlopoffGames.WebPlatforms.Yandex
{
    public class YandexManager
    {
        private readonly UnityYandexBridge _bridge;

        private bool _isSdkInit;
        private bool _isSdkReady;
        private bool _isGameReady;

        private string _lang;
        private bool _playerAuthorized;
        private string _lastPlayerDataRaw;

        public bool IsSdkInit => _isSdkInit;
        
        public bool IsSdkReady => _isSdkReady;

        public string Lang => _lang;

        public bool PlayerAuthorized => _playerAuthorized;

        public delegate void SdkInitDelegate();
        private SdkInitDelegate _onSdkInit;
        public event SdkInitDelegate OnSdkInit
        {
            add {
                _onSdkInit += value;
                
                if (_isSdkInit)
                {
                    _onSdkInit?.Invoke();
                }
            }
            remove => _onSdkInit -= value;
        }
        
        public delegate void SdkReadyDelegate(string lang, bool playerAuthorized);
        private SdkReadyDelegate _onSdkReady;
        public event SdkReadyDelegate OnSdkReady
        {
            add {
                _onSdkReady += value;
                
                if (_isSdkReady)
                {
                    _onSdkReady?.Invoke(_lang, _playerAuthorized);
                }
            }
            remove => _onSdkReady -= value;
        }
        
        public delegate void ViewHideDelegate();
        public event ViewHideDelegate OnViewHide;
        
        public delegate void ViewRestoreDelegate();
        public event ViewRestoreDelegate OnViewRestore;

        public delegate void FullscreenAdvOpenDelegate();
        public event FullscreenAdvOpenDelegate OnFullscreenAdvOpen;

        public delegate void FullscreenAdvCloseDelegate();
        public event FullscreenAdvCloseDelegate OnFullscreenAdvClose;

        public delegate void RewardedAdvOpenDelegate();
        public event RewardedAdvOpenDelegate OnRewardedAdvOpen;

        public delegate void RewardedAdvCloseDelegate(bool rewarded);
        public event RewardedAdvCloseDelegate OnRewardedAdvClose;
        
        public delegate void PlayerDataReceiveDelegate(Dictionary<string, object> playerData);
        public event PlayerDataReceiveDelegate OnPlayerDataReceive;

        public YandexManager(
            UnityYandexBridge bridge
            )
        {
            Application.runInBackground = true;
            
            _bridge = bridge;
            _bridge.OnSdkInit += () =>
            {
                _isSdkInit = true;
                _onSdkInit?.Invoke();
            };
            _bridge.OnSdkReady += (lang, playerAuthorized) =>
            {
                _isSdkReady = true;
                _lang = lang;
                _playerAuthorized = playerAuthorized;
                _onSdkReady?.Invoke(lang, playerAuthorized);
            };
            _bridge.OnViewHide += () => { OnViewHide?.Invoke(); };
            _bridge.OnViewRestore += () => { OnViewRestore?.Invoke(); };
            _bridge.OnFullscreenAdvOpen += () => { OnFullscreenAdvOpen?.Invoke(); };
            _bridge.OnFullscreenAdvClose += () => { OnFullscreenAdvClose?.Invoke(); };
            _bridge.OnRewardedAdvOpen += () => { OnRewardedAdvOpen?.Invoke(); };
            _bridge.OnRewardedAdvClose += rewarded => { OnRewardedAdvClose?.Invoke(rewarded); };
            _bridge.OnPlayerDataReceive += playerData => { OnPlayerDataReceive?.Invoke(playerData); };
        }
        
        public void OnGameReady()
        {
            if (!_isSdkInit)
            {
                Debug.LogWarning("Wait for yandex sdk init to call OnGameReady");
                return;
            }
            
            if (_isGameReady)
            {
                Debug.LogWarning("You try to call OnGameReady while game was ready before.");
                return;
            }
            
            UnityYandexBridge.Unity2Yandex_OnGameReady();

            _isGameReady = true;
        }

        public void OpenAuthDialog()
        {
            if (!_isGameReady)
            {
                Debug.LogWarning("Please call OnGameReady before use this method.");
                return;
            }
            
            UnityYandexBridge.Unity2Yandex_OpenAuthDialog();
        }
        
        public void ShowFullscreenAdv()
        {
            if (!_isGameReady)
            {
                Debug.LogWarning("Please call OnGameReady before use this method.");
                return;
            }
            
            UnityYandexBridge.Unity2Yandex_ShowFullscreenAdv();
        }
        
        public void ShowRewardedAdv()
        {
            if (!_isGameReady)
            {
                Debug.LogWarning("Please call OnGameReady before use this method.");
                return;
            }
            
            UnityYandexBridge.Unity2Yandex_ShowRewardedAdv();
        }

        public void SetPlayerData(Dictionary<string, object> data, bool flush)
        {
            var playerDataRaw = JsonConvert.SerializeObject(data);
            if (_lastPlayerDataRaw == playerDataRaw)
            {
                return;
            }
            
            UnityYandexBridge.Unity2Yandex_SetPlayerData(playerDataRaw, flush);
        }

        public void GetPlayerData(string[] keys)
        {
            UnityYandexBridge.Unity2Yandex_GetPlayerData(JsonConvert.SerializeObject(keys));
        }
        
        public void SetLeaderboardScore(string leaderboardName, int score)
        {
            UnityYandexBridge.Unity2Yandex_SetLeaderboardScore(leaderboardName, score);
        }
    }
}