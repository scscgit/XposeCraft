using System.Reflection;
using UnityEditor;
using UnityEngine;

// ReSharper disable PossibleNullReferenceException

namespace XposeCraft.UnityWorkarounds.Downloaded
{
    /// <summary>
    /// Increases the maximum speed of Unity Editor camera.
    /// Downloaded from https://gist.github.com/kalineh/4408cd47c61c8f134aa0
    /// </summary>
    [InitializeOnLoad]
    public class EditorCameraSpeed : EditorWindow
    {
        static EditorCameraSpeed()
        {
            SceneView.onSceneGUIDelegate += OnSceneDelegate;
        }

        [MenuItem("Window/Camera Speed &S")]
        public static void CameraSpeed()
        {
            GetWindow<EditorCameraSpeed>();
        }

        static void CameraSpeedUpdate()
        {
            //var e = Event.current;

            // Tools.s_LockedViewTool is ViewTool.FPS when holding right-click down
            // SceneView.OnGUI() calls SceneViewMotion.DoViewTool(self)
            // SceneViewMotion.DoViewTool(): key down event: process WASD and add to s_Motion vector
            // SceneViewMotion.DoViewTool(): layout event: view.pivot change by s_Motion * internal dt tracking

            // solution 1: try and modify speed values during this update
            // -> doesn't work because the timing of events and this call is different, and it is out of sync

            // solution 2: get a callback in our code somewhere during the gui event handler so we can modify some values
            // -> doesnt work because there seems to be no delegates or callbacks during SceneView.OnGUI that we can hook into

            // solution 3: modify ilcode bytes of SceneViewMotion.GetMovementDirection() to return a modified value
            // -> can modify ?

            // solution 4: replace kFPSPref* input keys and move the main window pivot ourselves
            // -> gotta implement a lot?

            // solution 5: build custom permanent control and listen for OnGUI()
            // -> will get events when not focused?
            // -> will get events that are consumed by scene view?
        }

        static float cameraMoveSpeed = 80.0f;
        static float cameraMoveSpeedCtrl = 1.0f;

        public void OnGUI()
        {
            var event_ = Event.current;
            var controlID = GUIUtility.GetControlID(FocusType.Passive);
            event_.GetTypeForControl(controlID);

            cameraMoveSpeed = EditorGUILayout.Slider(cameraMoveSpeed, 0.0f, 500.0f);
            cameraMoveSpeedCtrl = EditorGUILayout.Slider(cameraMoveSpeedCtrl, 0.1f, 1.0f);

            SceneView.onSceneGUIDelegate += OnSceneDelegate;
        }


        public static void OnSceneDelegate(SceneView sceneView)
        {
            if (Event.current.type != EventType.Layout
                || (ViewTool) typeof(Tools)
                    .GetField("s_LockedViewTool", BindingFlags.NonPublic | BindingFlags.Static)
                    .GetValue(null)
                != ViewTool.FPS)
            {
                return;
            }

            Assembly
                .GetAssembly(typeof(SceneView))
                .GetType("UnityEditor.SceneViewMotion")
                .GetField("s_FlySpeed", BindingFlags.NonPublic | BindingFlags.Static)
                .SetValue(null, Event.current.control ? cameraMoveSpeedCtrl : cameraMoveSpeed);

            // we can stop input with this
            //Event.current.Use();
        }
    }
}
