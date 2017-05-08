using UnityEngine;
using XposeCraft.Core.Faction.Units;

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
                script.FactionIndex = factionIndex;
            }
            return obj;
        }
    }
}
