using System;
using UnityEngine;

namespace XposeCraft.Core.Grid
{
    [Serializable]
    public class Grid
    {
        // base points information

        public int size;
        public GridPoint[] points;
        public Vector3 startLoc;
        public bool octileConnection;
        public float nodeDist;

        // specific points options

        public bool displayGrid { get; set; }
        public bool displayLines { get; set; }
        public bool displaceDown = true;
        public bool checkSlope = false;
        public float slopeMax = 5;
        public LayerMask checkLayer;
        public LayerMask displaceLayer;

        public void DisplayGrid(int depth)
        {
            if (displayGrid)
            {
                for (int x = 1; x < points.Length; x++)
                {
                    Gizmos.color = points[x].state == 2
                        ? Color.red
                        : Color.green;
                    Gizmos.DrawCube(
                        new Vector3(points[x].loc.x, points[x].loc.y, points[x].loc.z),
                        new Vector3(nodeDist, nodeDist, nodeDist));
                }
            }
            if (!displayLines)
            {
                return;
            }

            for (int x = 0; x < points.Length; x++)
            {
                GridPoint point = points[x];
                for (int y = 0; y < point.children.Length; y++)
                {
                    var childPosition = points[point.children[y]].loc;
                    var currentPosition = new Vector3(point.loc.x, point.loc.y, point.loc.z);
                    Gizmos.color = point.state == 2 ? Color.red : Color.green;
                    Gizmos.DrawLine(currentPosition, (childPosition - currentPosition) / 2 + currentPosition);
                }
            }
        }
    }
}
