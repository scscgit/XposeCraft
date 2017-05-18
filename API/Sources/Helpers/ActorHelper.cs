using XposeCraft.Game.Actors;

namespace XposeCraft.Game.Helpers
{
    /// <summary>
    /// Provides easy access for operations, that may be complicated or impossible by using direct API of other classes.
    /// </summary>
    /// <typeparam name="TForActorHelper">Type of Actors for which the Helper provides functions.</typeparam>
    public abstract class ActorHelper<TForActorHelper> where TForActorHelper : IActor
    {
    }
}
