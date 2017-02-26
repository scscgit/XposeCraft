﻿using UnityEngine;
using UnityEditor;

public class UGridFindPoint : EditorWindow
{
    int loc;
    int index;
    UGrid grid;
    GameObject findPoint;
    DrawCube pointCube;

    [MenuItem("Window/Grid Find Point")]
    static void Init()
    {
        GetWindow(typeof(UGridFindPoint));
    }

    void OnGUI()
    {
        if (findPoint == null)
        {
            findPoint = new GameObject {name = "Find Point"};
            pointCube = findPoint.AddComponent<DrawCube>();
        }
        if (grid == null)
        {
            grid = GameObject.Find("UGrid").GetComponent<UGrid>();
        }
        index = EditorGUILayout.IntField("Grid : ", index);
        if (index < grid.grids.Length && index >= 0)
        {
            loc = EditorGUILayout.IntField("Index : ", loc);
            if (loc < grid.grids[index].grid.Length && loc >= 0)
            {
                GUILayout.Label("X : " + grid.grids[index].grid[loc].loc.x +
                                ", Y : " + grid.grids[index].grid[loc].loc.y +
                                ", Z : " + grid.grids[index].grid[loc].loc.z);
                findPoint.transform.position = grid.grids[index].grid[loc].loc;
                pointCube.index = index;
            }
        }
    }
}