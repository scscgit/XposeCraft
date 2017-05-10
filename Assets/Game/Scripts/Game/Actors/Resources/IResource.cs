using System.Collections.Generic;
using XposeCraft.Core.Faction.Units;

namespace XposeCraft.Game.Actors.Resources
{
    public interface IResource : IActor
    {
        /// <summary>
        /// Internal method, do not use.
        /// </summary>
        void GatherByWorker(List<UnitController> builderUnits);
    }
}
