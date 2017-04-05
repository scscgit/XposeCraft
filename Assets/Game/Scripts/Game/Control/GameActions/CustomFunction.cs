namespace XposeCraft.Game.Control.GameActions
{
    /// <summary>
    /// Action of invoking a custom function within the action queue.
    /// </summary>
    class CustomFunction : GameAction
    {
        public delegate void CustomFunctionDelegate();

        private CustomFunctionDelegate Function { get; set; }

        public CustomFunction(CustomFunctionDelegate function)
        {
            Function = function;
        }
    }
}
