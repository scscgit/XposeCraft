using UnityEngine;
using XposeCraft.Game.Enums;

namespace XposeCraft.Game.Actors.Buildings
{
    public class NubianArmory : Building
    {
        public NubianArmory(GameObject gameObject) : base(gameObject)
        {
        }

        public bool CreateUnit(UnitType type)
        {
            // TODO: add to the queue, event when created, public accessors to the current state
            return true;
        }
    }
}
