using System;
using System.Collections.Generic;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Game.Actors.Resources;
using XposeCraft.Game.Actors.Units;
using XposeCraft.GameInternal;

namespace XposeCraft.Game.Control.GameActions
{
    public class GatherResource : GameAction
    {
        public IResource Gathering { get; private set; }

        public GatherResource(IResource resource)
        {
            Gathering = resource;
        }

        public override bool Progress(IUnit unit, UnitController unitController)
        {
            if (!base.Progress(unit, unitController))
            {
                return false;
            }
            var worker = unit as Worker;
            if (worker == null)
            {
                throw new Exception(typeof(GatherResource) + " Action was invoked on non-Worker unit, this is invalid");
            }
            worker.Gathering = Gathering;
            Gathering.GatherByWorker(new List<UnitController> {unitController});
            return true;
        }

        public override void OnFinish(IUnit unit, UnitController unitController)
        {
            base.OnFinish(unit, unitController);
            ((Worker) unit).Gathering = null;
            Log.d(unit + " is no longer gathering");
        }
    }
}
