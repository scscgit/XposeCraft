﻿using System;
using System.Collections.Generic;
using XposeCraft.Collections;
using XposeCraft.Game;
using XposeCraft.Game.Enums;

namespace XposeCraft.GameInternal
{
    /// <summary>
    /// Data structures that make up the Model of a Player used within a game.
    /// </summary>
    [Serializable]
    public class Player
    {
        /// <summary>
        /// Unity hot-swap serialization workaround.
        /// </summary>
        [Serializable]
        public class EventList : List<Event>
        {
        }

        /// <summary>
        /// Unity hot-swap serialization workaround.
        /// </summary>
        [Serializable]
        public class RegisteredEventsDictionary : SerializableDictionary<EventType, EventList>
        {
        }

        /// <summary>
        /// Curently executing Player to be used as a Model.
        /// Needs to be replaced before executing any Event.
        /// </summary>
        public static Player CurrentPlayer;

        /// <summary>
        /// Actions hooked to Events that can, but don't have to be, run at any time.
        /// </summary>
        public RegisteredEventsDictionary RegisteredEvents;

        // In-game Actors available to the Player.

        public List<Game.Actors.Units.Unit> Units;
        public List<Game.Actors.Buildings.Building> Buildings;
        public List<Game.Actors.Materials.Material> Materials;

        // Currencies of the Player.

        public int Minerals = 80;
    }
}