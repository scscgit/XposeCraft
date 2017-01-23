using UnityEngine;
using System.Collections;

public class DrawCube : MonoBehaviour
{
    UGrid grid = null;
    public int index = 0;
    float nodeSize = 0;

    // Use this for initialization
    void OnDrawGizmos()
    {
        if (grid == null)
        {
            grid = GameObject.Find("UGrid").GetComponent<UGrid>();
        }
        else
        {
            if (index < grid.grids.Length)
            {
                nodeSize = grid.grids[index].nodeDist;
                Gizmos.color = Color.green;
                Gizmos.DrawCube(gameObject.transform.position, new Vector3(nodeSize, nodeSize, nodeSize));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
