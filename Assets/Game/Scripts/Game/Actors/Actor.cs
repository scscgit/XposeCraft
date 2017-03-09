namespace XposeCraft.Game.Actors
{
    public abstract class Actor : IActor
    {
        protected Actor(Position position)
        {
            Position = position;
        }

        public Position Position { get; protected set; }
    }
}
