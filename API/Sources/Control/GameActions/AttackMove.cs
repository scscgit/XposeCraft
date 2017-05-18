using System;

namespace XposeCraft.Game.Control.GameActions
{
    /// <summary>
    /// Action of a movement to a new position, during which the unit attacks any enemies before it continues to move.
    /// </summary>
    public class AttackMove : GameAction
    {
        public Position Where { get; private set; }

        public AttackMove(Position where)
        {
            throw new NotImplementedException();
        }
    }
}
