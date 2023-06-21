using UnityEngine;

namespace NubikTowerBuilding.Behaviours
{
    public class Crane : MonoBehaviour
    {
        [SerializeField] private Transform craneOrigin;
        [SerializeField] private Transform craneHook;
        [SerializeField] private float ropeLength;
        [SerializeField] private LineRenderer ropeLineRenderer;

        private float _lastHookPhysicX;
        private float _fakeAttachmentVelocity;
        private Vector2 _ropSwingAmplitude;
        private Vector2 _ropSwingFrequency;
        private float _startOriginHeight;
        private float _currOriginHeight;
        private readonly Vector3[] _ropeDisplayPositions = new Vector3[2];
        private BuildingBlock _attachedBuildingBlock;

        private void Start()
        {
            _lastHookPhysicX = 0f;
            _fakeAttachmentVelocity = 0f;
            _startOriginHeight = craneOrigin.transform.position.y;
        }

        private void FixedUpdate()
        {
            var currHookPos = craneHook.transform.position.x;
            _fakeAttachmentVelocity = (currHookPos - _lastHookPhysicX) * 1024f * Time.fixedDeltaTime;
            _lastHookPhysicX = currHookPos;
        }

        private void Update()
        {
            MoveHook();
            MoveHookAttachment();
            UpdateRope();
        }

        private void MoveHook()
        {
            var newHookPosition = GetOriginPoint() + Vector3.down * ropeLength;

            newHookPosition += Vector3.right * GetHorizontalSwing();
            newHookPosition += Vector3.up * GetVerticalSwing();

            craneHook.position = newHookPosition;
        }

        private void MoveHookAttachment()
        {
            if (_attachedBuildingBlock != null)
            {
                var blockHeight = _attachedBuildingBlock.GetDimensions().y;
                var attachmentTrans = _attachedBuildingBlock.transform;
                var attachmentNewRot = Quaternion.Euler(0f, 0f, attachmentTrans.position.x);
                var attachmentCranePos = GetHookConnectPoint(blockHeight);
                var attachmentPivot = attachmentCranePos + Vector3.up * (blockHeight / 2f);
                var attachmentNewPos = attachmentNewRot * (attachmentCranePos - attachmentPivot) + attachmentPivot;
                attachmentTrans.position = attachmentNewPos;
                attachmentTrans.rotation = attachmentNewRot;
            }
        }

        private void UpdateRope()
        {
            _ropeDisplayPositions[0] = GetOriginPoint();
            _ropeDisplayPositions[1] = craneHook.position;
            ropeLineRenderer.SetPositions(_ropeDisplayPositions);
        }

        public void SetSwingAmplitude(Vector2 amplitude)
        {
            _ropSwingAmplitude = amplitude;
        }

        public void SetSwingFrequency(Vector2 frequency)
        {
            _ropSwingFrequency = frequency;
        }
        
        public void SetOriginHeight(float originHeight)
        {
            _currOriginHeight = originHeight;
        }

        private float GetHorizontalSwing()
        {
            return Mathf.Sin(Time.time * _ropSwingFrequency.x) * _ropSwingAmplitude.x;
        }

        private float GetVerticalSwing()
        {
            return Mathf.Sin(Time.time * _ropSwingFrequency.y + 90f * Mathf.Deg2Rad) * _ropSwingAmplitude.y;
        }
        
        public Vector3 GetOriginPoint()
        {
            return new Vector3(transform.position.x, _startOriginHeight + _currOriginHeight, transform.position.z);
        }
        
        public Vector3 GetHookConnectPoint(float objectHeight)
        {
            var hookHeight = craneHook.transform.localScale.y;
            var hookPosition = craneHook.position;
            hookPosition += Vector3.down * hookHeight / 2f;
            hookPosition += Vector3.down * objectHeight / 2f;
            return hookPosition;
        }

        public void DetachBuildingBlock()
        {
            if (_attachedBuildingBlock == null)
            {
                Debug.LogWarning("Crane doesn't have any attached blocks");
                return;
            }
            
            _attachedBuildingBlock = null;
        }

        public void AttachBuildingBlock(BuildingBlock buildingBlock)
        {
            if (_attachedBuildingBlock != null)
            {
                Debug.LogWarning("BuildPlatform already have attached building block");
                return;
            }
            
            _attachedBuildingBlock = buildingBlock;
            
            var buildingBlockBody = _attachedBuildingBlock.GetBody();
            buildingBlockBody.angularDrag = 20f;
            buildingBlockBody.useGravity = false;
            buildingBlockBody.isKinematic = true;
            MoveHookAttachment();
        }

        public void DropBuildingBlock()
        {
            var buildingBlockBody = _attachedBuildingBlock.GetBody();
            buildingBlockBody.isKinematic = false;
            buildingBlockBody.useGravity = true;
            buildingBlockBody.angularDrag = 0.05f;
            buildingBlockBody.rotation = Quaternion.identity;
            buildingBlockBody.velocity = Vector3.right * _fakeAttachmentVelocity;
            buildingBlockBody.constraints = RigidbodyConstraints.FreezeRotationX |
                                            RigidbodyConstraints.FreezeRotationY;
            
            var buildingBlockConstantForce =
                buildingBlockBody.gameObject.AddComponent<ConstantForce>();
            buildingBlockConstantForce.force = Physics.gravity * 40f;

            Destroy(_attachedBuildingBlock.GetComponent<HingeJoint>());
            _attachedBuildingBlock = null;
        }
    }
}
