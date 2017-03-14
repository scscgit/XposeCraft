using UnityEngine;

namespace XposeCraft.Game.Actors.Buildings
{
    public abstract class Building : Actor, IBuilding
    {
        public bool IsFinished { get; set; }

        protected Building(GameObject gameObject) : base(gameObject)
        {
        }
    }
}
