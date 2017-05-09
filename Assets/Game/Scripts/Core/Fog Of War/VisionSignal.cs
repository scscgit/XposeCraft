using UnityEngine;
using XposeCraft.GameInternal;

namespace XposeCraft.Core.Fog_Of_War
{
    public class VisionSignal : MonoBehaviour
    {
        // This sets up an object with dynamic vision on the fog of war
        // That vision will be represented by a transparent circle with the
        // defined radius.

        public int radius = 5;
        public int upwardSightHeight = 10;
        public int downwardSightHeight = 10;
        bool _quitting;

        public void OnEnable()
        {
            GameManager.Instance.Fog.AddSignalAgent(gameObject, radius, upwardSightHeight, downwardSightHeight, this);
        }

        private void OnApplicationQuit()
        {
            _quitting = true;
        }

        public void OnDisable()
        {
            if (_quitting)
            {
                return;
            }
            GameManager.Instance.Fog.RemoveSignalAgent(gameObject);
        }
    }
}
