using System;
using System.Collections.Generic;
using UnityEngine;
using XposeCraft.Collections;
using XposeCraft.Core.Faction;
using XposeCraft.Game;
using XposeCraft.Game.Actors;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Resources;
using XposeCraft.Game.Actors.Units;
using XposeCraft.Game.Enums;
using Event = XposeCraft.Game.Event;
using EventType = XposeCraft.Game.Enums.EventType;
using VisionState = XposeCraft.Core.Fog_Of_War.VisionReceiver.VisionState;

namespace XposeCraft.GameInternal
{
    /// <summary>
    /// Data structures that make up the Model of a Player used within a game.
    /// It is a good idea not to persist it as a prefab, as the instance will usually hold references to Scene objects.
    /// </summary>
    public class Player : MonoBehaviour
    {
        // Unity hot-swap serialization workarounds.

        [Serializable]
        public class EventList : List<Event>
        {
        }

        [Serializable]
        public class RegisteredEventsDictionary : SerializableDictionary2<EventType, EventList>
        {
        }

        /// <summary>
        /// Curently executing Player to be used as a Model.
        /// Needs to be replaced before executing any Event.
        /// </summary>
        public static Player CurrentPlayer;

        /// <summary>
        /// Faction of the Player, allied Players will share it.
        /// </summary>
        public Faction Faction;

        public int FactionIndex;

        /// <summary>
        /// This Player's Base.
        /// </summary>
        public PlaceType MyBase;

        /// <summary>
        /// This Player's Enemy's Base.
        /// </summary>
        public PlaceType EnemyBase;

        /// <summary>
        /// Actions hooked to Events that can, but don't have to be, run at any time.
        /// </summary>
        public RegisteredEventsDictionary RegisteredEvents;

        // In-game Actors available to the Player.

        public List<Unit> Units;
        public List<Building> Buildings;
        public List<Resource> Resources;

        public List<Unit> EnemyVisibleUnits;
        public List<Building> EnemyVisibleBuildings;

        // Currencies of the Player. (TODO)

        public int Minerals = 80;

        // Run-time configuration of the Player

        private bool _exceptionOnDeadUnitAction = true;

        public bool ExceptionOnDeadUnitAction
        {
            get { return _exceptionOnDeadUnitAction; }
            set { _exceptionOnDeadUnitAction = value; }
        }

        public void EnemyVisibilityChanged(Actor actor, VisionState previousState, VisionState newState)
        {
            if (previousState == VisionState.Vision && newState != VisionState.Vision)
            {
                EnemyHidden(actor, newState);
            }
            else if (previousState == VisionState.Undiscovered || previousState == VisionState.Discovered)
            {
                EnemyTurnedToVisible(actor);
            }
        }

        private void EnemyTurnedToVisible(Actor actor)
        {
            var unit = actor as IUnit;
            if (unit != null)
            {
                GameManager.Instance.FiredEvent(
                    this,
                    EventType.EnemyUnitsOnSight,
                    new Arguments
                    {
                        EnemyUnits = new[] {unit}
                    });
                EnemyVisibleUnits.Add((Unit) unit);
                return;
            }
            var building = actor as IBuilding;
            if (building != null)
            {
                GameManager.Instance.FiredEvent(
                    this,
                    EventType.EnemyBuildingsOnSight,
                    new Arguments
                    {
                        EnemyBuildings = new[] {building}
                    });
                EnemyVisibleBuildings.Add((Building) building);
            }
        }

        private void EnemyHidden(Actor actor, VisionState newState)
        {
            var unit = actor as IUnit;
            if (unit != null)
            {
                EnemyVisibleUnits.Remove((Unit) unit);
                return;
            }
            var building = actor as IBuilding;
            if (building != null && newState == VisionState.Undiscovered)
            {
                EnemyVisibleBuildings.Remove((Building) building);
            }
        }
    }
}
