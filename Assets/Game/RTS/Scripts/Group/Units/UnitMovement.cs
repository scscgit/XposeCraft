using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    Faction group;
    public int speed;
    public int rotateSpeed;
    public Vector3 target;
    public UGrid gridScript;
    public int gridI;
    public UPath myPath;
    public int depth;
    public bool pathComplete;
    int curPoint;
    public AStarManager pathing;
    public float checkDist = 1;
    public int layer;
    Transform myTransform;

    void Start()
    {
        Physics.IgnoreLayerCollision(layer, layer, true);
        pathing = GameObject.Find("A*").GetComponent<AStarManager>();
        gridScript = GameObject.Find("UGrid").GetComponent<UGrid>();
        myTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (pathComplete || myPath == null || myPath.list.Length <= 0)
        {
            return;
        }
        bool skip = false;
        for (int x = 1; x < myPath.list.Length - 1; x++)
        {
            if (gridScript.grids[gridI].grid[myPath.list[x]].state != 2)
            {
                continue;
            }
            RequestPath(target);
            skip = true;
            break;
        }
        if (skip)
        {
            return;
        }
        // Lerp Rotation
        Quaternion targetRotation = Quaternion.LookRotation(
            new Vector3(
                gridScript.grids[gridI].grid[myPath.list[curPoint]].loc.x,
                0,
                gridScript.grids[gridI].grid[myPath.list[curPoint]].loc.z)
            - new Vector3(myTransform.position.x, 0, myTransform.position.z));
        myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        myTransform.Translate(Vector3.forward * speed * Time.deltaTime);
        float distFromPlace = (new Vector3(
                                   gridScript.grids[gridI].grid[myPath.list[curPoint]].loc.x,
                                   0,
                                   gridScript.grids[gridI].grid[myPath.list[curPoint]].loc.z)
                               - new Vector3(
                                   myTransform.position.x,
                                   0,
                                   myTransform.position.z)).sqrMagnitude;
        if (!(distFromPlace < checkDist))
        {
            return;
        }
        curPoint++;
        if (curPoint == myPath.list.Length)
        {
            pathComplete = true;
        }
    }

    public void RequestPath(Vector3 target)
    {
        myPath = null;
        pathing.RequestPath(myTransform.position, target, gameObject, gridI);
        pathComplete = false;
    }

    public void SetPath(UPath path)
    {
        myPath = path;
        curPoint = 0;
    }

    void OnDrawGizmosSelected()
    {
        if (myPath != null && myPath.list.Length > 0)
        {
            myPath.color = Color.green;
            myPath.DisplayPath(curPoint, gridScript.grids[gridI].grid, gridScript.grids[gridI].nodeDist);
        }
    }
}
