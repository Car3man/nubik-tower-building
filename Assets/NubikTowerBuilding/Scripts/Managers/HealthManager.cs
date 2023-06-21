using NubikTowerBuilding.Behaviours;
using UnityEngine;
using Zenject;

namespace NubikTowerBuilding.Managers
{
    public class HealthManager : MonoBehaviour
    {
        [Inject] private BuildManager _buildManager;
        [SerializeField] private GameObject[] healthPoints;

        private int _currHealth;

        public delegate void HealthChangeDelegate(int newVal);
        public event HealthChangeDelegate OnHealthChange;

        public void CleanUp()
        {
            SetHealth(0, false);
        }

        private void Start()
        {
            _buildManager.OnFailBuild += OnFailBuild;
        }

        private void OnFailBuild(BuildingBlock buildingBlock)
        {
            SetHealth(_currHealth - 1, true);
        }

        public int GetHealth()
        {
            return _currHealth;
        }

        public void SetHealth(int health, bool notify)
        {
            _currHealth = health;

            for (var i = 0; i < healthPoints.Length; i++)
            {
                healthPoints[i].SetActive(i < health);
            }

            if (notify)
            {
                OnHealthChange?.Invoke(_currHealth);
            }
        }
    }
}