using XposeCraft.Game.Enums;

namespace XposeCraft.Game.Actors
{
    /// <inheritdoc cref="IActor"/>
    public abstract class Actor : IActor
    {
        public Position Position { get; }

        public OwnershipType Ownership { get; }

        public bool Visible { get; }
    }
}
