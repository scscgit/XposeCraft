using UnityEngine;
using XposeCraft.Core.Required;

namespace XposeCraft.Core.Grids
{
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
            GridPoint[] points = grid.grids[gridI].points;
            foreach (Seed seed in obj)
            {
                GameObject folder = new GameObject {name = "Folder"};
                for (int z = 0; z < seed.amount; z++)
                {
                    int loc = 0;
                    bool viable = false;
                    while (viable == false)
                    {
                        loc = Random.Range(0, points.Length);
                        Vector3 point = points[loc].loc;
                        if (point.x >= seed.area.x
                            && point.x <= seed.area.width
                            && point.z >= seed.area.y
                            && point.z <= seed.area.height
                            && points[loc].state != 2)
                        {
                            viable = true;
                        }
                    }
                    GameObject clone = Instantiate(seed.obj, points[loc].loc, Quaternion.identity) as GameObject;
                    clone.transform.parent = folder.transform;
                    clone.name = seed.obj.name;
                    points[loc].state = 2;
                }
            }
            generate = false;
        }
    }
}
