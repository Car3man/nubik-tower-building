mergeInto(LibraryManager.library, {

    Unity2Yandex_OnGameReady: function () {
        unity2yandex_onGameReady();
    },
  
    Unity2Yandex_OpenAuthDialog: function () {
        unity2yandex_openAuthDialog();
    },
    
    Unity2Yandex_ShowFullscreenAdv: function () {
        unity2yandex_showFullscreenAdv();
    },
    
    Unity2Yandex_ShowRewardedAdv: function () {
        unity2yandex_showRewardedAdv();
    },
    
    Unity2Yandex_SetPlayerData: function (data, flush) {
        unity2yandex_setPlayerData(UTF8ToString(data), flush);
    },
    
    Unity2Yandex_GetPlayerData: function () {
        unity2yandex_getPlayerData();
    },
    
    Unity2Yandex_SetLeaderboardScore: function (leaderboardName, score) {
        unity2yandex_setLeaderboardScore(UTF8ToString(leaderboardName), score);
    },
    
});