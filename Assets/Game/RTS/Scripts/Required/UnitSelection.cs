using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class UnitSelection : MonoBehaviour
{
    [FormerlySerializedAs("group")] public int FactionIndex;
    public int maxUnitSelect = 50;
    public int maxBuildingSelect = 50;
    public int maxTotalSelect = 100;
    public Texture selectTexture;
    public Texture borderTexture;
    public int borderSize = 3;
    public Rect windowRect;
    public LayerMask layer;
    public LayerMask selection;
    List<GameObject> unitList = new List<GameObject>();
    List<GameObject> buildingList = new List<GameObject>();
    int unitListLength;
    int buildingListLength;
    Vector2 startLoc;
    Vector3 startLocV3;
    Vector2 endLoc;
    Vector3 endLocV3;
    bool deselected;
    Vector3 movePoint;

    // Hidden Info
    //{

    bool mouseDown;
    bool largeSelect;
    float disp = 0.5f;
    public Rect selectionRect { get; set; }
    Rect localWindow { get; set; }
    public UGrid gridScript { get; set; }
    public MiniMap map { get; set; }
    public FactionManager FactionM { get; set; }
    public List<GameObject> curSelected { get; set; }
    public List<UnitController> curSelectedS { get; set; }
    public List<GameObject> curBuildSelected { get; set; }
    public List<BuildingController> curBuildSelectedS { get; set; }
    public int curSelectedLength { get; set; }
    public int curBuildSelectedLength { get; set; }
    public GUIManager guiManager { get; set; }
    public BuildingPlacement placement { get; set; }

    //}

    public void ResizeSelectionWindow(Vector2 ratio)
    {
        localWindow = new Rect(
            windowRect.x * ratio.x,
            windowRect.y * ratio.y,
            windowRect.width * ratio.x,
            windowRect.height * ratio.y
        );
    }

    public void Start()
    {
        gridScript = GameObject.Find("UGrid").GetComponent<UGrid>();
        FactionM = GameObject.Find("Faction Manager").GetComponent<FactionManager>();
        placement = gameObject.GetComponent<BuildingPlacement>();
        if (placement)
        {
            placement.SetFaction(FactionIndex);
        }
        GUIManager manager = gameObject.GetComponent<GUIManager>();
        if (manager)
        {
            manager.faction = FactionM.FactionList[FactionIndex].GetComponent<Faction>();
        }
        guiManager = gameObject.GetComponent<GUIManager>();
        GameObject obj = GameObject.Find("MiniMap");
        if (obj)
        {
            map = obj.GetComponent<MiniMap>();
        }
        //test = Camera.main.GetComponent<GUILayer>();
    }

    public void Update()
    {
        if (map)
        {
            if (
                !map.localBounds.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y))
                && Input.GetButton("LMB")
            )
            {
                if (!mouseDown)
                {
                    if (!guiManager.mouseOverGUI && !placement.place && !placement.placed)
                    {
                        mouseDown = true;
                        startLoc = Input.mousePosition;
                        startLoc = WithinScreen(startLoc);
                        if (!deselected)
                        {
                            Deselect();
                        }
                    }
                }
                else
                {
                    endLoc = Input.mousePosition;
                    endLoc = WithinScreen(endLoc);
                    if ((endLoc - startLoc).magnitude > disp)
                    {
                        largeSelect = true;
                    }
                }
            }
        }
        else if (Input.GetButton("LMB"))
        {
            if (!mouseDown)
            {
                if (!guiManager.mouseOverGUI && !placement.place && !placement.placed)
                {
                    mouseDown = true;
                    startLoc = Input.mousePosition;
                    startLoc = WithinScreen(startLoc);
                    if (!deselected)
                    {
                        Deselect();
                    }
                }
            }
            else
            {
                endLoc = Input.mousePosition;
                endLoc = WithinScreen(endLoc);
                if ((endLoc - startLoc).magnitude > disp)
                {
                    largeSelect = true;
                }
            }
        }

        //Check if mouse has been released, if so and largeSelect is true then call Select for box location and reset the variables
        if (Input.GetButtonUp("LMB"))
        {
            if (largeSelect)
            {
                Select();
            }
            mouseDown = false;
            largeSelect = false;
        }
        if (Input.GetButtonDown("RMB"))
        {
            RaycastHit hit;
            if (map)
            {
                if (map.localBounds.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
                {
                    Vector3 point = Determine3DLoc(new Vector2(
                        Input.mousePosition.x,
                        Screen.height - Input.mousePosition.y));
                    Physics.Raycast(point, Vector3.down, out hit, 10000);
                }
                else
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Physics.Raycast(ray, out hit, 10000, layer);
                }
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out hit, layer, 10000);
            }
            if (hit.collider)
            {
                SetTarget(hit.collider.gameObject, hit.point);
            }
        }
        deselected = false;
    }

    public void SetTarget(GameObject obj, Vector3 loc)
    {
        string type; // = "None";
        if (obj.CompareTag("Unit"))
        {
            type = "Unit";
        }
        else if (obj.CompareTag("Resource"))
        {
            type = "Resource";
        }
        else if (obj.CompareTag("Building"))
        {
            type = "Building";
        }
        else
        {
            type = "Location";
        }
        movePoint = loc;
        int z = 0;
        if (type == "Location")
        {
            for (int x = 0; x < curSelectedLength; x++)
            {
                UnitController unit = curSelectedS[z];
                unit.SetTarget(obj, movePoint, type);
                z++;
            }
        }
        else
        {
            for (int x = 0; x < curSelectedLength; x++)
            {
                curSelectedS[x].SetTarget(obj, movePoint, type);
            }
        }
    }

    Vector3 Determine3DLoc(Vector2 loc)
    {
        Rect size = new Rect(
            map.realWorldBounds.x / map.localBounds.x,
            map.realWorldBounds.y / map.localBounds.y,
            map.realWorldBounds.width / map.localBounds.width,
            map.realWorldBounds.height / map.localBounds.height);
        return new Vector3(
            (loc.x - map.localBounds.x) * size.width,
            100,
            map.realWorldBounds.y + map.realWorldBounds.height - (loc.y - map.localBounds.y) * size.height);
    }

    public void Select()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(startLoc);
        Physics.Raycast(ray, out hit, 10000, selection);
        if (hit.collider)
        {
            startLocV3 = hit.point;
        }
        ray = Camera.main.ScreenPointToRay(endLoc);
        Physics.Raycast(ray, out hit, 10000, selection);
        if (hit.collider)
        {
            endLocV3 = hit.point;
        }
        selectionRect = startLocV3.x > endLocV3.x
            ? new Rect(endLocV3.x, selectionRect.y, startLocV3.x, selectionRect.height)
            : new Rect(startLocV3.x, selectionRect.y, endLocV3.x, selectionRect.height);
        selectionRect = startLocV3.z > endLocV3.z
            ? new Rect(selectionRect.x, endLocV3.z, selectionRect.width, startLocV3.z)
            : new Rect(selectionRect.x, startLocV3.z, selectionRect.width, endLocV3.z);
        SelectionArea();
    }

    public void Deselect()
    {
        for (int x = 0; x < curSelectedLength; x++)
        {
            if (curSelected[x] == null)
            {
                curSelected.RemoveAt(x);
                x--;
                curSelectedLength--;
                continue;
            }
            curSelectedS[x].Select(false);
        }
        for (int x = 0; x < curBuildSelectedLength; x++)
        {
            if (curBuildSelected[x] == null)
            {
                curBuildSelected.RemoveAt(x);
                x--;
                curBuildSelectedLength--;
                continue;
            }
            curBuildSelected[x].GetComponent<BuildingController>().Select(false, 0);
        }
        curSelected = new List<GameObject>();
        curSelectedS = new List<UnitController>();
        curSelectedLength = 0;
        curBuildSelected = new List<GameObject>();
        curBuildSelectedS = new List<BuildingController>();
        curBuildSelectedLength = 0;
    }

    public void SelectionArea()
    {
        int curSelectedAmount = 0;
        for (int x = 0; x < unitListLength; x++)
        {
            if (unitList[x] == null)
            {
                unitList.RemoveAt(x);
                x--;
                unitListLength--;
                continue;
            }
            if (!(unitList[x].transform.position.x > selectionRect.x)
                || !(unitList[x].transform.position.x < selectionRect.width)
                || !(unitList[x].transform.position.z > selectionRect.y)
                || !(unitList[x].transform.position.z < selectionRect.height)
                || curSelectedAmount >= maxUnitSelect)
            {
                continue;
            }
            curSelectedAmount++;
            curSelected.Add(unitList[x]);
            curSelectedS.Add(unitList[x].GetComponent<UnitController>());
            curSelectedLength++;
            curSelectedS[curSelectedLength - 1].Select(true);
        }
        int curSelectedBuild = 0;
        for (int x = 0; x < buildingListLength; x++)
        {
            if (!(buildingList[x].transform.position.x > selectionRect.x)
                || !(buildingList[x].transform.position.x < selectionRect.width)
                || !(buildingList[x].transform.position.z > selectionRect.y)
                || !(buildingList[x].transform.position.z < selectionRect.height)
                || curSelectedAmount >= maxBuildingSelect)
            {
                continue;
            }
            curSelectedAmount++;
            curBuildSelected.Add(buildingList[x]);
            curBuildSelectedS.Add(buildingList[x].GetComponent<BuildingController>());
            curBuildSelectedLength++;
            buildingList[x].GetComponent<BuildingController>().Select(true, curSelectedBuild);
            curSelectedBuild++;
        }
    }

    public Vector2 WithinScreen(Vector2 loc)
    {
        if (loc.x < localWindow.x)
        {
            loc = new Vector2(localWindow.x, loc.y);
        }
        else if (loc.x > localWindow.x + localWindow.width)
        {
            loc = new Vector2(localWindow.x + localWindow.width, loc.y);
        }
        if (Screen.height - loc.y < localWindow.y)
        {
            loc = new Vector2(loc.x, Screen.height - localWindow.y);
        }
        else if (Screen.height - loc.y > localWindow.y + localWindow.height)
        {
            loc = new Vector2(loc.x, Screen.height - (localWindow.y + localWindow.height));
        }
        return loc;
    }

    public void OnGUI()
    {
        if (!largeSelect)
        {
            return;
        }
        Rect myRect = new Rect(0, 0, 0, 0);
        myRect = startLoc.x > endLoc.x
            ? new Rect(endLoc.x, myRect.y, startLoc.x - endLoc.x, myRect.height)
            : new Rect(startLoc.x, myRect.y, endLoc.x - startLoc.x, myRect.height);
        myRect = startLoc.y > endLoc.y
            ? new Rect(myRect.x, Screen.height - startLoc.y, myRect.width, startLoc.y - endLoc.y)
            : new Rect(myRect.x, Screen.height - endLoc.y, myRect.width, endLoc.y - startLoc.y);
        if (selectTexture)
        {
            GUI.DrawTexture(myRect, selectTexture);
        }
        if (!borderTexture)
        {
            return;
        }
        GUI.DrawTexture(new Rect(myRect.x, myRect.y, myRect.width, borderSize), borderTexture);
        GUI.DrawTexture(
            new Rect(myRect.x, myRect.y + myRect.height - borderSize, myRect.width, borderSize),
            borderTexture);
        GUI.DrawTexture(new Rect(myRect.x, myRect.y, borderSize, myRect.height), borderTexture);
        GUI.DrawTexture(
            new Rect(myRect.x + myRect.width - borderSize, myRect.y, borderSize, myRect.height),
            borderTexture);
    }

    public void Select(int i, string type)
    {
        if (type == "Unit")
        {
            for (int x = 0; x < curSelectedLength; x++)
            {
                curSelectedS[x].Select(false);
            }
            for (int x = 0; x < curBuildSelectedLength; x++)
            {
                curBuildSelectedS[x].Select(false, 0);
            }
            curBuildSelected = new List<GameObject>();
            curBuildSelectedS = new List<BuildingController>();
            curBuildSelectedLength = 0;
            GameObject obj = curSelected[i];
            UnitController cont = curSelectedS[i];
            curSelected = new List<GameObject>();
            curSelectedS = new List<UnitController>();
            curSelected.Add(obj);
            curSelectedS.Add(cont);
            curSelectedLength = 1;
            obj.SendMessage("Select", true);
        }
        else
        {
            for (int x = 0; x < curSelectedLength; x++)
            {
                curSelectedS[x].Select(false);
            }
            for (int x = 0; x < curBuildSelectedLength; x++)
            {
                curBuildSelectedS[x].Select(false, 0);
            }
            GameObject obj = curBuildSelected[i];
            BuildingController cont = curBuildSelectedS[i];
            curBuildSelected = new List<GameObject>();
            curBuildSelectedS = new List<BuildingController>();
            curBuildSelected.Add(obj);
            curBuildSelectedS.Add(cont);
            curBuildSelectedLength = 1;
            curSelected = new List<GameObject>();
            curSelectedS = new List<UnitController>();
            curSelectedLength = 0;
            cont.Select(true, 0);
        }
    }

    public void AddUnit(GameObject obj)
    {
        if (obj.GetComponent<UnitController>().FactionIndex == FactionIndex)
        {
            unitList.Add(obj);
            unitListLength++;
        }
    }

    public void RemoveUnit(GameObject obj)
    {
        for (int x = 0; x < unitListLength; x++)
        {
            if (unitList[x] == obj)
            {
                unitList.RemoveAt(x);
                unitListLength--;
                break;
            }
        }
    }

    public void AddSelectedUnit(GameObject obj)
    {
        Deselect();
        deselected = true;
        for (int x = 0; x < unitListLength; x++)
        {
            if (unitList[x] == obj)
            {
                curSelected.Add(unitList[x]);
                curSelectedS.Add(unitList[x].GetComponent<UnitController>());
                curSelectedLength++;
                curSelectedS[curSelectedLength - 1].Select(true);
                break;
            }
        }
    }

    public void AddSelectedBuilding(GameObject obj)
    {
        Deselect();
        deselected = true;
        for (int x = 0; x < buildingListLength; x++)
        {
            if (buildingList[x] == obj)
            {
                curBuildSelected.Add(buildingList[x]);
                curBuildSelectedS.Add(buildingList[x].GetComponent<BuildingController>());
                curBuildSelectedLength++;
                buildingList[x].GetComponent<BuildingController>().Select(true, 0);
                break;
            }
        }
    }

    public void AddBuilding(GameObject obj)
    {
        buildingList.Add(obj);
        buildingListLength++;
    }

    public void RemoveBuilding(GameObject obj)
    {
        for (int x = 0; x < buildingListLength; x++)
        {
            if (buildingList[x] == obj)
            {
                buildingList.RemoveAt(x);
                buildingListLength--;
                break;
            }
        }
    }
}
