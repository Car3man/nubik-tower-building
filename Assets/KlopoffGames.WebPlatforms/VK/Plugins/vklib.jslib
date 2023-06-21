mergeInto(LibraryManager.library, {

    Unity2VK_ShowNativeAds: function (adFormat) {
        unity2vk_showNativeAds(UTF8ToString(adFormat));
    },
  
    Unity2VK_SetPlayerData: function (key, value) {
        unity2vk_setPlayerData(UTF8ToString(key), UTF8ToString(value));
    },
    
    Unity2VK_GetPlayerData: function (keys) {
        unity2vk_getPlayerData(UTF8ToString(keys));
    },
    
    Unity2VK_ShowLeaderboardBox: function () {
        unity2vk_showLeaderboardBox();
    },
    
    Unity2VK_UpdateAndShowLeaderboardBox: function (userResult) {
        unity2vk_updateAndShowLeaderboardBox(userResult);
    },
    
    Unity2VK_AddToFavorites: function () {
        unity2vk_addToFavorites();
    },
    
    Unity2VK_ShowInviteBox: function () {
        unity2vk_showInviteBox();
    },
    
});