using System;
using System.Collections.Generic;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Core.Required;
using XposeCraft.Game.Actors.Units;
using XposeCraft.GameInternal;
using XposeCraft.GameInternal.Helpers;

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
            // Move
            UnitSelection.SetTarget(
                new List<UnitController> {unitController},
                GameManager.Instance.Terrain.gameObject,
                PositionHelper.PositionToLocation(Where));
            // TODO: implementing this will also solve the issue of finding MyUnit argument of an enemy detection event
            Log.e("The AttackMove feature is not implemented yet, applying Move");
            return true;
        }
    }
}
