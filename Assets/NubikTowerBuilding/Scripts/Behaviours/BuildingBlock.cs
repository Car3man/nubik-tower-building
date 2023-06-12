using UnityEngine;

namespace NubikTowerBuilding.Behaviours
{
    public class BuildingBlock : MonoBehaviour
    {
        [SerializeField] private Rigidbody body;

        private BuildingBlock _attachedBuildingBlock;
        private FixedJoint _fixedJoint;

        public delegate void CollideDelegate(BuildingBlock block, Collision other);
        public event CollideDelegate OnCollide;

        public delegate void BrokeDelegate(BuildingBlock block);
        public event BrokeDelegate OnBroke;

        private void OnCollisionEnter(Collision other)
        {
            OnCollide?.Invoke(this, other);
        }

        private void OnJointBreak(float breakForce)
        {
            OnBroke?.Invoke(this);
        }

        public Rigidbody GetBody()
        {
            return body;
        }
        
        public bool AttachBuildingBlock(BuildingBlock buildingBlock)
        {
            if (_attachedBuildingBlock != null)
            {
                Debug.LogWarning("BuildPlatform already have attached building block");
                return false;
            }
            
            _attachedBuildingBlock = buildingBlock;

            _attachedBuildingBlock.transform.SetParent(transform);
            
            var blockWidth = buildingBlock.transform.localScale.x;
            var blockHeight = buildingBlock.transform.localScale.y;
            var buildingBlockBody = _attachedBuildingBlock.GetBody();
            var buildingBlockLocPos = _attachedBuildingBlock.transform.localPosition;

            if (Mathf.Abs(buildingBlockLocPos.x) > blockWidth / 2f)
            {
                _attachedBuildingBlock.transform.SetParent(null);
                _attachedBuildingBlock = null;
                
                return false;
            }
            else
            {
                Destroy(buildingBlock.GetComponent<ConstantForce>());
                
                buildingBlockBody.useGravity = false;
                buildingBlockBody.isKinematic = true;
                buildingBlockBody.velocity = Vector3.zero;
                buildingBlockBody.angularVelocity = Vector3.zero;

                buildingBlockLocPos.y = blockHeight;
                buildingBlockLocPos.z = 0f;

                var correctedPosition = transform.TransformPoint(buildingBlockLocPos);
                buildingBlock.transform.localPosition = buildingBlockLocPos;
                buildingBlock.transform.localRotation = Quaternion.identity;
                
                return true;
            }
        }
    }
}
