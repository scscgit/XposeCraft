using UnityEngine;

namespace XposeCraft.Core.Grids
{
    public class SnapToGrid : MonoBehaviour
    {
        public UGrid myGrid;
        public int gridI;
        public bool snap;
        public int gridLoc;

        void OnDrawGizmosSelected()
        {
            if (!snap)
            {
                return;
            }
            int loc = ConvertLoc(transform.position);
            gridLoc = loc;
            transform.position = myGrid.grids[gridI].points[loc].loc;
        }

        public int ConvertLoc(Vector3 point)
        {
            float xLoc = point.x - myGrid.grids[gridI].startLoc.x;
            float yLoc = point.z - myGrid.grids[gridI].startLoc.z;
            int x = Mathf.RoundToInt(xLoc / myGrid.grids[gridI].nodeDist);
            int y = Mathf.RoundToInt(yLoc / myGrid.grids[gridI].nodeDist);
            return x + y * myGrid.grids[gridI].size;
        }
    }
}
