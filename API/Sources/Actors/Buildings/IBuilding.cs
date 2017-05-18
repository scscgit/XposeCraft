namespace XposeCraft.Game.Actors.Buildings
{
    public interface IBuilding : IActor
    {
        /// <summary>
        /// True if the Building is already fully constructed, which means the construction is not in a progress.
        /// </summary>
        bool Finished { get; }

        /// <summary>
        /// Percentage progress of the Building construction with values between 0 and 1.
        /// </summary>
        float ConstructionProgress { get; }
    }
}
