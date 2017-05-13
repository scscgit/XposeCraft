using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XposeCraft.Core.Required;
using XposeCraft.Core.Resources;

namespace XposeCraft.Core.Faction
{
    [RequireComponent(typeof(ResourceManager))]
    public class Faction : MonoBehaviour
    {
        [FormerlySerializedAs("relations")] public Relation[] Relations = new Relation[0];
        [FormerlySerializedAs("gui")] public FGUI Gui;
        [FormerlySerializedAs("unitList")] public Unit[] UnitList = new Unit[0];
        [FormerlySerializedAs("buildingList")] public Building[] BuildingList = new Building[0];
        [FormerlySerializedAs("tech")] public Technology[] Tech = new Technology[0];
        [FormerlySerializedAs("color")] public Color Color;

        public int[] EnemyFactionIndexes()
        {
            var enemies = new List<int>();
            for (var factionIndex = 0; factionIndex < Relations.Length; factionIndex++)
            {
                if (Relations[factionIndex].state == 2)
                {
                    enemies.Add(factionIndex);
                }
            }
            return enemies.ToArray();
        }
    }
}
