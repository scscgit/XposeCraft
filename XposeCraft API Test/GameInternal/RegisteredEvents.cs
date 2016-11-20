using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game;

namespace XposeCraft_UI_API_Prototype_Test.GameInternal
{
	class RegisteredEvents
	{
		// TODO: static constructor (for now in tester)
		public static void Initialize()
		{
			foreach (Events gameEvent in Registered.Keys)
			{
				Registered.Add(gameEvent, new List<Event>());
			}
		}

		public static IDictionary<Events, IList<Event>> Registered = new Dictionary<Events, IList<Event>>();
	}
}
