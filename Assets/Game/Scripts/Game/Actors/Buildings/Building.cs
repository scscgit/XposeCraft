namespace XposeCraft.Game.Actors.Buildings
{
    public abstract class Building : Actor, IBuilding
    {
        public bool IsFinished { get; set; }
    }
}
