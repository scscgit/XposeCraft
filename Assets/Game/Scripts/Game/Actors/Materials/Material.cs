using UnityEngine;

namespace XposeCraft.Game.Actors.Materials
{
    public abstract class Material : Actor, IMaterial
    {
        protected Material(GameObject gameObject) : base(gameObject)
        {
        }
    }
}
