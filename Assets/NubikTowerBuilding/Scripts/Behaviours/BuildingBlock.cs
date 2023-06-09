using UnityEngine;

namespace NubikTowerBuilding.Behaviours
{
    public class BuildingBlock : MonoBehaviour
    {
        [SerializeField] private Rigidbody body;
        [SerializeField] private ConstantForce gravityAccelerator;
        
        private Crane _connectedCrane;
        private Rigidbody _connectedPlatform;
        private BuildingBlock _connectedBlock;
        private HingeJoint _hingeJoint;
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

        public void ResetBuildingBlock()
        {
            body.useGravity = false;
            body.constraints = RigidbodyConstraints.FreezeRotationX |
                               RigidbodyConstraints.FreezeRotationY |
                               RigidbodyConstraints.FreezeRotationZ;
            
            _connectedCrane = null;

            if (_hingeJoint != null)
            {
                Destroy(_hingeJoint);
            }

            gravityAccelerator.enabled = false;
        }

        public void ConnectToCrane(Crane crane)
        {
            if (_connectedCrane != null)
            {
                Debug.LogWarning("BuildingBlock already connected to another crane.");
                return;
            }
            
            _connectedCrane = crane;
            
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;

            _hingeJoint = gameObject.AddComponent<HingeJoint>();
            _hingeJoint.autoConfigureConnectedAnchor = false;
            _hingeJoint.connectedBody = crane.GetHook();
            _hingeJoint.anchor = Vector3.up * 0.5f;
            _hingeJoint.connectedAnchor = Vector3.down * 0.5f;
            _hingeJoint.axis = Vector3.forward;

            gravityAccelerator.enabled = false;
        }

        public void ConnectToPlatform(Rigidbody platform)
        {
            if (_connectedPlatform != null)
            {
                Debug.LogWarning("BuildingBlock already connected to another platform.");
                return;
            }
            
            _connectedPlatform = platform;
            
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.useGravity = false;
            body.isKinematic = true;

            gravityAccelerator.enabled = false;
        }

        public void ConnectToBlock(BuildingBlock buildingBlock)
        {
            if (_connectedBlock != null)
            {
                Debug.LogWarning("BuildingBlock already connected to another block.");
                return;
            }
            
            _connectedBlock = buildingBlock;
            
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.MovePosition(new Vector3(
                body.position.x,
                buildingBlock.GetBody().position.y + buildingBlock.transform.localScale.y / 2f,
                body.position.z)
            );
            body.isKinematic = true;
            
            _fixedJoint = gameObject.AddComponent<FixedJoint>();
            _fixedJoint.connectedBody = buildingBlock.GetBody();

            gravityAccelerator.enabled = false;
        }

        public void Drop()
        {
            if (_connectedCrane == null)
            {
                Debug.LogWarning("BuildingBlock not connect to any crane.");
                return;
            }

            _connectedCrane = null;
            
            Destroy(_hingeJoint);
            
            gravityAccelerator.enabled = true;
            
            body.constraints = RigidbodyConstraints.None;
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
    }
}
