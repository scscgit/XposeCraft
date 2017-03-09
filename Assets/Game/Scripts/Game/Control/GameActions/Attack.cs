using System;
using System.Collections.Generic;
using XposeCraft.Game.Actors;

namespace XposeCraft.Game.Control.GameActions
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
