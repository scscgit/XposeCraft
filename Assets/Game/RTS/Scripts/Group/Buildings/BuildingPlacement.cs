using UnityEngine;

public class BuildingPlacement : MonoBehaviour
{
    ResourceManager resourceManager;
    UnitSelection unitSelect;
    Fog fog;
    [HideInInspector] public bool place;
    Building build;
    GameObject obj;
    UGrid grid;
    public int gridI;
    int loc;
    int group;
    bool aStar = false;
    [HideInInspector] public bool placed;

    public void Start()
    {
        if (resourceManager == null)
        {
            resourceManager = gameObject.GetComponent<ResourceManager>();
        }
        if (fog == null)
        {
            GameObject fogObj = GameObject.Find("Fog");
            if (fogObj)
            {
                fog = fogObj.GetComponent<Fog>();
            }
        }
        if (unitSelect == null)
        {
            unitSelect = gameObject.GetComponent<UnitSelection>();
        }
        if (grid == null)
        {
            GameObject gridObj = GameObject.Find("UGrid");
            if (gridObj)
            {
                grid = gridObj.GetComponent<UGrid>();
            }
        }
    }

    public void Update()
    {
        placed = false;
        if (place)
        {
            PlaceBuild();
        }
    }


    public void BeginPlace(Building nBuild)
    {
        bool canPlace = true;
        for (int x = 0; x < nBuild.cost.Length; x++)
        {
            if (nBuild.cost[x] > resourceManager.resourceTypes[x].amount)
            {
                canPlace = false;
            }
        }
        if (!canPlace)
        {
            return;
        }
        if (place)
        {
            Destroy(obj);
        }
        place = true;
        build = nBuild;
        obj = Instantiate(nBuild.tempObj, Vector3.zero, Quaternion.identity) as GameObject;
    }

    public void SetGroup(int id)
    {
        group = id;
    }

    public void OnDrawGizmos()
    {
        if (!place)
        {
            return;
        }
        for (int x = -build.closeWidth; x <= build.closeWidth; x++)
        {
            for (int y = -build.closeLength; y <= build.closeLength; y++)
            {
                switch (build.closePoints[
                    (x + build.closeWidth) * (build.closeLength * 2 + 1) + (y + build.closeLength)])
                {
                    case 0:
                        Gizmos.color = Color.green;
                        break;
                    case 1:
                        Gizmos.color = Color.yellow;
                        break;
                    default:
                        Gizmos.color = Color.red;
                        break;
                }
                float nodeSize = grid.grids[gridI].nodeDist;
                Gizmos.DrawCube(
                    new Vector3(
                        obj.transform.position.x + x * nodeSize,
                        obj.transform.position.y,
                        obj.transform.position.z + y * nodeSize),
                    new Vector3(nodeSize, nodeSize, nodeSize));
            }
        }
    }

    public void PlaceBuild()
    {
        if (Input.GetButtonDown("RMB"))
        {
            Destroy(obj);
            place = false;
        }
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, 10000);
        for (int x = 0; x < build.cost.Length; x++)
        {
            if (resourceManager.resourceTypes[x].amount - build.cost[x] >= 0)
            {
                continue;
            }
            Destroy(obj);
            place = false;
        }
        if (!hit.collider)
        {
            return;
        }
        int i = grid.DetermineLoc(hit.point, gridI);
        if (grid.grids[gridI].grid[i].state != 2)
        {
            loc = i;
            obj.transform.position = grid.grids[gridI].grid[i].loc;
        }
        bool canPlace = build.CheckPoints(grid, gridI, loc) && fog.CheckLocation(obj.transform.position);
        if (!canPlace || !Input.GetButtonDown("LMB"))
        {
            return;
        }
        GameObject tempObj;
        build.ClosePoints(grid, gridI, loc, aStar);

        if (build.autoBuild)
        {
            tempObj = Instantiate(build.obj, obj.transform.position, obj.transform.rotation) as GameObject;
        }
        else
        {
            tempObj = Instantiate(build.progressObj, obj.transform.position, obj.transform.rotation) as GameObject;
            unitSelect.SetTarget(tempObj, tempObj.transform.position);
        }
        if (!Input.GetButton("ContinuePlace"))
        {
            Destroy(obj);
            place = false;
            placed = true;
        }
        for (int x = 0; x < build.cost.Length; x++)
        {
            resourceManager.resourceTypes[x].amount -= build.cost[x];
        }
        BuildingController script = tempObj.GetComponent<BuildingController>();
        script.building = build;
        script.loc = loc;
        script.group = group;
    }
}
