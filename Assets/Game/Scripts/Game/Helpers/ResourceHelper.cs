using System.Collections.Generic;
using XposeCraft.Game.Actors;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Resources;
using XposeCraft.Game.Actors.Resources.Minerals;
using XposeCraft.Game.Actors.Units;
using XposeCraft.GameInternal;

namespace XposeCraft.Game.Helpers
{
    /// <summary>
    /// Helper class providing static methods for example to find <see cref="IResource"/> instances in the world.
    /// <inheritdoc cref="ActorHelper{TForActorHelper}"/>
    /// </summary>
    public sealed class ResourceHelper : ActorHelper<IResource>
    {
        /// <inheritdoc cref="GetResources{TResource}"/>
        /// <typeparam name="TResource">Type of the Resource to be searched for.</typeparam>
        /// <returns>List of resources.</returns>
        public static List<TResource> GetResourcesAsList<TResource>() where TResource : IResource
        {
            var list = new List<TResource>();
            ForEach<TResource, Resource>(resource => { list.Add(resource); }, Player.CurrentPlayer.Resources);
            return list;
        }

        /// <summary>
        /// Finds all Resources in the world, that are available to the Player.
        /// They can be collected by any <see cref="Worker"/>.
        /// </summary>
        /// <typeparam name="TResource">Type of the Resource to be searched for.</typeparam>
        /// <returns>Array of resources.</returns>
        public static TResource[] GetResources<TResource>() where TResource : IResource
        {
            return GetResourcesAsList<TResource>().ToArray();
        }

        /// <summary>
        /// Finds a closest available Mineral to an Actor.
        /// </summary>
        /// <param name="actor">Actor whose Position will used for the search reference.</param>
        /// <returns>A closest Mineral.</returns>
        public static Mineral GetNearestMineralTo(IActor actor)
        {
            return GetNearestResourceTo<Mineral>(actor);
        }

        /// <summary>
        /// Finds a closest available Resource to an Actor.
        /// </summary>
        /// <param name="actor">Actor whose Position will used for the search reference.</param>
        /// <typeparam name="TResource">Type of the Resource to be searched for.</typeparam>
        /// <returns>A closest Resource.</returns>
        public static TResource GetNearestResourceTo<TResource>(IActor actor) where TResource : IResource
        {
            const int initialMaxLengthFromBase = 90;
            var closestResource = default(TResource);
            var closestDistance = initialMaxLengthFromBase;
            ForEach<TResource, Resource>(resource =>
            {
                var distance = resource.Position.PathFrom(actor.Position).IsLengthLessThan(closestDistance);
                // Resources further away than the initial distance are discarded immediately
                if (distance == null)
                {
                    return;
                }
                closestResource = resource;
                closestDistance = distance.Value;
            }, Player.CurrentPlayer.Resources);
            return closestResource;
        }

        /// <summary>
        /// Finds all Resources situated near a Base.
        /// </summary>
        /// <param name="baseCenter">Base near which all nearest resources will be looked for.</param>
        /// <typeparam name="TResource">Type of the Resource to be searched for.</typeparam>
        /// <returns>List of resources.</returns>
        public static List<TResource> GetResourcesNearBase<TResource>(BaseCenter baseCenter)
            where TResource : IResource
        {
            const int maxPathLengthFromBase = 50;
            var building = (Building) baseCenter;
            // The building has all its resources cached
            if (building.NearbyResorces == null)
            {
                var allResourcesList = new List<Resource>();
                ForEach<Resource, Resource>(resource =>
                {
                    if (resource.Position.PathFrom(baseCenter.Position).IsLengthLessThan(maxPathLengthFromBase) != null)
                    {
                        allResourcesList.Add(resource);
                    }
                }, Player.CurrentPlayer.Resources);
                building.NearbyResorces = allResourcesList;
            }
            // They are further filtered by the chosen generic type
            var list = new List<TResource>();
            ForEach<TResource, Resource>(resource => { list.Add(resource); }, building.NearbyResorces);
            return list;
        }
    }
}
