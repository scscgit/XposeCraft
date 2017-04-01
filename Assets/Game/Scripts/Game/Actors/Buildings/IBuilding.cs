using System.Collections.Generic;
using XposeCraft.Core.Faction.Units;

namespace XposeCraft.Game.Actors.Buildings
{
    public interface IBuilding : IActor
    {
        bool Finished { get; }
        float Progress { get; }
        void FinishBuildingByWorker(List<UnitController> unitControllers);
    }
}
