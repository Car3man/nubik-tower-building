using UnityEngine;

namespace NubikTowerBuilding.Ui.Elements
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private RectTransform maskRectTransform;
        [SerializeField] private RectTransform fillRectTransform;
        
        public void SetValue(float value)
        {
            float maskWidth = maskRectTransform.rect.width;
            float widthValue = maskWidth * Mathf.Clamp01(1f - value);
            fillRectTransform.offsetMin = new Vector2(-widthValue, fillRectTransform.offsetMin.y);
            fillRectTransform.offsetMax = new Vector2(-widthValue, fillRectTransform.offsetMax.y);
        }
    }
}
