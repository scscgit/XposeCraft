namespace XposeCraft.Game.Control.GameActions
{
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
