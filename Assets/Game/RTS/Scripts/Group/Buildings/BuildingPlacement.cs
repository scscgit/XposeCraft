using UnityEngine;

public class BuildingPlacement : MonoBehaviour
{
    ResourceManager resourceManager;
    UnitSelection unitSelect;
    Fog fog;
    public bool place { get; set; }
    Building build;
    GameObject obj;
    UGrid uGrid;
    public int gridI;
    int loc;
    int factionIndex;
    bool aStar = false;
    public bool placed { get; set; }

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
        if (uGrid == null)
        {
            GameObject gridObj = GameObject.Find("UGrid");
            if (gridObj)
            {
                uGrid = gridObj.GetComponent<UGrid>();
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
        if (nBuild.tempObj == null)
        {
            Debug.LogWarning("Temp Object for the "
                             + (nBuild.obj == null ? "building" : nBuild.obj.name) + " is not set");
            return;
        }
        for (int x = 0; x < nBuild.cost.Length; x++)
        {
            if (nBuild.cost[x] > resourceManager.resourceTypes[x].amount)
            {
                return;
            }
        }
        if (place)
        {
            Destroy(obj);
        }
        place = true;
        build = nBuild;
        obj = Instantiate(nBuild.tempObj, Vector3.zero, Quaternion.identity) as GameObject;
    }

    public void SetFaction(int id)
    {
        factionIndex = id;
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
                float nodeSize = uGrid.grids[gridI].nodeDist;
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
        int i = uGrid.DetermineLoc(hit.point, gridI);
        if (uGrid.grids[gridI].points[i].state != 2)
        {
            loc = i;
            obj.transform.position = uGrid.grids[gridI].points[i].loc;
        }
        bool canPlace = build.CheckPoints(uGrid, gridI, loc) && fog.CheckLocation(obj.transform.position);
        if (!canPlace || !Input.GetButtonDown("LMB"))
        {
            return;
        }
        GameObject tempObj;
        build.ClosePoints(uGrid, gridI, loc, aStar);

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
        script.FactionIndex = factionIndex;
    }
}
