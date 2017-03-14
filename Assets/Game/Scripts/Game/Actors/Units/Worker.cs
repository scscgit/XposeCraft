using UnityEngine;
using XposeCraft.Game.Actors.Materials;
using XposeCraft.Game.Enums;

namespace XposeCraft.Game.Actors.Units
{
    /// <summary>
    /// Can gather various materials (mainly minerals) and build various buildings.
    /// </summary>
    public class Worker : Unit
    {
        public IMaterial Gathering { get; private set; }

        public Worker(GameObject gameObject) : base(gameObject)
        {
        }

        public void SendGather(IMaterial material)
        {
            // TODO: override ReplaceActionQueue to set Gathering back to null; make Gathering an action
            Gathering = material;
        }

        // TODO:
        // 1. send worker to the position near building, queued event when arrival
        // 2. start construction object, queued event when finished
        // 2.5. if interrupted, worker can repeat step 1 and continue on 2 without creating a new object
        // 3. finished event, return to gather
        public void CreateBuilding(BuildingType buildingType, Position position)
        {
        }
    }
}
