using System;

namespace XposeCraft.Game.Actors.Resources
{
    public class ResourceExhaustedException : Exception
    {
        public ResourceExhaustedException() : base("Resource is not available for gathering anymore")
        {
        }

        public ResourceExhaustedException(IResource resource) : base(
            "Resource " + resource.GetType().Name + " is not available for gathering anymore")
        {
        }

        public ResourceExhaustedException(string message) : base(message)
        {
        }
    }
}
