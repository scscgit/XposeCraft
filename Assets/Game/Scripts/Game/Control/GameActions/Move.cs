using System;
using System.Collections.Generic;

namespace XposeCraft.Game.Control.GameActions
{
	/// <summary>
	/// Action of a movement to a new position
	/// </summary>
	class Move : GameAction
	{
		Position Where;

		public Move(Position where)
		{
			Where = where;
		}
	}
}
