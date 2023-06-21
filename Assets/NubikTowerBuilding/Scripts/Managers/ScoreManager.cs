using NubikTowerBuilding.Behaviours;
using NubikTowerBuilding.Ui.Elements;
using TMPro;
using UnityEngine;
using Zenject;

namespace NubikTowerBuilding.Managers
{
    public class ScoreManager : MonoBehaviour
    {
        [Inject] private BuildManager _buildManager;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private ProgressBar perfectBuildProgressBar;
        
        private int _currScore;
        private float _perfectBuildBoostTime;
        
        private const int SuccessBuildScores = 10;
        private const float PerfectBuildScoresMultiplier = 2f;
        private const float PerfectBuildBoostDuration = 6f;

        public delegate void PerfectBuildActiveChangeDelegate(bool isActive);
        public event PerfectBuildActiveChangeDelegate OnPerfectBuildActiveChange;

        public void CleanUp()
        {
            SetScore(0);
            _perfectBuildBoostTime = 0f;
        }
        
        private void Start()
        {
            _buildManager.OnSuccessBuild += OnSuccessBuild;
            
            SetScore(0);
        }

        private void Update()
        {
            perfectBuildProgressBar.gameObject.SetActive(IsPerfectBuildActive());
            
            if (IsPerfectBuildActive())
            {
                perfectBuildProgressBar.SetValue(Mathf.Clamp01(_perfectBuildBoostTime / PerfectBuildBoostDuration));
            }

            var prevPerfectBuildBoostTime = _perfectBuildBoostTime;
            _perfectBuildBoostTime -= Time.deltaTime;
            if (_perfectBuildBoostTime <= 0f && prevPerfectBuildBoostTime > 0f)
            {
                OnPerfectBuildActiveChange?.Invoke(false);
            }
        }

        private void OnSuccessBuild(BuildingBlock buildingBlock, bool perfect)
        {
            if (perfect)
            {
                _perfectBuildBoostTime = PerfectBuildBoostDuration;
                OnPerfectBuildActiveChange?.Invoke(true);
            }

            int scoreToAdd = SuccessBuildScores;
            if (_perfectBuildBoostTime > 0)
            {
                scoreToAdd = Mathf.FloorToInt(scoreToAdd * PerfectBuildScoresMultiplier);
            }
            
            AddScore(scoreToAdd);
        }

        public bool IsPerfectBuildActive()
        {
            return _perfectBuildBoostTime > 0f;
        }

        public int GetScore()
        {
            return _currScore;
        }

        private void AddScore(int score)
        {
            SetScore(GetScore() + score);
        }
        
        private void SetScore(int score)
        {
            _currScore = score;

            scoreText.text = _currScore.ToString();
        }
    }
}
