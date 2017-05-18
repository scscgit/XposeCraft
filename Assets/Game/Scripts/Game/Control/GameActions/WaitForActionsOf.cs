using XposeCraft.Core.Faction.Units;
using XposeCraft.Game.Actors.Units;

namespace XposeCraft.Game.Control.GameActions
{
    /// <summary>
    /// Action of waiting for other units to finish their own queue.
    /// </summary>
    public class WaitForActionsOf : GameAction
    {
        private IUnit[] Targets;

        public WaitForActionsOf(IUnit target)
        {
            Targets = new[] {target};
        }

        public WaitForActionsOf(IUnit[] targets)
        {
            Targets = targets;
        }

        internal override bool Progress(IUnit unit, UnitController unitController)
        {
            if (!base.Progress(unit, unitController))
            {
                return false;
            }
            foreach (var target in Targets)
            {
                if (target.ActionQueue.QueueCount != 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
