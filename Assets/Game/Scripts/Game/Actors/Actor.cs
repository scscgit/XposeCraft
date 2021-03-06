using System;
using UnityEngine;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Units;
using XposeCraft.Game.Enums;
using XposeCraft.GameInternal;

namespace XposeCraft.Game.Actors
{
    /// <inheritdoc cref="IActor"/>
    public abstract class Actor : ScriptableObject, IActor
    {
        [SerializeField] private GameObject _gameObject;

        protected GameObject GameObject
        {
            get { return _gameObject; }
            set { _gameObject = value; }
        }

        public Position Position
        {
            get { return new Position(GameManager.Instance.UGrid.DetermineLocation(GameObject.transform.position)); }
        }

        public abstract OwnershipType Ownership { get; }

        public abstract bool Visible { get; }

        protected virtual void Initialize(Player playerOwner)
        {
        }

        /// <summary>
        /// Internal method, do not use directly.
        /// The only safe way to create an Actor. Constructor usage is not advised for ScriptableObject.
        /// Source: http://answers.unity3d.com/questions/310847/how-to-create-instance-of-scriptableobject-and-pas.html
        /// </summary>
        /// <param name="gameObject">Actor representation as a GameObject in the Scene.</param>
        /// <param name="playerOwner">Player that owns the Unit and should be used as its context.</param>
        /// <typeparam name="T">Subclass type.</typeparam>
        /// <returns>New instance of an Actor.</returns>
        internal static T Create<T>(GameObject gameObject, Player playerOwner) where T : Actor
        {
            return Create<T>(typeof(T), gameObject, playerOwner);
        }

        /// <summary>
        /// Internal method, do not use directly.
        /// </summary>
        internal static T Create<T>(Type type, GameObject gameObject, Player playerOwner) where T : Actor
        {
            if (!typeof(T).IsAssignableFrom(type))
            {
                throw new InvalidOperationException("T does not match type parameter");
            }
            var instance = (T) CreateInstance(type);
            if (gameObject != null && playerOwner != null)
            {
                // Player signs the name of Game Object for cosmetic purposes
                gameObject.name = string.Format("{0} {1}", gameObject.name, playerOwner.name);
            }
            instance.GameObject = gameObject;
            instance.Initialize(playerOwner);
            var building = instance as Building;
            if (building != null)
            {
                playerOwner.Buildings.Add(building);
            }
            else
            {
                var unit = instance as Unit;
                if (unit != null)
                {
                    playerOwner.Units.Add(unit);
                }
            }
            if (gameObject != null)
            {
                // Storing the GameObject in a map in order to be able to find its representative Actor
                GameManager.Instance.ActorLookup.Add(gameObject, instance);
            }
            return instance;
        }
    }
}
