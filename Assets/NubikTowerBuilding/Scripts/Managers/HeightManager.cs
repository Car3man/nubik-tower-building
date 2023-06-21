using NubikTowerBuilding.Behaviours;
using UnityEngine;
using Zenject;

namespace NubikTowerBuilding.Managers
{
    public class HeightManager : MonoBehaviour
    {
        [Inject] private GameCamera _gameCamera;
        [Inject] private Crane _crane;
        [Inject] private Tower _tower;
        [SerializeField] private float changeHeightSpeed;

        private float _currHeight;

        public void CleanUp()
        {
            _currHeight = 0;
        }
        
        private void Start()
        {
            _currHeight = GetTargetHeightOffset();
        }

        private void Update()
        {
            UpdateHeights();
        }
        
        private void UpdateHeights()
        {
            UpdateCraneHeight();
            UpdateCameraHeight();

            _currHeight = Mathf.MoveTowards(
                _currHeight,
                GetTargetHeightOffset(),
                changeHeightSpeed * Time.deltaTime
            );
        }

        private void UpdateCraneHeight()
        {
            _crane.SetOriginHeight(_currHeight);
        }
        
        private void UpdateCameraHeight()
        {
            var cameraTrans = _gameCamera.transform;
            var cameraPos = cameraTrans.position;
            cameraPos.y = 10f + _currHeight;
            cameraTrans.position = cameraPos;
        }

        private float GetTargetHeightOffset()
        {
            var blocks = _tower.GetBlocks();
            var offset = 0f;
            for (int i = 1; i < blocks.Count; i++)
            {
                offset += blocks[i].GetDimensions().y;
            }
            return offset;
        }
    }
}
