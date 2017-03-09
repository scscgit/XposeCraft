using System;
using System.Collections.Generic;
using XposeCraft.Game.Enums;
using XposeCraft.GameInternal;

namespace XposeCraft.Game
{
	public class Event
	{
		public bool IsRegistered
		{
			get; private set;
		} = false;
		public EventType GameEvent { get; private set; }
		public FunctionWithArguments Function { get; private set; }

		Event(EventType gameEvent, FunctionWithArguments function)
		{
			this.GameEvent = gameEvent;
			this.Function = function;
		}

		public delegate void FunctionWithArguments(Arguments args);

		public static Event Register(EventType gameEvent, FunctionWithArguments function)
		{
			var newEvent = new Event(gameEvent, function);
			newEvent.IsRegistered = true;
			RegisteredEvents.Registered[gameEvent].Add(newEvent);
			return newEvent;
		}

		public void UnregisterEvent()
		{
			if (IsRegistered)
			{
				IsRegistered = false;
				RegisteredEvents.Registered[GameEvent].Remove(this);
			}
		}
	}
}
