namespace XposeCraft.Game.Actors.Buildings
{
    class NubianArmory : Building
    {
        public NubianArmory(Position position) : base(position)
        {
        }

        public bool CreateUnit(Enums.UnitType type)
        {
            // TODO: add to the queue, event when created, public accessors to the current state
            return true;
        }
    }
}
