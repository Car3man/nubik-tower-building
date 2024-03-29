﻿using NubikTowerBuilding.Models;
using UnityEngine;

namespace NubikTowerBuilding.Behaviours
{
    public class BuildPlatform : MonoBehaviour
    {
        private BuildingBlock _attachedBuildingBlock;
        
        public AttachBuildingBlockResult AttachBuildingBlock(BuildingBlock buildingBlock)
        {
            if (_attachedBuildingBlock != null)
            {
                Debug.LogWarning("BuildPlatform already have attached building block");
                return new AttachBuildingBlockResult
                {
                    AnchorBlock = null,
                    ConnectedBlock = buildingBlock,
                    IsRootBlock = true,
                    IsSuccess = false,
                    IsPerfect = false
                };
            }

            _attachedBuildingBlock = buildingBlock;
            
            Destroy(buildingBlock.GetComponent<ConstantForce>());
            
            var buildPlatformTrans = transform;
            var buildPlatformPos = buildPlatformTrans.position;
            var buildPlatformHeight = buildPlatformTrans.localScale.y;
            var blockHeight = buildingBlock.GetDimensions().y;
            var buildingBlockBody = _attachedBuildingBlock.GetBody();
            buildingBlockBody.velocity = Vector3.zero;
            buildingBlockBody.angularVelocity = Vector3.zero;
            buildingBlockBody.useGravity = false;
            buildingBlockBody.isKinematic = true;
            var buildingBlockBodyPos = buildingBlockBody.position;
            buildingBlockBodyPos = new Vector3(
                buildingBlockBodyPos.x,
                buildPlatformPos.y + buildPlatformHeight / 2f + blockHeight / 2f,
                buildingBlockBodyPos.z
            );
            buildingBlock.transform.position = buildingBlockBodyPos;
            
            return new AttachBuildingBlockResult
            {
                AnchorBlock = null,
                ConnectedBlock = buildingBlock,
                IsRootBlock = true,
                IsSuccess = true,
                IsPerfect = false
            };
        }
    }
}