using UnityEngine;
using XposeCraft.Game;

namespace XposeCraft.GameInternal.Helpers
{
    public class PositionHelper
    {
        public static Vector3 PositionToLocation(Position position)
        {
            return GameManager.Instance.Grid.points[position.PointLocation].loc;
        }
    }
}
