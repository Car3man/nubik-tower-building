using NubikTowerBuilding.Behaviours;
using NubikTowerBuilding.Models;
using NubikTowerBuilding.Utility;
using UnityEngine;
using Zenject;

namespace NubikTowerBuilding.Managers
{
    public class SwingManager : MonoBehaviour
    {
        [Inject] private BuildPlatform _buildPlatform;
        [Inject] private Tower _tower;
        [SerializeField] private float maxHorizontalSwing;
        
        private float _swingFrequencyChangeSpeed;
        private float _swingAmplitudeChangeSpeed;
        private float _currSwingFrequency;
        private float _currSwingAmplitude;
        private float _targetSwingFrequency;
        private float _targetSwingAmplitude;
        
        public void CleanUp()
        {
            _currSwingFrequency = 1.5f;
            _currSwingAmplitude = 0f;
            _targetSwingFrequency = 1.5f;
            _targetSwingAmplitude = 0f;
        }
        
        private void Start()
        {
            _tower.OnAttachBuildingBlock += TowerOnAttachBuildingBlock;
            
            _swingFrequencyChangeSpeed = 0.005f;
            _swingAmplitudeChangeSpeed = 0.5f;
            
            UpdateTowerSwing();
        }

        private void Update()
        {
            InterpolateTowerSwing();
        }

        private void TowerOnAttachBuildingBlock(AttachBuildingBlockResult result)
        {
            if (result.IsSuccess)
            {
                UpdateTowerSwing();
            }
        }

        private void UpdateTowerSwing()
        {
            _targetSwingFrequency = 1.5f;
            _targetSwingAmplitude = 0f;
            
            if (_tower.GetHeight() >= 20)
            {
                _targetSwingAmplitude = 20 * 0.25f;
                _targetSwingFrequency = 1.5f + Mathf.Max(0f, (_tower.GetHeight() - 10) * 0.04f);
            }
            else if (_tower.GetHeight() >= 5)
            {
                _targetSwingAmplitude = _tower.GetHeight() * 0.15f;
            }

            if (_tower.GetHeight() > 1)
            {
                var minSwingPoint = GetTowerBlockMinSwingPoint(_tower.GetBlocks()[^1]);
                var maxSwingPoint = GetTowerBlockMaxSwingPoint(_tower.GetBlocks()[^1]);
                var swingHorDistance = Mathf.Abs(maxSwingPoint.x - minSwingPoint.x);
                var currMaxDiff = swingHorDistance / maxHorizontalSwing;
                if (currMaxDiff > 1f)
                {
                    _targetSwingAmplitude /= currMaxDiff;
                }
            }
        }

        private void InterpolateTowerSwing()
        {
            _currSwingFrequency = Mathf.MoveTowards(_currSwingFrequency, _targetSwingFrequency,
                _swingFrequencyChangeSpeed * Time.deltaTime);
            _currSwingAmplitude = Mathf.MoveTowards(_currSwingAmplitude, _targetSwingAmplitude,
                _swingAmplitudeChangeSpeed * Time.deltaTime);
            
            _tower.SetSwing(_currSwingAmplitude, _currSwingFrequency);
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (_tower.GetHeight() > 0)
            {
                var towerBlockFixedPoint = GetTowerBlockFixedPoint(_tower.GetBlocks()[^1]);

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(towerBlockFixedPoint, 0.6f);
                
                Gizmos.color = Color.blue;
                var minSwingPoint = GetTowerBlockMinSwingPoint(_tower.GetBlocks()[^1]);
                var maxSwingPoint = GetTowerBlockMaxSwingPoint(_tower.GetBlocks()[^1]);
                Gizmos.DrawSphere(minSwingPoint, 0.5f);
                Gizmos.DrawSphere(maxSwingPoint, 0.5f);
            }
        }

        private Vector3 GetTowerBlockFixedPoint(BuildingBlock block)
        {
            var blockHeight = block.GetDimensions().y;
            var blocks = _tower.GetBlocks();
            var fixedTop = new Vector3(0f, _buildPlatform.transform.position.y, 0f);
            for (int i = 0; i < blocks.Count; i++)
            {
                fixedTop += new Vector3(blocks[i].transform.localPosition.x, blockHeight, 0f);
                if (blocks[i] == block)
                {
                    break;
                }
            }
            fixedTop.y -= blockHeight / 2f;
            return fixedTop;
        }
        
        private Vector3 GetTowerBlockMinSwingPoint(BuildingBlock block)
        {
            const float pi3By2 = 3f * Mathf.PI / 2f;
            return GetTowerBlockSwingPoint(block, pi3By2);
        }

        private Vector3 GetTowerBlockMaxSwingPoint(BuildingBlock block)
        {
            const float piBy2 = Mathf.PI / 2f;
            return GetTowerBlockSwingPoint(block, piBy2);
        }
        
        private Vector3 GetTowerBlockSwingPoint(BuildingBlock block, float x)
        {
            var blocks = _tower.GetBlocks();
            var lowerBlock = blocks[0];
            var lowerBlockFixedPoint = GetTowerBlockFixedPoint(lowerBlock);
            var currBlockFixedPoint = GetTowerBlockFixedPoint(block);
            
            var maxSwing = _tower.GetSwingValue(x);
            var maxSwingPoint = MathUtility.RotatePointAroundPivot(
                currBlockFixedPoint,
                lowerBlockFixedPoint,
                Mathf.Deg2Rad * maxSwing
            );
            
            return maxSwingPoint;
        }

        private float GetSwingCorrectionAngle()
        {
            var blocks = _tower.GetBlocks();
            if (blocks.Count < 1)
            {
                return 0f;
            }

            var b = GetTowerBlockFixedPoint(blocks[^1]);
            var a = GetTowerBlockFixedPoint(blocks[0]);
            var angle = Quaternion.LookRotation(b - a) * Quaternion.Euler(-270f, 0f, 0f);
            return -angle.eulerAngles.x;
        }
    }
}
