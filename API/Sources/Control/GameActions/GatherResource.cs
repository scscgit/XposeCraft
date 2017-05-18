using System;
using XposeCraft.Game.Actors.Resources;

namespace XposeCraft.Game.Control.GameActions
{
    public class GatherResource : GameAction
    {
        public IResource Gathering { get; private set; }

        public GatherResource(IResource resource)
        {
            throw new NotImplementedException();
        }
    }
}
