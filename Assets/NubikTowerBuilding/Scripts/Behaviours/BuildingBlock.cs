using Cysharp.Threading.Tasks;
using NubikTowerBuilding.Models;
using UnityEngine;

namespace NubikTowerBuilding.Behaviours
{
    public class BuildingBlock : MonoBehaviour
    {
        [SerializeField] private Rigidbody body;
        [SerializeField] private Vector3 dimensions;
        
        private BuildingBlock _attachedBuildingBlock;
        private FixedJoint _fixedJoint;
        private bool _isDropOut;
        private bool _isCollided;

        public delegate void CollideDelegate(BuildingBlock block, Collision other);
        public event CollideDelegate OnCollide;

        private void OnCollisionEnter(Collision other)
        {
            if (_isDropOut ||
                _isCollided)
            {
                return;
            }

            _isCollided = true;
            OnCollide?.Invoke(this, other);
        }

        public Vector3 GetDimensions()
        {
            return dimensions;
        }
        
        public Rigidbody GetBody()
        {
            return body;
        }
        
        public AttachBuildingBlockResult AttachBuildingBlock(BuildingBlock buildingBlock, float maxAttachError)
        {
            if (_attachedBuildingBlock != null)
            {
                Debug.LogWarning("BuildPlatform already have attached building block");
                return new AttachBuildingBlockResult
                {
                    AnchorBlock = this,
                    ConnectedBlock = buildingBlock,
                    IsRootBlock = false,
                    IsSuccess = false,
                    IsPerfect = false
                };
            }
            
            _attachedBuildingBlock = buildingBlock;

            _attachedBuildingBlock.transform.SetParent(transform);

            var buildingBlockTransform = buildingBlock.transform;
            var blockWidth = buildingBlock.GetDimensions().x;
            var blockHeight = buildingBlock.GetDimensions().y;
            var buildingBlockBody = _attachedBuildingBlock.GetBody();
            var buildingBlockLocPos = _attachedBuildingBlock.transform.localPosition;
            
            if (Mathf.Abs(buildingBlockLocPos.x) > maxAttachError)
            {
                buildingBlockBody.AddForce(Vector3.right * buildingBlockLocPos.x * 1f, ForceMode.Impulse);
                
                _attachedBuildingBlock.transform.SetParent(null);
                _attachedBuildingBlock = null;
                
                return new AttachBuildingBlockResult
                {
                    AnchorBlock = this,
                    ConnectedBlock = buildingBlock,
                    IsRootBlock = false,
                    IsSuccess = false,
                    IsPerfect = false
                };
            }

            bool isPerfect = Mathf.Abs(buildingBlockLocPos.x) < blockWidth * 0.05f;
            
            Destroy(buildingBlock.GetComponent<ConstantForce>());
                
            buildingBlockBody.useGravity = false;
            buildingBlockBody.velocity = Vector3.zero;
            buildingBlockBody.angularVelocity = Vector3.zero;
            buildingBlockBody.isKinematic = true;

            if (isPerfect)
            {
                buildingBlockLocPos.x = 0f;
            }
            buildingBlockLocPos.y = blockHeight;
            buildingBlockLocPos.z = 0f;

            buildingBlockTransform.localPosition = buildingBlockLocPos;
            buildingBlockTransform.localRotation = Quaternion.Euler(0f, 0f, -buildingBlockTransform.localPosition.x * 15f);

            UniTask.Create(async () =>
            {
                await CorrectAttachedBlockRotation(buildingBlock, 0.5f);
            });
                
            return new AttachBuildingBlockResult
            {
                AnchorBlock = this,
                ConnectedBlock = buildingBlock,
                IsRootBlock = false,
                IsSuccess = true,
                IsPerfect = isPerfect
            };
        }

        private async UniTask CorrectAttachedBlockRotation(BuildingBlock buildingBlock, float duration)
        {
            var blockTrans = buildingBlock.transform;
            var localRot = blockTrans.localRotation;
            var localPos = blockTrans.localPosition;
            var localPivot = localPos + Vector3.down * (blockTrans.localScale.y / 2f);

            var time = 0f;
            while (time <= duration)
            {
                await UniTask.WaitForEndOfFrame(this);
                var newLocalRot = Quaternion.Lerp(localRot, Quaternion.identity, Mathf.Clamp01(time / duration));
                var newLocalPos = newLocalRot * (localPos - localPivot) + localPivot;
                blockTrans.localPosition = newLocalPos;
                blockTrans.localRotation = newLocalRot;
                time += Time.deltaTime;
            }
            
            blockTrans.localRotation = Quaternion.identity;
        }
    }
}
