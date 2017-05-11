namespace XposeCraft.Game.Actors.Buildings
{
    public interface IBuilding : IActor
    {
        /// <summary>
        /// Is true if the building is already finished, which means its construction is not in a progress.
        /// </summary>
        bool Finished { get; }

        float Progress { get; }
    }
}
