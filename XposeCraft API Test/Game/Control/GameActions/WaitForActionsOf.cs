using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Units;

namespace XposeCraft_UI_API_Prototype_Test.Game.Control.GameActions
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
