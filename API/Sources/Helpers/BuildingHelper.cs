using System;
using System.Collections.Generic;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Enums;

namespace XposeCraft.Game.Helpers
{
    /// <summary>
    /// Helper class providing static methods for example to find <see cref="IBuilding"/> instances in the world.
    /// <inheritdoc cref="ActorHelper{TForActorHelper}"/>
    /// </summary>
    public sealed class BuildingHelper : ActorHelper<IBuilding>
    {
        /// <inheritdoc cref="GetMyBuildings{TBuilding}"/>
        /// <typeparam name="TBuilding">Type of the Building to be searched for.</typeparam>
        /// <returns>List of Buildings.</returns>
        public static IList<TBuilding> GetMyBuildingsAsList<TBuilding>() where TBuilding : IBuilding
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds Player's owned Buildings, that he can control in the world.
        /// </summary>
        /// <typeparam name="TBuilding">Type of the Building to be searched for.</typeparam>
        /// <returns>Array of Buildings.</returns>
        public static TBuilding[] GetMyBuildings<TBuilding>() where TBuilding : IBuilding
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="GetVisibleEnemyBuildings{TUnit}"/>
        /// <typeparam name="TBuilding">Type of the Buildings to be searched for.</typeparam>
        /// <returns>List of Buildings</returns>
        public static IList<TBuilding> GetVisibleEnemyBuildingsAsList<TBuilding>() where TBuilding : IBuilding
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds enemy Buildings, that the Player curently sees within the Fog of War. They cannot be controlled.
        /// </summary>
        /// <typeparam name="TBuilding">Type of the Buildings to be searched for.</typeparam>
        /// <returns>Array of Buildings.</returns>
        public static TBuilding[] GetVisibleEnemyBuildings<TBuilding>() where TBuilding : IBuilding
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds a closest empty space near a Building, that is valid for a placement of a specific type of a Building.
        /// </summary>
        /// <param name="closestToBuilding">Building near which the placement is searched from.</param>
        /// <param name="forBuildingPlacement">Building type which should be able to be placed there.</param>
        /// <returns>A vali</returns>
        public static Position ClosestEmptySpaceTo(IBuilding closestToBuilding, BuildingType forBuildingPlacement)
        {
            throw new NotImplementedException();
        }
    }
}
