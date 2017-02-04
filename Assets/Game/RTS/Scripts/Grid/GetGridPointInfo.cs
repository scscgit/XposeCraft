using UnityEngine;

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
        transform.position = myGrid.grids[gridI].grid[loc].loc;
        point = myGrid.grids[gridI].grid[loc];
    }

    public int ConvertLoc(Vector3 point)
    {
        float xLoc = point.x - myGrid.grids[gridI].startLoc.x;
        float zLoc = point.z - myGrid.grids[gridI].startLoc.z;
        int x = Mathf.RoundToInt(xLoc / myGrid.grids[gridI].nodeDist);
        int z = Mathf.RoundToInt(zLoc / myGrid.grids[gridI].nodeDist);
        return x + (z * myGrid.grids[gridI].size);
    }
}
