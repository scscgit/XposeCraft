using UnityEngine;
using XposeCraft.Core.Grids;

namespace XposeCraft.Core.Misc
{
    public class DrawCube : MonoBehaviour
    {
        UGrid grid;
        public int index;
        float nodeSize;

        void OnDrawGizmos()
        {
            if (grid == null)
            {
                grid = GameObject.Find("UGrid").GetComponent<UGrid>();
            }
            else if (index < grid.grids.Length)
            {
                nodeSize = grid.grids[index].nodeDist;
                Gizmos.color = Color.green;
                Gizmos.DrawCube(gameObject.transform.position, new Vector3(nodeSize, nodeSize, nodeSize));
            }
        }
    }
}
