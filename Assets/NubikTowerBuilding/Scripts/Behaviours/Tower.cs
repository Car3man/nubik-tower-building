using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NubikTowerBuilding.Behaviours
{
    public class Tower : MonoBehaviour
    {
        [FormerlySerializedAs("platform")] [SerializeField] private BuildPlatform buildPlatform;

        private readonly List<BuildingBlock> _buildingBlocks = new();
        private float _swingAmplitude;
        private float _swingFrequency;

        private void FixedUpdate()
        {
            if (!IsEmpty())
            {
                var rootBlock = GetRootBlock();
                var swing = Mathf.Sin(Time.time * _swingFrequency) * _swingAmplitude;
                var rootBlockRotZ = Quaternion.Euler(0f, 0f, swing);
                rootBlock.GetBody().MoveRotation(rootBlockRotZ);
            }
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

        public BuildingBlock GetTopBlock()
        {
            if (_buildingBlocks.Count == 0)
            {
                return null;
            }
            return _buildingBlocks[^1];
        }

        public void SetSwing(float amplitude, float frequency)
        {
            _swingAmplitude = amplitude;
            _swingFrequency = frequency;
        }
        
        public void AddBuildingBlock(BuildingBlock buildingBlock)
        {
            if (IsEmpty())
            {
                buildPlatform.AttachBuildingBlock(buildingBlock);
                _buildingBlocks.Add(buildingBlock);
            }
            else
            {
                if (_buildingBlocks[^1].AttachBuildingBlock(buildingBlock))
                {
                    _buildingBlocks.Add(buildingBlock);
                }
                else
                {
                    // bad block
                }
            }
        }
    }
}
