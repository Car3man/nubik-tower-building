using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using UnityEngine;

namespace KlopoffGames.WebPlatforms.VK
{
    public class UnityVKBridge : MonoBehaviour
    {
        [DllImport("__Internal")]
        public static extern void Unity2VK_ShowNativeAds(string adFormat);
        
        [DllImport("__Internal")]
        public static extern void Unity2VK_SetPlayerData(string key, string value);
        
        [DllImport("__Internal")]
        public static extern void Unity2VK_GetPlayerData(string keys);
        
        [DllImport("__Internal")]
        public static extern void Unity2VK_ShowLeaderboardBox();
        
        [DllImport("__Internal")]
        public static extern void Unity2VK_UpdateAndShowLeaderboardBox(int userResult);
        
        [DllImport("__Internal")]
        public static extern void Unity2VK_AddToFavorites();
        
        [DllImport("__Internal")]
        public static extern void Unity2VK_ShowInviteBox();

        public delegate void SdkInitDelegate(string lang, string platform);
        public event SdkInitDelegate OnSdkInit;
        
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

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
        
        private void VK2Unity_OnSdkInit(string payload)
        {
            var template = new
            {
                lang = "",
                platform = ""
            };
            var parsedPayload = JsonConvert.DeserializeAnonymousType(payload, template);
            OnSdkInit?.Invoke(
                parsedPayload.lang,
                parsedPayload.platform
            );
        }
        
        private void VK2Unity_OnViewHide()
        {
            OnViewHide?.Invoke();
        }
        
        private void VK2Unity_OnViewRestore()
        {
            OnViewRestore?.Invoke();
        }

        private void VK2Unity_OnNativeAdsOpenRequest(string payload)
        {
            var template = new
            {
                adFormat = ""
            };
            var parsedPayload = JsonConvert.DeserializeAnonymousType(payload, template);
            OnNativeAdsOpenRequest?.Invoke(parsedPayload.adFormat);
        }

        private void VK2Unity_OnNativeAdsOpenResult(string payload)
        {
            var template = new
            {
                adFormat = "",
                result = false,
            };
            var parsedPayload = JsonConvert.DeserializeAnonymousType(payload, template);
            OnNativeAdsOpenResult?.Invoke(parsedPayload.adFormat, parsedPayload.result);
        }
        
        private void VK2Unity_OnPlayerDataReceive(string payload)
        {
            var parsedPayload = JsonConvert.DeserializeObject<Dictionary<string, object>>(payload);
            OnPlayerDataReceive?.Invoke(parsedPayload);
        }
    }
}