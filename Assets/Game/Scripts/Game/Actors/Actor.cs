using System;
using UnityEngine;
using XposeCraft.GameInternal;

namespace XposeCraft.Game.Actors
{
    /// <summary>
    /// Representation of a Game Actor in the Unity.
    /// Remember to mark all implementation subclasses [Serializable] in order for hotswap to work.
    /// </summary>
    public abstract class Actor : ScriptableObject, IActor
    {
        [SerializeField] private GameObject _gameObject;

        protected GameObject GameObject
        {
            get { return _gameObject; }
            private set { _gameObject = value; }
        }

        public Position Position
        {
            get { return new Position(GameManager.Instance.UGrid.DetermineLoc(GameObject.transform.position)); }
        }

        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// The only safe way to create an Actor. Constructor usage is not advised for ScriptableObject.
        /// Source: http://answers.unity3d.com/questions/310847/how-to-create-instance-of-scriptableobject-and-pas.html
        /// </summary>
        /// <param name="gameObject">Actor representation as a GameObject</param>
        /// <typeparam name="T">Subclass type</typeparam>
        /// <returns>New instance of an Actor</returns>
        public static T Create<T>(GameObject gameObject) where T : Actor
        {
            return Create<T>(typeof(T), gameObject);
        }

        public static T Create<T>(Type type, GameObject gameObject) where T : Actor
        {
            if (!typeof(T).IsAssignableFrom(type))
            {
                throw new InvalidOperationException("T does not match type parameter");
            }
            var instance = (T) CreateInstance(type);
            instance.GameObject = gameObject;
            instance.Initialize();
            return instance;
        }
    }
}
