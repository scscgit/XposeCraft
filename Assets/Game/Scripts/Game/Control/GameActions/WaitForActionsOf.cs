using XposeCraft.Game.Actors.Units;

namespace XposeCraft.Game.Control.GameActions
{
    /// <summary>
    /// Action of waiting for other unit to finish its queue
    /// </summary>
    class WaitForActionsOf : GameAction
    {
        IUnit[] Targets;

        public WaitForActionsOf(IUnit target)
        {
            Targets = new IUnit[1];
            Targets[0] = target;
        }

        public WaitForActionsOf(IUnit[] targets)
        {
            Targets = targets;
        }
    }
}
