using System;
using System.Collections.Generic;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Resources;
using XposeCraft.Game.Actors.Units;
using XposeCraft.GameInternal;

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

        private bool _exceptionIfExhausted;
        private bool _cancelled;

        /// <inheritdoc cref="GatherResource"/>
        /// <param name="resource">Resource to be gathered.</param>
        /// <exception cref="ResourceExhaustedException">The Resource was already exhausted before the attempt to go gather it.</exception>
        public GatherResource(IResource resource) : this(resource, true)
        {
        }

        /// <inheritdoc cref="GatherResource"/>
        /// <param name="resource">Resource to be gathered.</param>
        /// <param name="exceptionIfExhausted">If false, won't throw the <see cref="ResourceExhaustedException"/></param>
        /// <exception cref="ResourceExhaustedException">The Resource was already exhausted before the attempt to go gather it.</exception>
        public GatherResource(IResource resource, bool exceptionIfExhausted)
        {
            _exceptionIfExhausted = exceptionIfExhausted;
            Gathering = resource;
        }

        internal override bool Progress(IUnit unit, UnitController unitController)
        {
            if (!base.Progress(unit, unitController))
            {
                return false;
            }
            var worker = unit as Worker;
            if (worker == null)
            {
                throw new Exception(
                    typeof(GatherResource).Name + " Action was invoked on non-Worker unit, this is invalid");
            }
            worker.Gathering = Gathering;
            try
            {
                ((Resource) Gathering).GatherByWorker(new List<UnitController> {unitController});
            }
            catch (ResourceExhaustedException)
            {
                if (_exceptionIfExhausted)
                {
                    throw;
                }
                _cancelled = true;
            }
            return true;
        }

        internal override void OnFinish(IUnit unit, UnitController unitController)
        {
            if (_cancelled)
            {
                return;
            }
            base.OnFinish(unit, unitController);
            ((Worker) unit).Gathering = null;
            Log.d(unit + " is no longer gathering");
        }
    }
}
