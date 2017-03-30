using UnityEngine;

namespace XposeCraft.Core.Grids
{
    public class GetGridPointInfo : MonoBehaviour
    {
        public UGrid myGrid;
        public int gridI;
        public bool snap;
        public int gridLoc;
        public GridPoint point;

        void OnDrawGizmosSelected()
        {
            if (!snap)
            {
                return;
            }
            int loc = ConvertLoc(transform.position);
            gridLoc = loc;
            point = myGrid.grids[gridI].points[loc];
            transform.position = point.loc;
        }

        public int ConvertLoc(Vector3 point)
        {
            Grid grid = myGrid.grids[gridI];
            float xLoc = point.x - grid.startLoc.x;
            float zLoc = point.z - grid.startLoc.z;
            int x = Mathf.RoundToInt(xLoc / grid.nodeDist);
            int z = Mathf.RoundToInt(zLoc / grid.nodeDist);
            return x + z * grid.size;
        }
    }
}
