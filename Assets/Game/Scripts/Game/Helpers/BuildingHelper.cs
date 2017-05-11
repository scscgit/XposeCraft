using System.Collections.Generic;
using System.Linq;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.GameInternal;

namespace XposeCraft.Game.Helpers
{
    /// <summary>
    /// Helper class providing static methods for example to find <see cref="IBuilding"/> instances in the world.
    /// <inheritdoc cref="ActorHelper{TForActorHelper}"/>
    /// </summary>
    public sealed class BuildingHelper : ActorHelper<IBuilding>
    {
        public static IList<TBuilding> GetBuildingsAsList<TBuilding>() where TBuilding : IBuilding
        {
            var list = new List<TBuilding>();
            ForEach<TBuilding, Building>(building => { list.Add(building); }, Player.CurrentPlayer.Buildings);
            return list;
        }

        public static TBuilding[] GetBuildings<TBuilding>() where TBuilding : IBuilding
        {
            return GetBuildingsAsList<TBuilding>().ToArray();
        }

        /// <inheritdoc cref="GetVisibleEnemyBuildings{TUnit}"/>
        /// <typeparam name="TBuilding">Type of the Buildings to be searched for.</typeparam>
        /// <returns>List of Buildings</returns>
        public static IList<TBuilding> GetVisibleEnemyBuildingsAsList<TBuilding>() where TBuilding : IBuilding
        {
            var list = new List<TBuilding>();
            ForEach<TBuilding, Building>(
                building => { list.Add(building); }, Player.CurrentPlayer.EnemyVisibleBuildings);
            return list;
        }

        /// <summary>
        /// Finds enemy Buildings, that the Player curently sees within the Fog of War. They cannot be controlled.
        /// </summary>
        /// <typeparam name="TBuilding">Type of the Buildings to be searched for.</typeparam>
        /// <returns>Array of Buildings.</returns>
        public static TBuilding[] GetVisibleEnemyBuildings<TBuilding>() where TBuilding : IBuilding
        {
            return GetVisibleEnemyBuildingsAsList<TBuilding>().ToArray();
        }

        public static Position ClosestEmptySpaceTo(IBuilding building)
        {
            // TODO: implement, demo
            return null;
        }
    }
}
