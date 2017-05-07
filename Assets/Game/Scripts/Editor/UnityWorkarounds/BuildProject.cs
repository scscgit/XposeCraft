using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityTest;
using UnityTest.IntegrationTestRunner;
using XposeCraft.GameInternal;

namespace XposeCraft.UnityWorkarounds
{
    /// <summary>
    /// Unity cannot build a project with specific scenes directly from the console without a helper method like this.
    /// Putting [InitializeOnLoad] over the script makes the subscriptions (playmodeStateChanged) last through playmode.
    /// </summary>
    [InitializeOnLoad]
    public class BuildProject : MonoBehaviour
    {
        private class TestCallback : ITestRunnerCallback
        {
            public void RunStarted(string platform, List<TestComponent> testsToRun)
            {
                print("*** Run of " + testsToRun.Count + " tests started ***");
            }

            public void RunFinished(List<TestResult> testResults)
            {
                print("*** Run of " + testResults.Count + " tests finished ***");
                _stopTests = true;
            }

            public void AllScenesFinished()
            {
                print("*** All scenes finished ***");
            }

            public void TestStarted(TestResult test)
            {
                print("*** Test " + test.Name + " started ***");
            }

            public void TestFinished(TestResult test)
            {
                print("*** Test " + test.Name + " finished ***");
            }

            public void TestRunInterrupted(List<ITestComponent> testsNotRun)
            {
                print("*** Test run interrupted, tests not run: ***");
                testsNotRun.ForEach(test => print(test.Name));
                _stopTests = true;
            }
        }

        static BuildProject()
        {
            // Static constructor, [InitializeOnLoad] and a persisted variable can initialize the Test after Play start
            if (PlayerPrefs.GetInt("testAfterPlayingReady", 0) == 0)
            {
                return;
            }
            //EditorApplication.playmodeStateChanged += TestAfterPlaying;
            TestAfterPlaying();
            PlayerPrefs.SetInt("testAfterPlayingReady", 0);
        }

        // Used to fire a method used for testing right after entering Playing mode
        private static bool testAfterPlayingReady;

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

        [MenuItem("Unity Test Tools/Build XposeCraft")]
        public static void Build()
        {
            var buildError = BuildPipeline.BuildPlayer(BuildScenes, GetLocation(), GetTarget(), GetOptions());
            if (!string.IsNullOrEmpty(buildError))
            {
                Debug.LogError(buildError);
            }
            Test();
        }

        /// <summary>
        /// Run tests after combining and opening the scenes.
        /// </summary>
        [MenuItem("Unity Test Tools/Test XposeCraft")]
        public static void Test()
        {
            OpenScenes(TestScenes);
            PlayerPrefs.SetInt("testAfterPlayingReady", 1);
            EditorApplication.isPlaying = true;
            var allTestComponents = TestComponent.FindAllTestsOnScene().ToList();
            var dynamicTests = allTestComponents.Where(t => t.dynamic).ToList();
            var dynamicTestsToRun = dynamicTests.Select(c => c.dynamicTypeName).ToList();
            allTestComponents.RemoveAll(dynamicTests.Contains);

            TestComponent.DisableAllTests();
            var testRunner = TestRunner.GetTestRunner();
            testRunner.TestRunnerCallback.Add(new TestCallback());
            testRunner.InitRunner(allTestComponents, dynamicTestsToRun);

            testRunner.StartCoroutine(testRunner.StateMachine());
            print("*** Start of testing ***");
        }

        private static void TestAfterPlaying()
        {
            // Currently not used, moved Passed and Failed functionality to the GameTestRunner
            new Thread(() =>
            {
                while (true)
                {
                    // _stopTests doesn't currently work, as its callback is never called from the TestRunner
                    if (_stopTests)
                    {
                        GameTestRunner.Passed = true;
                    }
                    Thread.Sleep(2000);
                }
            }).Start();
        }

        /// <summary>
        /// Failed attempt at running the test via an integrated support.
        /// </summary>
        [Obsolete]
        public static void TestBatchIntegration()
        {
            Batch.RunIntegrationTests(null, TestScenes.ToList(), new List<string>());
        }

        /// <summary>
        /// Opens the scenes based on scene file paths.
        /// </summary>
        /// <param name="scenes">Paths of scenes to be opened.</param>
        public static void OpenScenes(string[] scenes)
        {
            var emptyScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            for (int openSceneIndex = 0; openSceneIndex < SceneManager.sceneCount - 1; ++openSceneIndex)
            {
                var x = EditorSceneManager.CloseScene(SceneManager.GetSceneAt(openSceneIndex), true);
                Log.i(SceneManager.GetSceneAt(openSceneIndex).name + " " + x);
            }
            foreach (string scene in scenes)
            {
                EditorSceneManager.OpenScene(scene, OpenSceneMode.Additive);
            }
            EditorSceneManager.CloseScene(emptyScene, true);
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
