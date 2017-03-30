using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityTest;
using UnityTest.IntegrationTestRunner;

namespace XposeCraft.UnityWorkarounds
{
    /// <summary>
    /// Unity cannot build a project with specific scenes directly from the console without a helper method like this.
    /// </summary>
    public class BuildProject : MonoBehaviour
    {
        private class TestCallback : ITestRunnerCallback
        {
            public void RunStarted(string platform, List<TestComponent> testsToRun)
            {
                print("*** Run started ***");
            }

            public void RunFinished(List<TestResult> testResults)
            {
                print("*** Run finished ***");
                _stopTests = true;
            }

            public void AllScenesFinished()
            {
                print("*** All scenes finished ***");
            }

            public void TestStarted(TestResult test)
            {
                print("*** Test started ***");
            }

            public void TestFinished(TestResult test)
            {
                print("*** Test finished ***");
            }

            public void TestRunInterrupted(List<ITestComponent> testsNotRun)
            {
                print("*** Test run interrupted ***");
                _stopTests = true;
            }
        }

        private static readonly string[] BuildScenes =
        {
            "Assets/Game/Scenes/BasicScene.unity"
        };

        private static readonly string[] TestScenes =
        {
            BuildScenes[0],
            "Assets/Game/Scenes/AutomationTest.unity"
        };

        private static bool _stopTests;

        public static void Build()
        {
            var buildError = BuildPipeline.BuildPlayer(BuildScenes, GetLocation(), GetTarget(), GetOptions());
            if (!string.IsNullOrEmpty(buildError))
            {
                Debug.LogError(buildError);
            }
        }

        /// <summary>
        /// Run tests after combining the scenes.
        /// Currently not working.
        /// </summary>
        [Obsolete]
        public static void Test()
        {
            foreach (string scene in TestScenes)
            {
                EditorSceneManager.OpenScene(scene, OpenSceneMode.Additive);
            }
            var testRunner = TestRunner.GetTestRunner();
            testRunner.TestRunnerCallback.Add(new TestCallback());
            print("*** Start of testing ***");
            Batch.RunIntegrationTests(null, TestScenes.ToList(), new List<string>());

            while (!_stopTests)
            {
                System.Threading.Thread.Sleep(5000);
                print(_stopTests ? "*** End of testing ***" : "*** Tests are still running ***");
            }
        }

        static string GetLocation()
        {
            var location = Environment.GetEnvironmentVariable("location");
            if (string.IsNullOrEmpty(location))
            {
                throw new ArgumentException(
                    "Wrong content of environment value \"location\", please define a location for build result");
            }
            return location;
        }

        static BuildTarget GetTarget()
        {
            BuildTarget target;
            switch (Environment.GetEnvironmentVariable("target").ToLower())
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
                    throw new ArgumentException(
                        "Wrong content of environment value \"target\", please define a build target platform");
            }
            return target;
        }

        static BuildOptions GetOptions()
        {
            BuildOptions options;
            switch (Environment.GetEnvironmentVariable("options").ToLower())
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
