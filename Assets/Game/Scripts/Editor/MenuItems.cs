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
        /// Opens game scenes required for Player testing.
        /// </summary>
        [MenuItem("XposeCraft/Open Game")]
        public static void OpenScenes()
        {
            BuildProject.OpenScenes(BuildProject.BuildScenes);
        }

        /// <summary>
        /// Run tests after combining and opening the scenes.
        /// </summary>
        [MenuItem("XposeCraft/Run as an Integration Test")]
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
            if (!EditorApplication.isPlaying)
            {
                // Player did not understand the meaning of this button, it's not meant to be used outside the game
                EditorApplication.isPlaying = true;
            }
            var fog = GameObject.Find("Fog").GetComponent<Fog>();
            var factionManager = GameObject.Find("Faction Manager").GetComponent<FactionManager>();
            fog.FactionIndexDisplay = (fog.FactionIndexDisplay + 1) % factionManager.FactionList.Length;
        }
    }
}
