using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XposeCraft.Core.Grids;

namespace XposeCraft.RTS
{
    [CustomEditor(typeof(UGrid))]
    public class UGridEditor : Editor
    {
        int menuSetting;
        Texture selected;
        public static List<string> layers;
        public static List<int> layerNumbers;
        public static string[] layerNames;
        public static long lastUpdateTick;

        public override void OnInspectorGUI()
        {
            UGrid starget = (UGrid) target;
            selected = starget.selectionTexture;
            string[] gridList = new string[starget.grids.Length];
            for (int x = 0; x < gridList.Length; x++)
            {
                gridList[x] = "Grid " + (x + 1);
            }
            int size = gridList.Length;
            int lsize = size;
            size = EditorGUILayout.IntField("Grid Amount : ", size);
            if (size != lsize)
            {
                ModifyG(lsize, size, starget);
            }
            starget.index = EditorGUILayout.Popup(starget.index, gridList);
            EditorGUILayout.Space();
            if (starget.index >= gridList.Length)
            {
                starget.index = 0;
                return;
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Settings"))
            {
                menuSetting = 0;
            }
            if (menuSetting == 0)
            {
                GUI.DrawTexture(GUILayoutUtility.GetLastRect(), selected);
            }
            if (GUILayout.Button("Display"))
            {
                menuSetting = 1;
            }
            if (menuSetting == 1)
            {
                GUI.DrawTexture(GUILayoutUtility.GetLastRect(), selected);
            }
            EditorGUILayout.EndHorizontal();
            int i = starget.index;
            Grid grid = starget.grids[i];
            if (menuSetting == 0)
            {
                grid.size = EditorGUILayout.IntField("Grid Size : ", grid.size);
                grid.nodeDist = EditorGUILayout.FloatField("Node Size : ", grid.nodeDist);
                EditorGUILayout.Space();
                grid.startLoc = EditorGUILayout.Vector3Field("Start Loc : ", grid.startLoc);
                grid.octileConnection = EditorGUILayout.Toggle("Octile Connection : ", grid.octileConnection);
                grid.displaceDown = EditorGUILayout.Toggle("Displace Down : ", grid.displaceDown);
                grid.checkSlope = EditorGUILayout.Toggle("Check Slope : ", grid.checkSlope);
                if (grid.checkSlope)
                {
                    grid.slopeMax = EditorGUILayout.FloatField("Slope Max : ", grid.slopeMax);
                }
                EditorGUILayout.Space();
                grid.checkLayer = LayerMaskField("State Check Layers : ", grid.checkLayer, true);
                grid.displaceLayer = LayerMaskField("Displace Layer : ", grid.displaceLayer, true);
            }
            else if (menuSetting == 1)
            {
                grid.displayGrid = EditorGUILayout.Toggle("Display Cubes : ", grid.displayGrid);
                grid.displayLines = EditorGUILayout.Toggle("Display Lines : ", grid.displayLines);
            }
            if (GUILayout.Button("Generate Grid"))
            {
                starget.GenerateGrid(starget.index);
                // Immediate effect instead of waiting for the camera to move
                SceneView.RepaintAll();
            }
        }

        public static LayerMask LayerMaskField(string label, LayerMask selected, bool showSpecial)
        {
            if (layers == null
                || DateTime.Now.Ticks - lastUpdateTick > 10000000L && Event.current.type == EventType.Layout)
            {
                lastUpdateTick = DateTime.Now.Ticks;
                if (layers == null)
                {
                    layers = new List<string>();
                    layerNumbers = new List<int>();
                    layerNames = new string[4];
                }
                else
                {
                    layers.Clear();
                    layerNumbers.Clear();
                }

                int emptyLayers = 0;
                for (int i = 0; i < 32; i++)
                {
                    string layerName = LayerMask.LayerToName(i);

                    if (layerName != "")
                    {
                        for (; emptyLayers > 0; emptyLayers--)
                        {
                            layers.Add("Layer " + (i - emptyLayers));
                        }
                        layerNumbers.Add(i);
                        layers.Add(layerName);
                    }
                    else
                    {
                        emptyLayers++;
                    }
                }

                if (layerNames.Length != layers.Count)
                {
                    layerNames = new string[layers.Count];
                }
                for (int i = 0; i < layerNames.Length; i++)
                {
                    layerNames[i] = layers[i];
                }
            }

            selected.value = EditorGUILayout.MaskField(label, selected.value, layerNames);

            return selected;
        }

        void ModifyG(int ol, int nl, UGrid starget)
        {
            Grid[] copyArr = new Grid[ol];
            for (int x = 0; x < copyArr.Length; x++)
            {
                copyArr[x] = starget.grids[x];
            }
            starget.grids = new Grid[nl];
            if (nl < ol)
            {
                for (int x = 0; x < copyArr.Length - 1; x++)
                {
                    starget.grids[x] = copyArr[x];
                }
            }
            else
            {
                for (int x = 0; x < starget.grids.Length - 1; x++)
                {
                    starget.grids[x] = copyArr[x];
                }
                starget.grids[nl - 1] = new Grid();
            }
        }
    }
}
