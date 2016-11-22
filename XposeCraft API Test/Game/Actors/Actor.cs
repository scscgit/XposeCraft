using System;
using System.Collections.Generic;

namespace XposeCraft.Game.Actors
{
	public abstract class Actor : IActor
	{
		protected Actor(Position position)
		{
			this.Position = position;
		}

		public Position Position
		{
			get; protected set;
		}
	}
}
