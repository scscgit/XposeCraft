using System;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Game.Actors;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Units;

namespace XposeCraft.Game.Control.GameActions
{
    /// <summary>
    /// Action of attack on a unit.
    /// </summary>
    public class Attack : GameAction
    {
        private IActor _target;

        public IActor Target
        {
            get { return _target; }
            // TODO: verify the experimental setter API if there are any hazards
            set
            {
                if (!(value is IUnit) && !(value is IBuilding))
                {
                    throw new InvalidOperationException(
                        "The target actor is not valid: it is not a Unit or a Building.");
                }
                _target = value;
            }
        }

        public Attack(IActor target)
        {
            Target = target;
        }

        internal override bool Progress(IUnit unit, UnitController unitController)
        {
            if (!base.Progress(unit, unitController))
            {
                return false;
            }
            var targetUnit = Target as Unit;
            if (targetUnit != null)
            {
                targetUnit.AttackedByUnit(unitController);
                return true;
            }
            var targetBuilding = Target as Building;
            if (targetBuilding == null)
            {
                throw new Exception(
                    "Fatal error, constructor hasn't properly asserted that Target is Unit or Building");
            }
            targetBuilding.AttackedByUnit(unitController);
            return true;
        }
    }
}
