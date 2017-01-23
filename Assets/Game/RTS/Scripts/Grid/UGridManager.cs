using UnityEngine;

public class UGridManager : MonoBehaviour
{
    public UGrid grid;
    public bool displayGrid;
    public bool generate;
    public int size;

    void OnDrawGizmos()
    {
        grid.grids[0].displayGrid = displayGrid;
        grid.grids[0].size = size;
        grid.generate = generate;
        generate = false;
    }
}
