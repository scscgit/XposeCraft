using UnityEngine;
using XposeCraft.Core.Resources;

namespace XposeCraft.GameInternal.Helpers
{
    public class ResourceHelper
    {
        public static int GetMinerals(ResourceManager resourceManager)
        {
            var resource = resourceManager.resourceTypes[0];
            Debug.Assert(resource.name.Equals("Minerals"));
            return resource.amount;
        }
    }
}
