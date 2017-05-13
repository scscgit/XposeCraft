using UnityEditor;
using UnityEngine;
using XposeCraft.Core.Faction;
using XposeCraft.Core.Fog_Of_War;
using XposeCraft.UnityWorkarounds;

namespace XposeCraft
{
    /// <summary>
    /// Player's interface with the Game.
    /// </summary>
    public class MenuItems
    {
        /// <summary>
        /// Opens scenes required for Player testing.
        /// </summary>
        [MenuItem("XposeCraft/Open Game Scenes")]
        public static void OpenScenes()
        {
            BuildProject.OpenScenes(BuildProject.TestScenes);
        }

        /// <summary>
        /// Run tests after combining and opening the scenes.
        /// </summary>
        [MenuItem("XposeCraft/Run Test")]
        public static void Test()
        {
            BuildProject.Test();
        }

        /// <summary>
        /// Switches the Fog to display another Player.
        /// </summary>
        [MenuItem("XposeCraft/Switch Fog Player")]
        public static void SwitchFogPlayer()
        {
            var fog = GameObject.Find("Fog").GetComponent<Fog>();
            var factionManager = GameObject.Find("Faction Manager").GetComponent<FactionManager>();

            fog.FactionIndexDisplay = (fog.FactionIndexDisplay + 1) % factionManager.FactionList.Length;
        }
    }
}
