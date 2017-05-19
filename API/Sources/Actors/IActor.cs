using XposeCraft.Game.Enums;

namespace XposeCraft.Game.Actors
{
    /// <summary>
    /// Representation of a Game Actor in Unity.
    /// </summary>
    public interface IActor
    {
        /// <summary>
        /// Current position of the Actor.
        /// </summary>
        Position Position { get; }

        /// <summary>
        /// Checks who owns the Actor. In case of the Owner all actions are available and it is always Visible.
        /// </summary>
        OwnershipType Ownership { get; }

        /// <summary>
        /// True if the Player can see the Actor, so it is not hidden in the Fog. Owned Actors are always visible.
        /// </summary>
        bool Visible { get; }
    }
}
