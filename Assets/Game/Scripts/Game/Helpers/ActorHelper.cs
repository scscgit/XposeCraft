using System.Collections.Generic;
using XposeCraft.Game.Actors;

namespace XposeCraft.Game.Helpers
{
    abstract class ActorHelper<TForActor> where TForActor : IActor
    {
        public delegate void ForEachAction<TActor>(TActor unit);

        private static readonly object ForEachLock = new object();

        protected static void ForEach<ActorType>(ForEachAction<ActorType> action, IList<TForActor> from)
            where ActorType : TForActor
        {
            lock (ForEachLock)
            {
                foreach (TForActor unit in from)
                {
                    if (unit is ActorType)
                    {
                        action((ActorType) unit);
                    }
                }
            }
        }
    }
}
