using System.Collections.Generic;
using XposeCraft.Game.Actors;

namespace XposeCraft.Game.Helpers
{
    /// <summary>
    /// Provides easy access for operations, that may be complicated or impossible by using direct API of other classes.
    /// </summary>
    /// <typeparam name="TForActorHelper">Type of Actors for which the Helper provides functions.</typeparam>
    public abstract class ActorHelper<TForActorHelper> where TForActorHelper : IActor
    {
        protected delegate void ForEachAction<in TActor>(TActor unit);

        private static readonly object ForEachLock = new object();

        protected static void ForEach<TActor, TFromActor>(ForEachAction<TActor> action, IList<TFromActor> from)
            where TActor : TForActorHelper
            where TFromActor : TForActorHelper
        {
            lock (ForEachLock)
            {
                foreach (var unit in from)
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
