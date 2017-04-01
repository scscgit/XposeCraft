using System.Collections.Generic;
using System.Linq;
using XposeCraft.Game.Actors;
using XposeCraft.Game.Actors.Resources;
using XposeCraft.GameInternal;

namespace XposeCraft.Game.Helpers
{
    public sealed class ResourceHelper : ActorHelper<IResource>
    {
        public static IList<TResource> GetResourcesAsList<TResource>() where TResource : IResource
        {
            var list = new List<TResource>();
            ForEach<TResource, Resource>(resource => { list.Add(resource); }, Player.CurrentPlayer.Resources);
            return list;
        }

        public static TResource[] GetResources<TResource>() where TResource : IResource
        {
            return GetResourcesAsList<TResource>().ToArray();
        }

        public static TResource GetNearestResourceTo<TResource>(IActor actor) where TResource : IResource
        {
            TResource closestResource = default(TResource);
            ForEach<TResource, Resource>(resource =>
            {
                if (closestResource == null
                    || resource.Position.PathFrom(actor.Position) < closestResource.Position.PathFrom(actor.Position))
                {
                    closestResource = resource;
                }
            }, Player.CurrentPlayer.Resources);
            return closestResource;
        }
    }
}
