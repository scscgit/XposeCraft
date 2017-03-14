namespace XposeCraft.Game.Actors.Buildings
{
    public interface IBuilding : IActor
    {
        bool IsFinished { get; set; }
    }
}
