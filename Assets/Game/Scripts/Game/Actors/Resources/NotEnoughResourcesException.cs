using System;

namespace XposeCraft.Game.Actors.Resources
{
    public class NotEnoughResourcesException : Exception
    {
        public NotEnoughResourcesException() : base("Not enough resources")
        {
        }

        public NotEnoughResourcesException(string message) : base(message)
        {
        }
    }
}
