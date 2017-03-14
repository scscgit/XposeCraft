using System.Collections.Generic;
using XposeCraft.Game.Actors;

namespace XposeCraft.Game.Helpers
{
    abstract class ActorHelper<TForActorHelper> where TForActorHelper : IActor
    {
        public delegate void ForEachAction<in TActor>(TActor unit);

        private static readonly object ForEachLock = new object();

        protected static void ForEach<TActor, TFromActor>(ForEachAction<TActor> action, IList<TFromActor> from)
            where TActor : TForActorHelper
            where TFromActor : TForActorHelper
        {
            lock (ForEachLock)
            {
                foreach (TFromActor unit in from)
                {
                    if (unit is TActor)
                    {
                        // What the hell, http://stackoverflow.com/questions/4092393/value-of-type-t-cannot-be-converted-to
                        action((TActor) (object) unit);
                    }
                }
            }
        }
    }
}
