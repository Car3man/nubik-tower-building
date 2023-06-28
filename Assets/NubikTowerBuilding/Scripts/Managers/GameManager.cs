using Cysharp.Threading.Tasks;
using KlopoffGames.Core.Audio;
using KlopoffGames.Core.Windows;
#if VK_GAMES && !UNITY_EDITOR
using KlopoffGames.WebPlatforms.VK;
#endif
#if YANDEX_GAMES && !UNITY_EDITOR
using KlopoffGames.WebPlatforms.Yandex;
#endif
using NubikTowerBuilding.Behaviours;
using NubikTowerBuilding.Services;
using NubikTowerBuilding.Ui.Windows;
using UnityEngine;
using Zenject;

namespace NubikTowerBuilding.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Inject] private AudioManager _audio;
        [Inject] private WindowManager _windows;
        [Inject] private BuildManager _buildManager;
        [Inject] private HealthManager _healthManager;
        [Inject] private HeightManager _heightManager;
        [Inject] private ScoreManager _scoreManager;
        [Inject] private SwingManager _swingManager;
        [Inject] private BuildEffectManager _buildEffectManager;
        [Inject] private GameOverManager _gameOverManager;
        [Inject] private SavingService _savingService;
        [Inject] private UserCoinsService _userCoinsService;
#if YANDEX_GAMES && !UNITY_EDITOR
        [Inject] private YandexManager _yandex;
#endif
#if VK_GAMES && !UNITY_EDITOR
        [Inject] private VKManager _vk;
#endif
        [SerializeField] private GameObject inGameCanvas;

        private bool _isGameStarted;

        public int ReachedHeight => _buildManager.Tower.GetHeight();
        public int ReachedPopulation { get; private set; }
        public int EarnedCoins => Mathf.FloorToInt(_scoreManager.GetScore() / 50f);
        
        public const int OccupantsInNormalBlock = 1;
        public const int OccupantsInPerfectBlock = 2;
        
        public void StartGame()
        {
            OnGameStart();
            _savingService.SetGameRunCounter(_savingService.GetGameRunCounter() + 1);
        }

        public void PlayAgain()
        {
            OnGameStart();
        }

        public void BackToLobby()
        {
            OnGameReady(true);
        }

        private void Start()
        {
            _buildManager.OnSuccessBuild += BuildManagerOnSuccessBuild;
            _gameOverManager.OnGameOver += OnGameOver;

            _audio.PlayMusicIfNotSame("MainTheme", 0.1f);
            _buildManager.SetBuildingBlockType(_savingService.GetLastBlockPlayed());
            
            OnGameReady(false);
        }

        private void CleanUpManagers()
        {
            ReachedPopulation = 0;
            
            _buildManager.CleanUp(false);
            _healthManager.CleanUp();
            _heightManager.CleanUp();
            _scoreManager.CleanUp();
            _swingManager.CleanUp();
            _buildEffectManager.CleanUp();
            _gameOverManager.CleanUp();
        }

        private void BuildManagerOnSuccessBuild(BuildingBlock buildingBlock, bool perfect)
        {
            ReachedPopulation += perfect ? OccupantsInPerfectBlock : OccupantsInNormalBlock;
        }

        private void OnGameOver()
        {
            if (!_isGameStarted)
            {
                return;
            }
            
            OnGameEnd();
        }

        private void OnGameReady(bool fromGameRun)
        {
            CleanUpManagers();
            inGameCanvas.SetActive(false);

            if (fromGameRun)
            {
                _windows.CreateWindow<LobbyGameWindow>("Windows/Game");
                
                CheckAppShare();
            }
            else
            {
                var isFirstTime = _savingService.GetIsFTUE();
                if (isFirstTime)
                {
                    StartGame();
                    _savingService.SetIsFTUE(false);
                }
                else
                {
                    _windows.CreateWindow<LobbyGameWindow>("Windows/Game");
                }
            }
        }

        private async void OnGameStart()
        {
            CleanUpManagers();
            await UniTask.WaitForEndOfFrame(this);
            
            inGameCanvas.SetActive(true);
            _healthManager.SetHealth(3, false);
            _buildManager.SetCanBuild(true);
            
            _isGameStarted = true;
            
            _savingService.SetLastBlockPlayed(_buildManager.GetBuildingBlockType());
        }

#pragma warning disable CS1998
        private async void OnGameEnd()
#pragma warning restore CS1998
        {
            var score = _scoreManager.GetScore();
            var bestScore = _savingService.GetBestScore();
            if (score > bestScore)
            {
                _savingService.SetBestScore(score);
                
#if YANDEX_GAMES && !UNITY_EDITOR
                _yandex.SetLeaderboardScore("defaultLeaderboard", score);
#endif

#if VK_GAMES && !UNITY_EDITOR
                await UniTask.Delay(System.TimeSpan.FromSeconds(2.5f));
                _vk.UpdateAndShowLeaderboardBox(score);
#endif
            }

            _userCoinsService.AddCoins(EarnedCoins);
            
            _windows.CreateWindow<GameOverWindow>("Windows/Game");
            _buildManager.SetCanBuild(false);
        }

        private void CheckAppShare()
        {
            if (_savingService.GetIsAppShareDisabled())
            {
                // ReSharper disable once RedundantJumpStatement
                return;
            }

#if VK_GAMES && !UNITY_EDITOR
            if (_savingService.GetGameRunCounter() == 2 ||
                (_savingService.GetGameRunCounter() + 2) % 5 == 0)
            {
                _windows.CreateWindow<AppShareWindow>("Windows/Game");
            }
#endif
        }
    }
}
