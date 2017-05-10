using XposeCraft.Core.Faction.Units;

namespace XposeCraft.Game.Actors.Buildings
{
    public interface IBuilding : IActor
    {
        /// <summary>
        /// Internal method, do not use.
        /// </summary>
        void AttackedByUnit(UnitController builderUnit);

        /// <summary>
        /// Internal method, do not use.
        /// </summary>
        void FinishBuildingByWorker(UnitController unitController);

        /// <summary>
        /// Is true if the building is already finished, which means its construction is not in a progress.
        /// </summary>
        bool Finished { get; }

        float Progress { get; }
    }
}
