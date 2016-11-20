using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.GameInternal;

namespace XposeCraft_UI_API_Prototype_Test.Game
{
	class Event
	{
		bool IsRegistered;
		public Events GameEvent { get; private set; }
		public FunctionWithArguments Function { get; private set; }

		Event(Events chosenEvent, FunctionWithArguments function)
		{
			this.Function = function;
		}

		public delegate void FunctionWithArguments(Arguments args);

		public static Event Register(Events gameEvent, FunctionWithArguments function)
		{
			var newEvent = new Event(gameEvent, function);
			newEvent.IsRegistered = true;
			RegisteredEvents.Registered[gameEvent].Add(newEvent);
			return newEvent;
		}

		public void UnregisterEvent()
		{
			IsRegistered = false;
			RegisteredEvents.Registered[GameEvent].Remove(this);
		}
	}
}
