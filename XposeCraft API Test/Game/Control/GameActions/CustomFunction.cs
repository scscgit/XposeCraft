using System;
using System.Collections.Generic;

namespace XposeCraft.Game.Control.GameActions
{
	class CustomFunction : GameAction
	{
		public delegate void CustomFunctionDelegate();
		CustomFunctionDelegate Function;

		public CustomFunction(CustomFunctionDelegate function)
		{
			Function = function;
		}
	}
}
