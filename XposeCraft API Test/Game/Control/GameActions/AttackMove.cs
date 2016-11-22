using System;
using System.Collections.Generic;

namespace XposeCraft.Game.Control.GameActions
{
	/// <summary>
	/// Action of a movement to a new position,
	/// during which the unit attacks any enemies before it continues
	/// </summary>
	class AttackMove : GameAction
	{
		Position Where;

		public AttackMove(Position where)
		{
			Where = where;
		}
	}
}
