using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NubikTowerBuilding.Behaviours;
using NubikTowerBuilding.Effects;
using NubikTowerBuilding.Models;
using NubikTowerBuilding.Spawners;
using UnityEngine;
using Zenject;

namespace NubikTowerBuilding.Managers
{
    public class BuildEffectManager : MonoBehaviour
    {
        [Inject] private BuildPuffEffectSpawner _buildPuffEffectSpawner;
        [Inject] private PerfectBuildSplashEffectSpawner _perfectSplashEffectSpawner;
        [Inject] private PerfectBuildIdleEffectSpawner _perfectIdleEffectSpawner;
        [Inject] private SettleInEffectSpawner _settleInEffectSpawner;
        [Inject] private GameCamera _gameCamera;
        [Inject] private Tower _tower;
        [Inject] private ScoreManager _scoreManager;
        [SerializeField] private ParticleSystem cloudEffectsEffect;

        private int _settleInEffectCounter;
        private readonly Dictionary<BuildingBlock, PerfectBuildIdleEffect> _activeIdleEffects = new();
        
        public void CleanUp()
        {
            _settleInEffectCounter = 0;
            ClearIdleEffects();
            cloudEffectsEffect.Stop();
        }

        private void Start()
        {
            _scoreManager.OnPerfectBuildActiveChange += OnPerfectBuildActiveChange;
            _tower.OnAttachBuildingBlock += OnAttachBuildingBlockResult;
        }

        private void OnPerfectBuildActiveChange(bool isActive)
        {
            if (isActive)
            {
                var buildingBlocks = _tower.GetBlocks();
                for (int i = buildingBlocks.Count - 1; i >= buildingBlocks.Count - 4 && i >= 0; i--)
                {
                    CreatePerfectIdleEffect(buildingBlocks[i]);
                }
                CreatePerfectSplashEffect(buildingBlocks[^1]);
            }
            else
            {
                ClearIdleEffects();
            }
        }

        private void OnAttachBuildingBlockResult(AttachBuildingBlockResult result)
        {
            if (_tower.GetHeight() == 15)
            {
                cloudEffectsEffect.Play();
            }
            
            if (result.IsSuccess)
            {
                CreateBuildPuffEffect(result.ConnectedBlock);
                
                if (_scoreManager.IsPerfectBuildActive())
                {
                    CreatePerfectIdleEffect(result.ConnectedBlock);
                }

                CreateSettleInEffect(result.ConnectedBlock, 
                    result.IsPerfect ? GameManager.OccupantsInPerfectBlock : GameManager.OccupantsInNormalBlock
                );
            }
        }

        private void ClearIdleEffects()
        {
            foreach (var buildingBlock in _activeIdleEffects.Keys)
            {
                _perfectIdleEffectSpawner.Despawn(_activeIdleEffects[buildingBlock]);
            }
            _activeIdleEffects.Clear();
        }

        private void CreateBuildPuffEffect(BuildingBlock buildingBlock)
        {
            var instance = _buildPuffEffectSpawner.Spawn(buildingBlock.transform.position, 1f);
            var instanceTrans = instance.transform;
            instanceTrans.SetParent(buildingBlock.transform);
            instanceTrans.localPosition = Vector3.down * (buildingBlock.GetDimensions().y * 0.35f);
            instanceTrans.localRotation = Quaternion.identity;
            instanceTrans.localScale = Vector3.one;
        }
        
        private void CreatePerfectSplashEffect(BuildingBlock buildingBlock)
        {
            var instance = _perfectSplashEffectSpawner.Spawn(buildingBlock.transform.position, 1f);
            var instanceTrans = instance.transform;
            instanceTrans.SetParent(buildingBlock.transform);
            instanceTrans.localPosition = Vector3.zero;
            instanceTrans.localRotation = Quaternion.identity;
            instanceTrans.localScale = Vector3.one;
        }

        private void CreatePerfectIdleEffect(BuildingBlock buildingBlock)
        {
            if (_activeIdleEffects.ContainsKey(buildingBlock))
            {
                return;
            }
            var instance = _perfectIdleEffectSpawner.Spawn(buildingBlock.transform.position, 1f);
            var instanceTrans = instance.transform;
            instanceTrans.SetParent(buildingBlock.transform);
            instanceTrans.localPosition = Vector3.zero;
            instanceTrans.localRotation = Quaternion.identity;
            instanceTrans.localScale = Vector3.one;
            _activeIdleEffects.Add(buildingBlock, instance);
        }
        
        private async void CreateSettleInEffect(BuildingBlock buildingBlock, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var flyFromRandomX = Random.Range(40f, 50f);
                var flyFromRandomZ = Random.Range(-30f, 30f);
                var flyFromPosition = new Vector3(flyFromRandomX, _gameCamera.transform.position.y + 45f, flyFromRandomZ);
                if (_settleInEffectCounter % 2 == 0)
                {
                    flyFromPosition.x = -flyFromPosition.x;
                }
                _settleInEffectSpawner.Spawn(
                    flyFromPosition,
                    buildingBlock.transform,
                    4f
                );
                _settleInEffectCounter++;
                await UniTask.Delay(System.TimeSpan.FromSeconds(0.2f));
            }
        }
    }
}