namespace XposeCraft.Game.Actors.Buildings
{
    abstract class Building : Actor, IBuilding
    {
        public bool IsFinished { get; set; }

        protected Building(Position position) : base(position)
        {
        }
    }
}
