using System;
using System.Collections.Generic;
using XposeCraft.Game.Control.GameActions;

namespace XposeCraft.Game.Control
{
	public class UnitActionQueue
	{
		Queue<IGameAction> Queue = new Queue<IGameAction>();

		public UnitActionQueue()
		{
		}

		public UnitActionQueue(IGameAction action)
		{
			After(action);
		}

		public UnitActionQueue After(IGameAction action)
		{
			Queue.Enqueue(action);
			return this;
		}
	}
}
