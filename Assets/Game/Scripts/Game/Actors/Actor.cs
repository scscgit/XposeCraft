using UnityEngine;

namespace XposeCraft.Game.Actors
{
    /// <summary>
    /// Representation of a Game Actor in the Unity.
    /// Remember to mark all implementation subclasses [Serializable] in order for hotswap to work.
    /// </summary>
    public abstract class Actor : ScriptableObject, IActor
    {
        protected GameObject GameObject { get; private set; }

        public Position Position
        {
            get
            {
                // TODO: create new position based on GridPoint
                return null;
            }
        }

        /// <summary>
        /// The only safe way to create an Actor. Constructor usage is not advised for ScriptableObject.
        /// Source: http://answers.unity3d.com/questions/310847/how-to-create-instance-of-scriptableobject-and-pas.html
        /// </summary>
        /// <param name="gameObject">Actor representation as a GameObject</param>
        /// <typeparam name="T">Subclass type</typeparam>
        /// <returns></returns>
        public static T Create<T>(GameObject gameObject) where T : Actor
        {
            var instance = CreateInstance<T>();
            instance.GameObject = gameObject;
            return instance;
        }

        protected Actor(GameObject gameObject)
        {
            GameObject = gameObject;
        }
    }
}
