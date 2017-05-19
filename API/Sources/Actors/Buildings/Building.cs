namespace XposeCraft.Game.Actors.Buildings
{
    /// <inheritdoc cref="IBuilding"/>
    public abstract class Building : Actor, IBuilding
    {
        public bool Finished { get; }

        public float ConstructionProgress { get; }

        public bool Destroyed { get; }
    }
}
