using KlopoffGames.Core.Audio;
using UnityEngine;
using Zenject;

namespace NubikTowerBuilding.Managers
{
    public class GameOverManager : MonoBehaviour
    {
        [Inject] private AudioManager _audio;
        [Inject] private HealthManager _healthManager;

        public bool IsGameOver { get; private set; }

        public delegate void GameOverDelegate();
        public event GameOverDelegate OnGameOver;
        
        public void CleanUp()
        {
            IsGameOver = false;
        }

        private void Start()
        {
            _healthManager.OnHealthChange += OnHealthChange;
        }

        private void OnHealthChange(int newVal)
        {
            if (IsGameOver)
            {
                return;
            }

            IsGameOver = newVal <= 0;

            if (IsGameOver)
            {
                _audio.PlaySound("Lose", false, 0.5f, 1f);
                OnGameOver?.Invoke();
            }
        }
    }
}