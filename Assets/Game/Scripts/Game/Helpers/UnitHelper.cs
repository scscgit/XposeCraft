using System.Collections.Generic;
using System.Linq;
using XposeCraft.Game.Actors.Units;
using XposeCraft.GameInternal;

namespace XposeCraft.Game.Helpers
{
    /// <summary>
    /// Provide easy access for operations, that may be complicated or impossible by using direct API of other classess.
    /// TODO: decide what hierarchy should they have.
    /// </summary>
    class UnitHelper : ActorHelper<IUnit>
    {
        public static IList<TUnit> GetUnitsAsList<TUnit>() where TUnit : IUnit
        {
            var list = new List<TUnit>();
            ForEach<TUnit>(unit => { list.Add(unit); }, Model.Instance.Units);
            return list;
        }

        // TODO: GetUnits() overload that implies UnitType = IUnit, but it causes error when I simply overload it
        public static TUnit[] GetUnits<TUnit>() where TUnit : IUnit
        {
            return GetUnitsAsList<TUnit>().ToArray();
        }
    }
}
