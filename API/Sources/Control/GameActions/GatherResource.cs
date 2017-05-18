using System;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Resources;
using XposeCraft.Game.Actors.Units;

namespace XposeCraft.Game.Control.GameActions
{
    /// <summary>
    /// Gathers a chosen Resource and returns back to the <see cref="BaseCenter"/> with collected Resources, which are
    /// subsequenty available for spending by Player anywhere. Only <see cref="Worker"/> units can do this Action.
    /// Throws <see cref="ResourceExhaustedException"/> if the target resource is no longer available.
    /// </summary>
    public class GatherResource : GameAction
    {
        /// <summary>
        /// Gathering target Resource.
        /// </summary>
        public IResource Gathering { get; private set; }

        /// <inheritdoc cref="GatherResource"/>
        /// <param name="resource">Resource to be gathered.</param>
        /// <exception cref="ResourceExhaustedException">The Resource was already exhausted before the attempt to go gather it.</exception>
        public GatherResource(IResource resource)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="GatherResource"/>
        /// <param name="resource">Resource to be gathered.</param>
        /// <param name="exceptionIfExhausted">If false, won't throw the <see cref="ResourceExhaustedException"/></param>
        /// <exception cref="ResourceExhaustedException">The Resource was already exhausted before the attempt to go gather it.</exception>
        public GatherResource(IResource resource, bool exceptionIfExhausted)
        {
            throw new NotImplementedException();
        }
    }
}
