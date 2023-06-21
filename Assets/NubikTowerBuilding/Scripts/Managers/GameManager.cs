using Cysharp.Threading.Tasks;
using KlopoffGames.Core.Audio;
using KlopoffGames.Core.Windows;
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
        }

        public void PlayAgain()
        {
            OnGameStart();
        }

        public void BackToLobby()
        {
            OnGameReady();
        }

        private void Start()
        {
            _buildManager.OnSuccessBuild += BuildManagerOnSuccessBuild;
            _gameOverManager.OnGameOver += OnGameOver;

            _audio.PlayMusicIfNotSame("MainTheme", 0.1f);
            _buildManager.SetBuildingBlockType(_savingService.GetLastBlockPlayed());
            
            OnGameReady();
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

        private void OnGameReady()
        {
            CleanUpManagers();
            inGameCanvas.SetActive(false);
            _windows.CreateWindow<LobbyGameWindow>("Windows/Game");
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

        private void OnGameEnd()
        {
            _userCoinsService.AddCoins(EarnedCoins);
            _windows.CreateWindow<GameOverWindow>("Windows/Game");
            _buildManager.SetCanBuild(false);
        }
    }
}
