using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using KlopoffGames.Core.Audio;
using NubikTowerBuilding.Behaviours;
using NubikTowerBuilding.Models;
using NubikTowerBuilding.Spawners;
using UnityEngine;
using Zenject;

namespace NubikTowerBuilding.Managers
{
    public class BuildManager : MonoBehaviour
    {
        [Inject] private AudioManager _audio;
        [Inject] private BuildingBlockSpawner _buildingBlockSpawner;
        [Inject] private Crane _crane;
        [Inject] private Tower _tower;

        [SerializeField] private Vector2 startCraneSwingAmplitude;
        [SerializeField] private Vector2 startCraneSwingFrequency;
        
        [SerializeField] private Vector2 gameCraneSwingAmplitude;
        [SerializeField] private Vector2 gameCraneSwingFrequency;

        private BuildingBlockType _buildingBlockType;
        private bool _canBuild;
        private BuildingBlock _currBuildingBlock;
        private bool _waitForDropAnimation;
        private Dictionary<BuildingBlock, CancellationTokenSource> _observableBlocks;

        public Tower Tower => _tower;
        
        public delegate void SuccessBuildDelegate(BuildingBlock buildingBlock, bool perfect);
        public event SuccessBuildDelegate OnSuccessBuild;

        public delegate void FailBuildDelegate(BuildingBlock buildingBlock);
        public event FailBuildDelegate OnFailBuild;

        public void CleanUp(bool cleanCurrBlock)
        {
            _canBuild = false;
            _tower.CleanTower();
            if (cleanCurrBlock)
            {
                _currBuildingBlock = null;
                if (_currBuildingBlock != null)
                {
                    _crane.DetachBuildingBlock();
                }
                _buildingBlockSpawner.Despawn(_currBuildingBlock);
            }
            _waitForDropAnimation = false;
            _observableBlocks = new Dictionary<BuildingBlock, CancellationTokenSource>();
        }
        
        public void SetCanBuild(bool canBuild)
        {
            _canBuild = canBuild;
        }

        public BuildingBlockType GetBuildingBlockType()
        {
            return _buildingBlockType;
        }

        public void SetBuildingBlockType(BuildingBlockType buildingBlockType)
        {
            if (_buildingBlockType != buildingBlockType)
            {
                _crane.DetachBuildingBlock();
                _buildingBlockSpawner.Despawn(_currBuildingBlock);
                _currBuildingBlock = null;
                
                _buildingBlockType = buildingBlockType;
                CreateNextBlock();
            }
        }
        
        private void Start()
        {
            InitTowerCallbacks();
            OnReadyToNewBlock();
        }

        private void InitTowerCallbacks()
        {
            _tower.OnAttachBuildingBlock += OnBuildingBlockAttach;
        }

        private void Update()
        {
            UpdateCraneAmplitudeAndFrequency();
            
            if (!_canBuild)
            {
                return;
            }
            
            if (!_waitForDropAnimation && _currBuildingBlock != null && Input.GetMouseButtonUp(0))
            {
                DropBlock();
            }
        }

        private void UpdateCraneAmplitudeAndFrequency()
        {
            var towerHeight = _tower.GetHeight();
            if (towerHeight == 0)
            {
                _crane.SetSwingAmplitude(startCraneSwingAmplitude);
                _crane.SetSwingFrequency(startCraneSwingFrequency);
            }
            else
            {
                _crane.SetSwingAmplitude(gameCraneSwingAmplitude);
                _crane.SetSwingFrequency(gameCraneSwingFrequency);
            }
        }

        private void CreateNextBlock()
        {
            if (_currBuildingBlock != null)
            {
                Debug.LogWarning("Current building block is not null. Can't create next block.");
                return;
            }
            
            _currBuildingBlock = _buildingBlockSpawner.Spawn(_tower.GetHeight() == 0, _buildingBlockType);
            _crane.AttachBuildingBlock(_currBuildingBlock);
        }

        private void DropBlock()
        {
            if (_currBuildingBlock == null)
            {
                Debug.LogWarning("Current building block is null. Nothing to drop.");
                return;
            }

            _observableBlocks.Add(_currBuildingBlock, new CancellationTokenSource());
            UniTask.Create(() => ObserveFallingBlock(_currBuildingBlock, _observableBlocks[_currBuildingBlock]));
            
            _currBuildingBlock.OnCollide += OnBuildingBlockCollide;
            _currBuildingBlock = null;
            
            _crane.DropBuildingBlock();

            _waitForDropAnimation = true;
        }

        private void OnReadyToNewBlock()
        {
            _waitForDropAnimation = false;
            CreateNextBlock();
        }

        private void OnBuildingBlockCollide(BuildingBlock block, Collision other)
        {
            _observableBlocks[block].Cancel();
            
            block.OnCollide -= OnBuildingBlockCollide;

            if (other.gameObject.layer == LayerMask.NameToLayer("BuildPlatform"))
            {
                if (_tower.IsEmpty())
                {
                    _tower.AddBuildingBlock(block);
                }
                else
                {
                    OnFailBuild?.Invoke(block);
                    UniTask.Create(() => DestroyBuildingBlockAfterFail(block));
                }
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("BuildingBlock"))
            {
                _tower.AddBuildingBlock(block);
            }
            else
            {
                OnFailBuild?.Invoke(block);
                UniTask.Create(() => DestroyBuildingBlockAfterFail(block));
            }

            OnReadyToNewBlock();
        }

        private void OnBuildingBlockAttach(AttachBuildingBlockResult result)
        {
            if (result.IsSuccess)
            {
                OnSuccessBuild?.Invoke(result.ConnectedBlock, result.IsPerfect);
                _audio.PlaySound("SuccessBuild", false, 1f, 1f);
                if (result.IsPerfect)
                {
                    _audio.PlaySound("PerfectBuild", false, 1f, 1f);
                }
            }
            else
            {
                OnFailBuild?.Invoke(result.ConnectedBlock);
                UniTask.Create(() => DestroyBuildingBlockAfterFail(result.ConnectedBlock));
                _audio.PlaySound("FailBuild", false, 1f, 1f);
            }
        }

        private async UniTask ObserveFallingBlock(BuildingBlock buildingBlock, CancellationTokenSource cancellationTokenSource)
        {
            var timeout = 1f;
            var time = 0f;
            while (time < timeout && !cancellationTokenSource.IsCancellationRequested)
            {
                await UniTask.WaitForEndOfFrame(this, cancellationTokenSource.Token);
                time += Time.deltaTime;
            }
            if (!cancellationTokenSource.IsCancellationRequested)
            {
                OnFailBuild?.Invoke(buildingBlock);
                OnReadyToNewBlock();
                
                _buildingBlockSpawner.Despawn(buildingBlock);
            }
        }

        private async UniTask DestroyBuildingBlockAfterFail(BuildingBlock failedBlock)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f));
            _buildingBlockSpawner.Despawn(failedBlock);
        }
    }
}
