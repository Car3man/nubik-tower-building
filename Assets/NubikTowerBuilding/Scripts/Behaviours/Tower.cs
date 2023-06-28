using System.Collections.Generic;
using NubikTowerBuilding.Models;
using NubikTowerBuilding.Spawners;
using UnityEngine;
using Zenject;

namespace NubikTowerBuilding.Behaviours
{
    public class Tower : MonoBehaviour
    {
        [Inject] private BuildingBlockSpawner _buildingBlockSpawner;
        [Inject] private BuildPlatform _buildPlatform;

        private readonly List<BuildingBlock> _buildingBlocks = new();
        private float _swingAmplitude;
        private float _swingFrequency;
        private float _swingCorrectionAngle;

        public delegate void AttachBuildingBlockDelegate(AttachBuildingBlockResult result);
        public event AttachBuildingBlockDelegate OnAttachBuildingBlock;

        private void Update()
        {
            CheckBlocksBodies();
            UpdateSwing();
        }
        
        private void CheckBlocksBodies()
        {
            for (int i = 0; i < GetHeight() - 3; i++)
            {
                if (_buildingBlocks[i].GetBody() != null)
                {
                    Destroy(_buildingBlocks[i].GetBody());
                    Destroy(_buildingBlocks[i].GetComponent<Collider>());
                }
            }
        }

        private void UpdateSwing() 
        {
            if (!IsEmpty())
            {
                var rootBlock = GetRootBlock();
                var swingAngle = GetSwingValue(Time.time * _swingFrequency);
                var rootBlockRot = Quaternion.Euler(0f, 0f, swingAngle + _swingCorrectionAngle);
                rootBlock.transform.rotation = rootBlockRot;
            }
        }

        public float GetSwingValue(float t)
        {
            return Mathf.Sin(t) * _swingAmplitude;
        }

        public bool IsEmpty()
        {
            return GetHeight() == 0;
        }

        public int GetHeight()
        {
            return _buildingBlocks.Count;
        }
        
        public BuildingBlock GetRootBlock()
        {
            if (_buildingBlocks.Count == 0)
            {
                return null;
            }
            return _buildingBlocks[0];
        }
        
        public List<BuildingBlock> GetBlocks()
        {
            return _buildingBlocks;
        }

        public void SetSwing(float amplitude, float frequency)
        {
            _swingAmplitude = amplitude;
            _swingFrequency = frequency;
        }

        public void SetSwingCorrectionAngle(float angle)
        {
            _swingCorrectionAngle = angle;
        }

        public void CleanTower()
        {
            foreach (var buildingBlock in _buildingBlocks)
            {
                _buildingBlockSpawner.Despawn(buildingBlock);
            }
            _buildingBlocks.Clear();
        }
        
        public void AddBuildingBlock(BuildingBlock buildingBlock)
        {
            AttachBuildingBlockResult result;
            
            if (IsEmpty())
            {
                result = _buildPlatform.AttachBuildingBlock(buildingBlock);
            }
            else
            {
                var blockWidth = buildingBlock.GetDimensions().x;
                var maxAttachError = Mathf.Clamp(blockWidth * 0.7f - GetHeight() * 0.015f, blockWidth * 0.4f, blockWidth * 0.7f);
                result = _buildingBlocks[^1].AttachBuildingBlock(buildingBlock, maxAttachError);
            }
            
            if (result.IsSuccess)
            {
                _buildingBlocks.Add(buildingBlock);
            }
            
            OnAttachBuildingBlock?.Invoke(result);
        }
    }
}
