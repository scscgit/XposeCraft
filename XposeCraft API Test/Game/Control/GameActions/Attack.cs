using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game.Actors;

namespace XposeCraft_UI_API_Prototype_Test.Game.Control.GameActions
{
	/// <summary>
	/// Action of attack to a unit
	/// </summary>
	class Attack : GameAction
	{
		IActor Target;

		public Attack(IActor target)
		{
			Target = target;
		}
	}
}
