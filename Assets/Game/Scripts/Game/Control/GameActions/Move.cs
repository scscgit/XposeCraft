using System.Collections.Generic;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Core.Required;
using XposeCraft.Game.Actors.Units;
using XposeCraft.GameInternal;
using XposeCraft.GameInternal.Helpers;

namespace XposeCraft.Game.Control.GameActions
{
    /// <summary>
    /// Action of a movement to a new position.
    /// </summary>
    public class Move : GameAction
    {
        private Position Where { get; set; }

        public Move(Position where)
        {
            Where = where;
        }

        public override bool Progress(IUnit unit, UnitController unitController)
        {
            if (!base.Progress(unit, unitController))
            {
                return false;
            }
            UnitSelection.SetTarget(
                new List<UnitController> {unitController},
                GameManager.Instance.Terrain.gameObject,
                PositionHelper.PositionToLocation(Where));
            return true;
        }
    }
}
