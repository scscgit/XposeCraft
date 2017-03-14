using UnityEditor;
using UnityEngine;

namespace XposeCraft.UnityWorkarounds
{
    /// <summary>
    /// Unity cannot build a project with specific scenes directly from the console without a helper method like this.
    /// </summary>
    public class BuildProject : MonoBehaviour
    {
        public static void Build()
        {
            string[] scenes = {"Assets/Game/Scenes/BasicScene.unity"};

            BuildPipeline.BuildPlayer(scenes, GetLocation(), GetTarget(), GetOptions());
        }

        static string GetLocation()
        {
            var location = System.Environment.GetEnvironmentVariable("location");
            if (string.IsNullOrEmpty(location))
            {
                throw new System.ArgumentException(
                    "Wrong content of environment value \"location\", please define a location for build result");
            }
            return location;
        }

        static BuildTarget GetTarget()
        {
            BuildTarget target;
            switch (System.Environment.GetEnvironmentVariable("target").ToLower())
            {
                case "windows":
                case "win":
                case "windows32":
                case "win32":
                    target = BuildTarget.StandaloneWindows;
                    break;
                case "windows64":
                case "win64":
                    target = BuildTarget.StandaloneWindows64;
                    break;
                case "linux":
                case "linux32":
                    target = BuildTarget.StandaloneLinux;
                    break;
                case "linux64":
                    target = BuildTarget.StandaloneLinux64;
                    break;
                case "osx":
                case "mac":
                case "osx32":
                case "mac32":
                    target = BuildTarget.StandaloneOSXIntel;
                    break;
                case "osx64":
                case "mac64":
                    target = BuildTarget.StandaloneOSXIntel64;
                    break;
                case "osxuniversal":
                case "macuniversal":
                    target = BuildTarget.StandaloneOSXUniversal;
                    break;

                default:
                    throw new System.ArgumentException(
                        "Wrong content of environment value \"target\", please define a build target platform");
            }
            return target;
        }

        static BuildOptions GetOptions()
        {
            BuildOptions options;
            switch (System.Environment.GetEnvironmentVariable("options").ToLower())
            {
                case "development":
                case "dev":
                    options = BuildOptions.Development;
                    break;

                default:
                    options = BuildOptions.None;
                    break;
            }
            return options;
        }
    }
}
