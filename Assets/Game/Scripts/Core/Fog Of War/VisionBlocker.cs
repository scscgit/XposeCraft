using UnityEngine;
using XposeCraft.GameInternal;

namespace XposeCraft.Core.Fog_Of_War
{
    public class VisionBlocker : MonoBehaviour
    {
        // This script sets up static areas on the map where the unit cannot see
        // past. Use this for blocking vision on buildings or high terrain.
        // The blocked area will be a circle with the defined radius.

        public int radius = 1;
        public int height = -1;

        void Start()
        {
            GameManager.Instance.Fog.ClosePoints(radius, transform.position, height);
        }
    }
}
