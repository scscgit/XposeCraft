using XposeCraft.Core.Faction.Units;
using XposeCraft.Game.Actors.Units;

namespace XposeCraft.Game.Control.GameActions
{
    /// <summary>
    /// Action of invoking a custom function within the action queue.
    /// </summary>
    public class CustomFunction : GameAction
    {
        public delegate void CustomFunctionDelegate();

        private CustomFunctionDelegate Function { get; set; }

        public CustomFunction(CustomFunctionDelegate function)
        {
            Function = function;
        }

        public override bool Progress(IUnit unit, UnitController unitController)
        {
            if (!base.Progress(unit, unitController))
            {
                return false;
            }
            Function();
            return true;
        }
    }
}
