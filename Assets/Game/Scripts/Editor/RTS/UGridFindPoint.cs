using UnityEngine;
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
            GridPoint[] points = grid.grids[index].points;
            if (loc < points.Length && loc >= 0)
            {
                Vector3 pointLoc = points[loc].loc;
                GUILayout.Label("X : " + pointLoc.x + ", Y : " + pointLoc.y + ", Z : " + pointLoc.z);
                findPoint.transform.position = pointLoc;
                pointCube.index = index;
            }
        }
    }
}
