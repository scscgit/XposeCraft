using System.Collections.Generic;
using System.Linq;
using XposeCraft.Game.Actors.Units;
using XposeCraft.GameInternal;

namespace XposeCraft.Game.Helpers
{
    /// <summary>
    /// Helper class providing static methods for example to find <see cref="IUnit"/> instances in the world.
    /// <inheritdoc cref="ActorHelper{TForActorHelper}"/>
    /// </summary>
    public sealed class UnitHelper : ActorHelper<IUnit>
    {
        /// <inheritdoc cref="GetMyUnits{TUnit}"/>
        /// <typeparam name="TUnit">Type of the Unit to be searched for.</typeparam>
        /// <returns>List of Units</returns>
        public static IList<TUnit> GetMyUnitsAsList<TUnit>() where TUnit : IUnit
        {
            Tutorial.Instance.GetUnitOrBuilding();
            var list = new List<TUnit>();
            ForEach<TUnit, Unit>(unit => { list.Add(unit); }, Player.CurrentPlayer.Units);
            return list;
        }

        /// <summary>
        /// Finds Player's owned Units, that he can control in the world.
        /// </summary>
        /// <typeparam name="TUnit">Type of the Unit to be searched for.</typeparam>
        /// <returns>Array of Units.</returns>
        public static TUnit[] GetMyUnits<TUnit>() where TUnit : IUnit
        {
            return GetMyUnitsAsList<TUnit>().ToArray();
        }

        /// <inheritdoc cref="GetVisibleEnemyUnits{TUnit}"/>
        /// <typeparam name="TUnit">Type of the Unit to be searched for.</typeparam>
        /// <returns>List of Units</returns>
        public static IList<TUnit> GetVisibleEnemyUnitsAsList<TUnit>() where TUnit : IUnit
        {
            var list = new List<TUnit>();
            ForEach<TUnit, Unit>(unit => { list.Add(unit); }, Player.CurrentPlayer.EnemyVisibleUnits);
            return list;
        }

        /// <summary>
        /// Finds enemy Units, that the Player curently sees within the Fog of War. They cannot be controlled.
        /// </summary>
        /// <typeparam name="TUnit">Type of the Unit to be searched for.</typeparam>
        /// <returns>Array of Units.</returns>
        public static TUnit[] GetVisibleEnemyUnits<TUnit>() where TUnit : IUnit
        {
            return GetVisibleEnemyUnitsAsList<TUnit>().ToArray();
        }
    }
}
