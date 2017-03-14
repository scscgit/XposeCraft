using UnityEngine;

// Removed Serializable so the generated Grid will not be persisted
// and instead will be autogenerated when the game starts
//[System.Serializable]
namespace XposeCraft.Core.Grid
{
    public class GridPoint
    {
        public Vector3 loc = Vector3.zero;

        //public bool check = false;
        //public bool added = false;
        //public float g_cost = 0;
        //public float f_cost = 0;

        public int parent;

        // State 0 = Open
        // State 1 = Walkable
        // State 2 = Closed
        public int state;

        public bool unit = false;

        // Convert to INT
        public int[] children;

        //public int[] sector;

        public int index;

        public GridPoint()
        {
        }

        public GridPoint(Vector3 nLoc)
        {
            loc = nLoc;
        }

        public GridPoint(GridPoint gridPoint)
        {
            loc = gridPoint.loc;
            state = gridPoint.state;
            children = gridPoint.children;
            index = gridPoint.index;
        }
    }
}
