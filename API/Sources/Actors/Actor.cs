using System;

namespace XposeCraft.Game.Actors
{
    /// <summary>
    /// Representation of a Game Actor in Unity.
    /// </summary>
    public abstract class Actor : IActor
    {
        /// <summary>
        /// Current Position of the Actor.
        /// </summary>
        public Position Position
        {
            get { throw new NotImplementedException(); }
        }
    }
}
