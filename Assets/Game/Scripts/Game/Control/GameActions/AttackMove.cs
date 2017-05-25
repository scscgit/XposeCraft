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

        internal override bool Progress(IUnit unit, UnitController unitController)
        {
            if (!base.Progress(unit, unitController))
            {
                return false;
            }
            // If the Action was already running and any target wasn't followed, it has ended
            if (unitController.IsAttackMove && unitController.AttackMoveTarget == null)
            {
                unitController.IsAttackMove = false;
                return true;
            }
            // Starts the movement towards the goal
            Move.MoveToPosition(Where, unitController);
            unitController.IsAttackMove = true;
            unitController.AttackMoveTarget = null;
            return false;
        }
    }
}
