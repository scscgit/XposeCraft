using XposeCraft.Game.Enums;

namespace XposeCraft.Game.Actors.Buildings
{
    public interface IUnitProduction
    {
        bool CreateUnit(UnitType type);
        int QueuedUnits { get; }
        int QueueLimit { get; }
    }
}
