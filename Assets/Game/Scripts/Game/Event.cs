using XposeCraft.Game.Enums;
using XposeCraft.GameInternal;

namespace XposeCraft.Game
{
    public class Event
    {
        public bool IsRegistered { get; private set; }
        public EventType GameEvent { get; private set; }
        public FunctionWithArguments Function { get; private set; }

        Event(EventType gameEvent, FunctionWithArguments function)
        {
            GameEvent = gameEvent;
            Function = function;
        }

        public delegate void FunctionWithArguments(Arguments args);

        public static Event Register(EventType gameEvent, FunctionWithArguments function)
        {
            var newEvent = new Event(gameEvent, function) {IsRegistered = true};
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
