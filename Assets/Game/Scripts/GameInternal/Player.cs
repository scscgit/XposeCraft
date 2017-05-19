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
using VisionState = XposeCraft.Core.Fog_Of_War.VisionReceiver.VisionState;

namespace XposeCraft.GameInternal
{
    /// <summary>
    /// Data structures that make up the Model of a Player used within a game.
    /// It is a good idea not to persist it as a prefab, as the instance will usually hold references to Scene objects.
    /// </summary>
    public class Player : MonoBehaviour
    {
        /// <summary>
        /// Reason of losing.
        /// </summary>
        public enum LoseReason
        {
            ExceptionThrown,
            AllBuildingsDestroyed,
            TimeoutStalemate
        }

        // Unity hot-swap serialization workarounds.

        [Serializable]
        public class EventList : List<GameEvent>
        {
        }

        [Serializable]
        public class RegisteredEventsDictionary : SerializableDictionary2<GameEventType, EventList>
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

        // Run-time configuration of the Player

        private bool _exceptionOnDeadUnitAction;

        public bool ExceptionOnDeadUnitAction
        {
            get { return _exceptionOnDeadUnitAction; }
            set { _exceptionOnDeadUnitAction = value; }
        }

        public void EnemyVisibilityChanged(
            Actor enemyActor, List<Actor> actorsSawChange, VisionState previousState, VisionState newState)
        {
            if (previousState == VisionState.Vision && newState != VisionState.Vision)
            {
                EnemyHidden(enemyActor, newState);
            }
            else if (previousState == VisionState.Undiscovered || previousState == VisionState.Discovered)
            {
                EnemyTurnedToVisible(enemyActor, actorsSawChange);
            }
        }

        private void EnemyTurnedToVisible(Actor enemyActor, List<Actor> myActorsSaw)
        {
            // Only the first detection matters, as it is about visibility state "change"
            var myActor = myActorsSaw.Count == 0 ? null : myActorsSaw[0];
            var enemyUnit = enemyActor as IUnit;
            if (enemyUnit != null)
            {
                GameManager.Instance.FiredEvent(
                    this,
                    GameEventType.EnemyUnitsOnSight,
                    new Arguments
                    {
                        MyUnit = myActor as IUnit,
                        MyBuilding = myActor as IBuilding,
                        EnemyUnits = new[] {enemyUnit}
                    });
                EnemyVisibleUnits.Add((Unit) enemyUnit);
                return;
            }
            var enemyBuilding = enemyActor as IBuilding;
            if (enemyBuilding != null)
            {
                GameManager.Instance.FiredEvent(
                    this,
                    GameEventType.EnemyBuildingsOnSight,
                    new Arguments
                    {
                        MyUnit = myActor as IUnit,
                        MyBuilding = myActor as IBuilding,
                        EnemyBuildings = new[] {enemyBuilding}
                    });
                EnemyVisibleBuildings.Add((Building) enemyBuilding);
            }
        }

        private void EnemyHidden(Actor enemyActor, VisionState newState)
        {
            var unit = enemyActor as IUnit;
            if (unit != null)
            {
                EnemyVisibleUnits.Remove((Unit) unit);
                return;
            }
            var building = enemyActor as IBuilding;
            if (building != null && newState == VisionState.Undiscovered)
            {
                EnemyVisibleBuildings.Remove((Building) building);
            }
        }

        public void Win()
        {
            if (this == GameManager.Instance.GuiPlayer)
            {
                GameTestRunner.Passed = true;
            }
        }

        public void Lost(LoseReason loseReason)
        {
            switch (loseReason)
            {
                case LoseReason.ExceptionThrown:
                    Log.e(string.Format("Player {0} lost because an exception was thrown", name));
                    break;
                case LoseReason.AllBuildingsDestroyed:
                    Log.e(string.Format("Player {0} lost because all his buildings were destroyed", name));
                    break;
                case LoseReason.TimeoutStalemate:
                    Log.e(string.Format("Stalemate. Player {0} lost because of a game timeout", name));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("loseReason", loseReason, null);
            }
            if (this == GameManager.Instance.GuiPlayer)
            {
                GameTestRunner.Failed = true;
            }
            // TODO: this causes enemies to Win (if no remaining players and not stalemate)
        }

        public void Lost(Exception exception)
        {
            Log.e(exception);
            Lost(LoseReason.ExceptionThrown);
        }
    }
}
