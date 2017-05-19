using XposeCraft.Game.Actors.Units;

namespace XposeCraft.Game.Actors.Resources
{
    /// <summary>
    /// Resource placed in the game. Can be collected by <see cref="Worker"/>.
    /// </summary>
    public interface IResource : IActor
    {
        /// <summary>
        /// True if the Resource has been already exhausted and can never be used again.
        /// </summary>
        bool Exhausted { get; }
    }
}
