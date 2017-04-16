using System;
using XposeCraft.Game.Actors;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Units;

namespace XposeCraft.Game.Control.GameActions
{
    /// <summary>
    /// Action of attack on a unit.
    /// </summary>
    class Attack : GameAction
    {
        private IActor Target { get; set; }

        public Attack(IActor target)
        {
            if (!(target is IUnit) && !(target is IBuilding))
            {
                throw new InvalidOperationException("The target actor is not valid: it is not a Unit or a Building.");
            }
            Target = target;
        }
    }
}
