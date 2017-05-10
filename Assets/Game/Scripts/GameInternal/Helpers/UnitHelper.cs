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
            ChangeUnitFaction(script, factionIndex);
            return obj;
        }

        private static void ChangeUnitFaction(UnitController unitController, int factionIndex)
        {
            // Changes the Faction to a correct one, removing previous vision registration initialized during start
            var signal = unitController.GetComponent<VisionSignal>();
            var receiver = unitController.GetComponent<VisionReceiver>();
            if (signal)
            {
                signal.OnDisable();
            }
            if (receiver)
            {
                receiver.OnDisable();
            }
            unitController.FactionIndex = factionIndex;
            if (signal)
            {
                signal.OnEnable();
            }
            if (receiver)
            {
                receiver.OnEnable();
            }
        }
    }
}
