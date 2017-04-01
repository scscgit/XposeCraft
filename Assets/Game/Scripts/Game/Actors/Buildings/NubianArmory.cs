using XposeCraft.Game.Enums;

namespace XposeCraft.Game.Actors.Buildings
{
    public class NubianArmory : Building, IUnitProduction
    {
        public new bool CreateUnit(UnitType type)
        {
            return base.CreateUnit(type);
        }

        public new int QueuedUnits
        {
            get { return base.QueuedUnits; }
        }

        public new int QueueLimit
        {
            get { return base.QueueLimit; }
        }
    }
}
