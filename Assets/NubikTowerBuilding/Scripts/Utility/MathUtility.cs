using UnityEngine;

namespace NubikTowerBuilding.Utility
{
    public static class MathUtility
    {
        public static Vector2 RotatePointAroundPivot(Vector2 point, Vector2 pivot, float radians)
        {
            var cosTheta = Mathf.Cos(radians);
            var sinTheta = Mathf.Sin(radians);

            var x = cosTheta * (point.x - pivot.x) - sinTheta * (point.y - pivot.y) + pivot.x;
            var y = sinTheta * (point.x - pivot.x) + cosTheta * (point.y - pivot.y) + pivot.y;

            return new Vector2(x, y);
        }
    }
}
