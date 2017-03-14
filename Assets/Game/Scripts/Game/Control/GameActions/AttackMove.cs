namespace XposeCraft.Game.Control.GameActions
{
    /// <summary>
    /// Action of a movement to a new position,
    /// during which the unit attacks any enemies before it continues
    /// </summary>
    class AttackMove : GameAction
    {
        private Position Where { get; set; }

        public AttackMove(Position where)
        {
            Where = where;
        }
    }
}
