using NubikTowerBuilding.Behaviours;
using NubikTowerBuilding.Models;
using NubikTowerBuilding.Spawners;
using UnityEngine;
using Zenject;

namespace NubikTowerBuilding.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Inject] private BuildingBlockSpawner _buildingBlockSpawner;
        [SerializeField] private GameCamera gameCamera;
        [SerializeField] private Crane crane;
        [SerializeField] private Tower tower;

        private BuildingBlock _currBuildingBlock;
        private bool _waitForDropAnimation;

        private void Start()
        {
            OnReadyToNewBlock();
        }

        private void Update()
        {
            UpdateHeights();
            
            if (!_waitForDropAnimation && _currBuildingBlock != null && Input.GetMouseButtonUp(0))
            {
                DropBlock();
            }
        }

        private void UpdateHeights()
        {
            UpdateCraneHeight();
            UpdateCameraHeight();
        }

        private void UpdateCraneHeight()
        {
            var craneTrans = crane.transform;
            var cranePos = craneTrans.position;
            cranePos.y = 70f + 6f / 2f * tower.GetHeight(); // 70f is start height, 6 is block height
            craneTrans.position = cranePos;
        }
        
        private void UpdateCameraHeight()
        {
            var cameraTrans = gameCamera.transform;
            var cameraPos = cameraTrans.position;
            cameraPos.y = 40f + 6f / 2f * tower.GetHeight(); // 40f is start height, 6 is block height
            cameraTrans.position = cameraPos;
        }

        private void CreateNextBlock()
        {
            if (_currBuildingBlock != null)
            {
                Debug.LogWarning("Current building block is not null. Can't create next block.");
                return;
            }
            
            _currBuildingBlock = _buildingBlockSpawner.Spawn(BuildingBlockType.Default);
            _currBuildingBlock.ConnectToCrane(crane);
        }

        private void DropBlock()
        {
            if (_currBuildingBlock == null)
            {
                Debug.LogWarning("Current building block is null. Nothing to drop.");
                return;
            }

            _currBuildingBlock.OnCollide += OnBuildingBlockCollide;
            _currBuildingBlock.OnBroke += OnBuildingBlockBroke;
            _currBuildingBlock.Drop();
            _currBuildingBlock = null;

            _waitForDropAnimation = true;
        }

        private void OnReadyToNewBlock()
        {
            _waitForDropAnimation = false;
            CreateNextBlock();
        }
        
        private void OnBuildingBlockCollide(BuildingBlock block, Collision other)
        {
            block.OnCollide -= OnBuildingBlockCollide;
            
            if (other.gameObject.layer == LayerMask.NameToLayer("BuildPlatform"))
            {
                if (tower.IsEmpty())
                {
                    tower.AddBuildingBlock(block);
                }
                else
                {
                    throw new System.Exception("Game over");
                }
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("BuildingBlock"))
            {
                tower.AddBuildingBlock(block);
            }
            else
            {
                throw new System.Exception("Game over");
            }

            OnReadyToNewBlock();
        }

        private void OnBuildingBlockBroke(BuildingBlock block)
        {
            throw new System.NotImplementedException();
        }
    }
}
