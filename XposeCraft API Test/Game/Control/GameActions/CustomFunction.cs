using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XposeCraft_UI_API_Prototype_Test.Game.Control.GameActions
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
