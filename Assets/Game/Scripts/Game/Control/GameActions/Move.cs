namespace XposeCraft.Game.Control.GameActions
{
    /// <summary>
    /// Action of a movement to a new position
    /// </summary>
    class Move : GameAction
    {
        private Position Where { get; set; }

        public Move(Position where)
        {
            Where = where;
        }
    }
}
