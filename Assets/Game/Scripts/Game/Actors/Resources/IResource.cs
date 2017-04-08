using System.Collections.Generic;
using XposeCraft.Core.Faction.Units;

namespace XposeCraft.Game.Actors.Resources
{
    public interface IResource : IActor
    {
        void GatherByWorker(List<UnitController> builderUnits);
    }
}
