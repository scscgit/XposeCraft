using UnityEditor;
using UnityEngine;
using XposeCraft.Core.Grids;

namespace XposeCraft.RTS
{
    public class UGridFindPoint : EditorWindow
    {
        private int _pointIndex;
        private int _gridIndex;
        private UGrid _uGrid;

        [MenuItem("Window/Grid Find Point")]
        static void Init()
        {
            GetWindow(typeof(UGridFindPoint));
        }

        void OnGUI()
        {
            if (_uGrid == null)
            {
                _uGrid = GameObject.Find("UGrid").GetComponent<UGrid>();
            }
            _gridIndex = EditorGUILayout.IntField("Grid : ", _gridIndex);
            if (_gridIndex < _uGrid.grids.Length && _gridIndex >= 0)
            {
                _pointIndex = EditorGUILayout.IntField("Index : ", _pointIndex);
                GridPoint[] points = _uGrid.grids[_gridIndex].points;
                if (_pointIndex < points.Length && _pointIndex >= 0)
                {
                    Vector3 pointLoc = points[_pointIndex].loc;
                    GUILayout.Label("X : " + pointLoc.x + ", Y : " + pointLoc.y + ", Z : " + pointLoc.z);
                    _uGrid.FindPointGridIndex = _gridIndex;
                    _uGrid.FindPointLoc = pointLoc;
                    SceneView.RepaintAll();
                }
            }
        }
    }
}
