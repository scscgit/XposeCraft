using System;

namespace XposeCraft.Game.Actors.Buildings
{
    public abstract class Building : Actor, IBuilding
    {
        public bool Finished
        {
            get { throw new NotImplementedException(); }
        }

        public float ConstructionProgress
        {
            get { throw new NotImplementedException(); }
        }
    }
}
