using System;
using System.Collections.Generic;
using XposeCraft.Game.Actors;

namespace XposeCraft.Game.Helpers
{
	abstract class ActorHelper<ForActorType> where ForActorType : IActor
	{
		public delegate void ForEachAction<ActorType>(ActorType unit);

		private static readonly object ForEachLock = new object();

		protected static void ForEach<ActorType>
			(ForEachAction<ActorType> action, IList<ForActorType> from)
			where ActorType : ForActorType
		{
			lock (ForEachLock)
			{
				foreach (ForActorType unit in from)
				{
					if (unit is ActorType)
					{
						action((ActorType)unit);
					}
				}
			}
		}
	}
}
