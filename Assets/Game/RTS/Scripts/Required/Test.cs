using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour
{
    public APath path;

    // Update is called once per frame
    void Start()
    {
        path.FindPath(new Vector3(50, 0, 50), new Vector3(300, 0, 300));
    }

    void OnDrawGizmos()
    {
        if (path.myPath != null)
        {
            if (path.myPath.list.Length > 0)
            {
                path.myPath.color = Color.green;
                path.myPath.DisplayPath(0, path.gridScript.grids[0].grid, path.gridScript.grids[0].nodeDist);
            }
        }
    }
}
