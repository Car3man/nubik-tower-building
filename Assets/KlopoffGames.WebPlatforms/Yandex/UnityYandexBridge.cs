using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using UnityEngine;

namespace KlopoffGames.WebPlatforms.Yandex
{
    public class UnityYandexBridge : MonoBehaviour
    {
        [DllImport("__Internal")]
        public static extern void Unity2Yandex_OnGameReady();
        
        [DllImport("__Internal")]
        public static extern void Unity2Yandex_OpenAuthDialog();
        
        [DllImport("__Internal")]
        public static extern void Unity2Yandex_ShowFullscreenAdv();
        
        [DllImport("__Internal")]
        public static extern void Unity2Yandex_ShowRewardedAdv();

        [DllImport("__Internal")]
        public static extern void Unity2Yandex_SetPlayerData(string data, bool flush);
        
        [DllImport("__Internal")]
        public static extern void Unity2Yandex_GetPlayerData(string keys);
        
        [DllImport("__Internal")]
        public static extern void Unity2Yandex_SetLeaderboardScore(string leaderboardName, int score);

        public delegate void SdkInitDelegate();
        public event SdkInitDelegate OnSdkInit;
        
        public delegate void SdkReadyDelegate(string lang, bool playerAuthorized);
        public event SdkReadyDelegate OnSdkReady;
        
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
        
        public delegate void FocusStateReceivedDelegate(bool hasFocus);
        public event FocusStateReceivedDelegate OnFocusStateReceived;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
        
        private void Yandex2Unity_OnSdkInit(string payload)
        {
            OnSdkInit?.Invoke();
        }
        
        private void Yandex2Unity_OnViewHide()
        {
            OnViewHide?.Invoke();
        }
        
        private void Yandex2Unity_OnViewRestore()
        {
            OnViewRestore?.Invoke();
        }
        
        private void Yandex2Unity_OnSdkReady(string payload)
        {
            var template = new
            {
                lang = "",
                playerAuthoized = false
            };
            var parsedPayload = JsonConvert.DeserializeAnonymousType(payload, template);
            OnSdkReady?.Invoke(
                parsedPayload.lang, 
                parsedPayload.playerAuthoized
            );
        }

        private void Yandex2Unity_OnFullscreenAdvOpen(string payload)
        {
            OnFullscreenAdvOpen?.Invoke();
        }

        private void Yandex2Unity_OnFullscreenAdvClose(string payload)
        {
            OnFullscreenAdvClose?.Invoke();
        }

        private void Yandex2Unity_OnRewardedAdvOpen(string payload)
        {
            OnRewardedAdvOpen?.Invoke();
        }
        
        private void Yandex2Unity_OnRewardedAdvClose(string payload)
        {
            var template = new { rewarded = false };
            var parsedPayload = JsonConvert.DeserializeAnonymousType(payload, template);
            OnRewardedAdvClose?.Invoke(parsedPayload.rewarded);
        }

        private void Yandex2Unity_OnPlayerDataReceive(string payload)
        {
            var parsedPayload = JsonConvert.DeserializeObject<Dictionary<string, object>>(payload);
            OnPlayerDataReceive?.Invoke(parsedPayload);
        }
        
        private void Yandex2Unity_OnFocusStateReceived(string payload)
        {
            var template = new
            {
                hasFocus = false
            };
            var parsedPayload = JsonConvert.DeserializeAnonymousType(payload, template);
            OnFocusStateReceived?.Invoke(parsedPayload.hasFocus);
        }
    }
}