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

            var verticalSwing = Mathf.Sin(Time.time * ropSwingFrequency.y) * ropSwingAmplitude.y;
            newHookPosition += Vector3.up * verticalSwing;
            
            craneHookBody.MovePosition(newHookPosition);
        }

        public Rigidbody GetHook()
        {
            return craneHookBody;
        }
    }
}
