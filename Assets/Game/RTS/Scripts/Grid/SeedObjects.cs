using UnityEngine;

public class SeedObjects : MonoBehaviour
{
    public Seed[] obj;
    UGrid grid;
    public int gridI;
    public bool generate;

    void OnDrawGizmos()
    {
        if (!grid)
        {
            grid = GameObject.Find("UGrid").GetComponent<UGrid>();
        }

        if (!generate)
        {
            return;
        }
        foreach (Seed seed in obj)
        {
            GameObject folder = new GameObject {name = "Folder"};
            for (int z = 0; z < seed.amount; z++)
            {
                int loc = 0;
                bool viable = false;
                while (viable == false)
                {
                    loc = Random.Range(0, grid.grids[gridI].grid.Length);
                    Vector3 point = grid.grids[gridI].grid[loc].loc;
                    if (point.x >= seed.area.x
                        && point.x <= seed.area.width
                        && point.z >= seed.area.y
                        && point.z <= seed.area.height)
                    {
                        viable = true;
                    }
                    if (grid.grids[gridI].grid[loc].state == 2)
                    {
                        viable = false;
                    }
                }
                GameObject clone =
                    Instantiate(seed.obj, grid.grids[gridI].grid[loc].loc, Quaternion.identity) as GameObject;
                clone.transform.parent = folder.transform;
                clone.name = seed.obj.name;
                grid.grids[gridI].grid[loc].state = 2;
            }
        }
        generate = false;
    }
}
