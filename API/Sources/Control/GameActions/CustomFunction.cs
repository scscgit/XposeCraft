using System;

namespace XposeCraft.Game.Control.GameActions
{
    /// <summary>
    /// Action of invoking a custom function within the action queue.
    /// </summary>
    public class CustomFunction : GameAction
    {
        public delegate void CustomFunctionDelegate();

        public CustomFunction(CustomFunctionDelegate function)
        {
            throw new NotImplementedException();
        }
    }
}
