using System;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Game.Actors.Units;

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
            Where = where;
        }

        public override bool Progress(IUnit unit, UnitController unitController)
        {
            if (!base.Progress(unit, unitController))
            {
                return false;
            }
            // TODO: implementing this will also solve the issue of finding MyUnit argument of an enemy detection event
            throw new NotImplementedException();
        }
    }
}
