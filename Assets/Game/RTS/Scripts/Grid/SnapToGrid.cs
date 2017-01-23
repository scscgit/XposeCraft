using UnityEngine;

public class SnapToGrid : MonoBehaviour
{
    public UGrid myGrid;
    public int gridI;
    public bool snap;
    public int gridLoc;

    void OnDrawGizmosSelected()
    {
        if (snap)
        {
            int loc = ConvertLoc(transform.position);
            gridLoc = loc;
            transform.position = myGrid.grids[gridI].grid[loc].loc;
        }
    }

    public int ConvertLoc(Vector3 point)
    {
        float xLoc = (point.x - myGrid.grids[gridI].startLoc.x);
        float yLoc = (point.z - myGrid.grids[gridI].startLoc.z);
        int x = Mathf.RoundToInt(xLoc / myGrid.grids[gridI].nodeDist);
        int y = Mathf.RoundToInt(yLoc / myGrid.grids[gridI].nodeDist);
        int loc = x + y * myGrid.grids[gridI].size;
        return loc;
    }
}
