namespace XposeCraft.Game.Actors.Resources
{
    /// <inheritdoc cref="IResource"/>
    public abstract class Resource : Actor, IResource
    {
        public bool Exhausted { get; }
    }
}
