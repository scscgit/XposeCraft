using System;

namespace XposeCraft.Game.Actors.Buildings
{
    public class UnitProductionException : Exception
    {
        public UnitProductionException() : base("Unit is not available")
        {
        }

        public UnitProductionException(string message) : base(message)
        {
        }
    }
}
