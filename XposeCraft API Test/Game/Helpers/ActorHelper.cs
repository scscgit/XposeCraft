using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game.Actors;

namespace XposeCraft_UI_API_Prototype_Test.Game.Helpers
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
				foreach (IActor unit in from)
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
