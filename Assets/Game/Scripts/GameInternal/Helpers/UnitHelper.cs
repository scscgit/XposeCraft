using UnityEngine;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Core.Fog_Of_War;

namespace XposeCraft.GameInternal.Helpers
{
    public class UnitHelper
    {
        public static GameObject InstantiateUnit(GameObject prefab, Vector3 location, int factionIndex)
        {
            var obj = Object.Instantiate(prefab, location, Quaternion.identity);
            var script = obj.GetComponent<UnitController>();
            if (script)
            {
                // Changes the Faction to a correct one, removing previous vision registration initialized during start
                var signal = script.GetComponent<VisionSignal>();
                if (signal)
                {
                    signal.OnDisable();
                }
                script.FactionIndex = factionIndex;
                if (signal)
                {
                    signal.OnEnable();
                }
            }
            return obj;
        }
    }
}
