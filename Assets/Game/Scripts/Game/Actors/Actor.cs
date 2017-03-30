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
            get
            {
                var uGrid = GameManager.Instance.UGrid;
                return new Position(uGrid.DetermineLoc(GameObject.transform.position, uGrid.index));
            }
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
            var instance = CreateInstance<T>();
            instance.GameObject = gameObject;
            instance.Initialize();
            return instance;
        }

        /// <summary>
        /// The only safe way to create an Actor is to add him to a GameObject.
        /// </summary>
        /// <param name="gameObject">Actor representation as a GameObject, null if it has to be created</param>
        /// <typeparam name="T">Subclass type</typeparam>
        /// <returns>Attached instance of the Actor</returns>
        [Obsolete]
        public static T CreateAsMonoBehaviour<T>(GameObject gameObject) where T : Actor
        {
            // Alternative to be used when if the Actor is MonoBehaviour instead of ScriptableObject
            throw new InvalidOperationException();
//            if (gameObject == null)
//            {
//                gameObject = new GameObject {name = typeof(T).Name};
//            }
//            var instance = gameObject.AddComponent<T>();
//            instance.GameObject = gameObject;
//            return instance;
        }
    }
}
