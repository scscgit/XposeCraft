using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

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
        EditorGUILayout.BeginHorizontal("");
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
        if (menuSetting == 0)
        {
            starget.grids[i].size = EditorGUILayout.IntField("Grid Size : ", starget.grids[i].size);
            starget.grids[i].nodeDist = EditorGUILayout.FloatField("Node Size : ", starget.grids[i].nodeDist);
            EditorGUILayout.Space();
            starget.grids[i].startLoc = EditorGUILayout.Vector3Field("Start Loc : ", starget.grids[i].startLoc);
            starget.grids[i].octileConnection = EditorGUILayout.Toggle(
                "Octile Connection : ",
                starget.grids[i].octileConnection);
            starget.grids[i].displaceDown = EditorGUILayout.Toggle("Displace Down : ", starget.grids[i].displaceDown);
            starget.grids[i].checkSlope = EditorGUILayout.Toggle("Check Slope : ", starget.grids[i].checkSlope);
            if (starget.grids[i].checkSlope)
            {
                starget.grids[i].slopeMax = EditorGUILayout.FloatField("Slope Max : ", starget.grids[i].slopeMax);
            }
            EditorGUILayout.Space();
            starget.grids[i].checkLayer = LayerMaskField("State Check Layers : ", starget.grids[i].checkLayer, true);
            starget.grids[i].displaceLayer = LayerMaskField("Displace Layer : ", starget.grids[i].displaceLayer, true);
        }
        else if (menuSetting == 1)
        {
            starget.grids[i].displayGrid = EditorGUILayout.Toggle("Display Cubes : ", starget.grids[i].displayGrid);
            starget.grids[i].displayLines = EditorGUILayout.Toggle("Display Lines : ", starget.grids[i].displayLines);
        }
        if (GUILayout.Button("Generate Grid"))
        {
            starget.GenerateGrid(starget.index);
        }
    }

    public static LayerMask LayerMaskField(string label, LayerMask selected, bool showSpecial)
    {
        if (layers == null
            || System.DateTime.Now.Ticks - lastUpdateTick > 10000000L && Event.current.type == EventType.Layout)
        {
            lastUpdateTick = System.DateTime.Now.Ticks;
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
