using UnityEngine;

namespace NubikTowerBuilding.Behaviours
{
    public class Crane : MonoBehaviour
    {
        [SerializeField] private Transform craneOrigin;
        [SerializeField] private Rigidbody craneHookBody;
        [SerializeField] private float ropeLength;
        [SerializeField] private Vector2 ropSwingAmplitude;
        [SerializeField] private Vector2 ropSwingFrequency;
        [SerializeField] private LineRenderer ropeLineRenderer;

        private readonly Vector3[] _ropeDisplayPositions = new Vector3[2];
        private BuildingBlock _attachedBuildingBlock;
        
        private void Update()
        {
            _ropeDisplayPositions[0] = craneOrigin.position;
            _ropeDisplayPositions[1] = craneHookBody.position;
            ropeLineRenderer.SetPositions(_ropeDisplayPositions);
        }

        private void FixedUpdate()
        {
            var newHookPosition = craneOrigin.position + Vector3.down * ropeLength;
            
            var horizontalSwing = Mathf.Sin(Time.time * ropSwingFrequency.x) * ropSwingAmplitude.x;
            newHookPosition += Vector3.right * horizontalSwing;

            var verticalSwing = Mathf.Cos(Time.time * ropSwingFrequency.y) * ropSwingAmplitude.y;
            newHookPosition += Vector3.up * verticalSwing;
            
            craneHookBody.MovePosition(newHookPosition);
        }

        public Rigidbody GetHook()
        {
            return craneHookBody;
        }

        public Vector3 GetHookConnectPoint(float objectHeight)
        {
            var hookHeight = craneHookBody.transform.localScale.y;
            var hookPosition = craneHookBody.position;
            hookPosition += Vector3.down * hookHeight / 2f;
            hookPosition += Vector3.down * objectHeight / 2f;
            return hookPosition;
        }

        public void AttachBuildingBlock(BuildingBlock buildingBlock)
        {
            if (_attachedBuildingBlock != null)
            {
                Debug.LogWarning("BuildPlatform already have attached building block");
                return;
            }

            var blockHeight = buildingBlock.transform.localScale.y;
            _attachedBuildingBlock = buildingBlock;
            _attachedBuildingBlock.transform.position = GetHookConnectPoint(blockHeight / 2f);
            
            var buildingBlockBody = _attachedBuildingBlock.GetBody();
            buildingBlockBody.constraints = RigidbodyConstraints.FreezeRotation;
            
            var blockHingeJoint = _attachedBuildingBlock.gameObject
                .AddComponent<HingeJoint>();
            blockHingeJoint.connectedBody = GetHook();
            blockHingeJoint.anchor = Vector3.up * 0.5f;
            blockHingeJoint.autoConfigureConnectedAnchor = false;
            blockHingeJoint.connectedAnchor = Vector3.down * 0.5f;
        }

        public void DropBuildingBlock()
        {
            var buildingBlockBody = _attachedBuildingBlock.GetBody();
            buildingBlockBody.angularVelocity = Vector3.zero;
            buildingBlockBody.velocity = Vector3.zero;
            buildingBlockBody.useGravity = true;
            buildingBlockBody.rotation = Quaternion.identity;
            buildingBlockBody.constraints = RigidbodyConstraints.FreezeRotationX |
                                            RigidbodyConstraints.FreezeRotationY;
            
            var buildingBlockConstantForce =
                buildingBlockBody.gameObject.AddComponent<ConstantForce>();
            buildingBlockConstantForce.force = Physics.gravity * 10f;
            
            Destroy(_attachedBuildingBlock.GetComponent<HingeJoint>());
            _attachedBuildingBlock = null;
        }
    }
}
