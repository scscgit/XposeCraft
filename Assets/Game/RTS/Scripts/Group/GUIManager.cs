using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIManager : MonoBehaviour
{
    public GUIElement[] elements;

    public Vector2 standardSize = new Vector2(1720, 1080);
    public Rect unitGraphic = new Rect(0, 0, 0, 0);
    public Vector2 UColumnsXRows = new Vector2(10, 5);
    public Vector2 unitBDisp = new Vector2(5, 10);

    public bool universalBuild = false;
    public Rect buildButtonSize = new Rect(50, 900, 75, 75);
    public Vector2 BColumnsXRows = new Vector2(10, 5);
    public Vector2 buildingBDisp = new Vector2(80, 100);

    public Rect resourceSize = new Rect(0, 0, 100, 50);
    public Vector2 RColumnsXRows = new Vector2(10, 5);
    public Vector2 resourceBDisp = new Vector2(80, 100);

    public Rect miniMapSize = new Rect(1520, 880, 200, 200);

    public Rect unitDescriptionLocation = new Rect(0, 0, 100, 100);
    public Rect techDescriptionLocation = new Rect(0, 0, 100, 100);
    public Rect buildingDescriptionLocation = new Rect(0, 0, 100, 100);
    public GUISkin skin;
    BuildingPlacement place;
    UnitSelection select;
    [HideInInspector] public Faction group;
    ResourceManager resourceManager;
    Vector2 ratio;
    Vector2 lastWindowSize = Vector2.zero;
    public bool mouseOverGUI = false;
    [HideInInspector] public bool mouseOverUnitProduction = false;
    [HideInInspector] public int unitProductionIndex = 0;
    [HideInInspector] public bool mouseOverTechProduction = false;
    [HideInInspector] public int techProductionIndex = 0;
    [HideInInspector] public bool mouseOverBuildingProduction = false;
    [HideInInspector] public int buildingProductionIndex = 0;
    public bool lastState = false;
    int progressAmount = 0;
    List<Progress> progressList = new List<Progress>(0);

    void OnDrawGizmos()
    {
        if (gameObject.name != "Player Manager")
        {
            gameObject.name = "Player Manager";
        }
    }

    void Start()
    {
        if (place == null)
        {
            place = gameObject.GetComponent<BuildingPlacement>();
        }
        if (select == null)
        {
            select = gameObject.GetComponent<UnitSelection>();
        }
        if (resourceManager == null)
        {
            resourceManager = gameObject.GetComponent<ResourceManager>();
        }
        ReconfigureWindows();
    }

    void ReconfigureWindows()
    {
        ratio = new Vector2(Screen.width / standardSize.x, Screen.height / standardSize.y);
        GameObject.Find("MiniMap").GetComponent<MiniMap>().localBounds = new Rect(ratio.x * miniMapSize.x,
            ratio.y * miniMapSize.y, ratio.x * miniMapSize.width, ratio.y * miniMapSize.height);
        GameObject.Find("MiniMap").GetComponent<MiniMap>().SetSize();
        select.ResizeSelectionWindow(ratio);
        lastWindowSize = new Vector2(Screen.width, Screen.height);
        Debug.Log("Reconfiguring Windows");
    }

    void Update()
    {
        Vector2 windowSize = new Vector2(Screen.width, Screen.height);
        if (lastWindowSize != windowSize)
        {
            ReconfigureWindows();
        }
    }

    void OnGUI()
    {
        GUI.skin = skin;
        lastState = mouseOverGUI;
        mouseOverGUI = false;
        mouseOverUnitProduction = false;
        mouseOverTechProduction = false;
        useGUILayout = false;
        GUI.depth = 10;
        for (int x = 0; x < progressAmount; x++)
        {
            if (progressList[x] == null)
            {
                progressList.RemoveAt(x);
                x--;
                progressAmount--;
                continue;
            }
            progressList[x].DisplayProgress();
        }
        for (int x = 0; x < select.curSelectedLength; x++)
        {
            select.curSelectedS[x].DisplayHealth();
        }
        for (int x = 0; x < select.curBuildSelectedLength; x++)
        {
            select.curBuildSelectedS[x].DisplayHealth();
        }
        for (int x = 0; x < elements.Length; x++)
        {
            elements[x].Display(ratio);
            if (!elements[x].allowClickThrough)
            {
                if (elements[x].loc.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
                {
                    mouseOverGUI = true;
                }
            }
        }
        // Create Building
        if (universalBuild)
        {
            DisplayBuild();
        }
        else
        {
            bool canBuild = false;
            bool[] buildingCanBuild = new bool[group.BuildingList.Length];
            for (int x = 0; x < select.curSelectedLength; x++)
            {
                UnitController cont = select.curSelectedS[x];
                if (cont.build.builderUnit)
                {
                    for (int a = 0; a < cont.build.build.Length; a++)
                    {
                        if (!buildingCanBuild[a])
                        {
                            if (cont.build.build[a].canBuild)
                            {
                                buildingCanBuild[a] = true;
                            }
                        }
                    }
                    canBuild = true;
                }
            }
            if (canBuild)
            {
                DisplayBuild(buildingCanBuild);
            }
        }

        // Resource 

        int y = 0;
        int z = 0;
        for (int x = 0; x < resourceManager.resourceTypes.Length; x++)
        {
            // Displays the Resource and the Amount
            GUI.Box(new Rect((resourceSize.x + y * resourceBDisp.x) * ratio.x,
                    (resourceSize.y + z * resourceBDisp.y) * ratio.y,
                    (resourceSize.width) * ratio.x, (resourceSize.height) * ratio.y),
                resourceManager.resourceTypes[x].name + " : " + resourceManager.resourceTypes[x].amount);
            y = y + 1;
            if (y >= RColumnsXRows.x)
            {
                y = 0;
                z++;
                if (z >= RColumnsXRows.y)
                {
                    break;
                }
            }
        }

        y = 0;
        z = 0;

        // UnitController
        for (int x = 0; x < select.curSelectedLength; x++)
        {
            Rect rectLoc = new Rect((unitGraphic.x + (y * unitBDisp.x)) * ratio.x,
                (unitGraphic.y + (z * unitBDisp.y)) * ratio.y, unitGraphic.width * ratio.x,
                unitGraphic.height * ratio.y);
            if (GUI.Button(rectLoc, ""))
            {
                select.Select(x, "Unit");
            }
            if (rectLoc.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
            {
                mouseOverGUI = true;
            }
            // scsc: hotfix for gui trying to render e.g. 2-5th/5 units even after clicking on a 3rd one which selects only 1, thus getting out of bounds
            if (select.curSelectedS.Count > x && select.curSelectedS[x].gui.image)
            {
                GUI.DrawTexture(rectLoc, select.curSelectedS[x].gui.image);
            }
            y++;
            if (y >= UColumnsXRows.x)
            {
                y = 0;
                z++;
                if (z >= UColumnsXRows.y)
                {
                    break;
                }
            }
        }

        // Building Controller
        for (int x = 0; x < select.curBuildSelectedLength; x++)
        {
            Rect rectLoc = new Rect((unitGraphic.x + (y * unitBDisp.x)) * ratio.x,
                (unitGraphic.y + (z * unitBDisp.y)) * ratio.y, unitGraphic.width * ratio.x,
                unitGraphic.height * ratio.y);
            if (GUI.Button(rectLoc, ""))
            {
                select.Select(x, "Build");
            }
            if (rectLoc.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
            {
                mouseOverGUI = true;
            }
            if (select.curBuildSelectedS[x].gui.image != null)
            {
                GUI.DrawTexture(rectLoc, select.curBuildSelectedS[x].gui.image);
            }
            y++;
            if (y >= UColumnsXRows.x)
            {
                y = 0;
                z++;
                if (z >= UColumnsXRows.y)
                {
                    break;
                }
            }
        }

        // Building

        if (select.curBuildSelectedLength > 0)
        {
            select.curBuildSelectedS[0].DisplayGUI(ratio.x, ratio.y);
        }
        GUI.skin.box.wordWrap = true;
        if (mouseOverGUI)
        {
            if (mouseOverUnitProduction)
            {
                DisplayUnitProductionDescription();
            }
            else if (mouseOverTechProduction)
            {
                DisplayTechProductionDescription();
            }
            else if (mouseOverBuildingProduction)
            {
                DisplayBuildingProductionDescription();
            }
        }
    }

    public void DisplayBuild()
    {
        Vector2 ratio = new Vector2(Screen.width / standardSize.x, Screen.height / standardSize.y);
        int y = 0;
        int z = 0;
        for (int x = 0; x < group.BuildingList.Length; x++)
        {
            if (group.BuildingList[x].obj)
            {
                // Displays the Building Name
                Rect rectLoc = new Rect((buildButtonSize.x + y * buildingBDisp.x) * ratio.x,
                    (buildButtonSize.y + z * buildingBDisp.y) * ratio.y,
                    buildButtonSize.width * ratio.x, buildButtonSize.height * ratio.y);
                if (GUI.Button(rectLoc, group.BuildingList[x].obj.GetComponent<BuildingController>().name))
                {
                    place.BeginPlace(group.BuildingList[x]);
                }
                if (rectLoc.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
                {
                    mouseOverGUI = true;
                    mouseOverBuildingProduction = true;
                    buildingProductionIndex = x;
                }
                y = y + 1;
                if (y >= BColumnsXRows.x)
                {
                    y = 0;
                    z++;
                    if (z >= BColumnsXRows.y)
                    {
                        break;
                    }
                }
            }
        }
    }

    public void DisplayBuild(bool[] canBuild)
    {
        Vector2 ratio = new Vector2(Screen.width / standardSize.x, Screen.height / standardSize.y);
        int y = 0;
        int z = 0;
        for (int x = 0; x < group.BuildingList.Length; x++)
        {
            if (group.BuildingList[x].obj && canBuild[x])
            {
                // Displays the Building Name
                Rect rectLoc = new Rect((buildButtonSize.x + y * buildingBDisp.x) * ratio.x,
                    (buildButtonSize.y + z * buildingBDisp.y) * ratio.y,
                    buildButtonSize.width * ratio.x, buildButtonSize.height * ratio.y);
                if (GUI.Button(rectLoc, group.BuildingList[x].obj.GetComponent<BuildingController>().name))
                {
                    place.BeginPlace(group.BuildingList[x]);
                }
                if (rectLoc.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
                {
                    mouseOverGUI = true;
                    mouseOverBuildingProduction = true;
                    buildingProductionIndex = x;
                }
                y = y + 1;
                if (y >= BColumnsXRows.x)
                {
                    y = 0;
                    z++;
                    if (z >= BColumnsXRows.y)
                    {
                        break;
                    }
                }
            }
        }
    }

    public void DisplayUnitProductionDescription()
    {
        Rect rectLoc = new Rect(unitDescriptionLocation.x * ratio.x, unitDescriptionLocation.y * ratio.y,
            unitDescriptionLocation.width * ratio.x, unitDescriptionLocation.height * ratio.y);
        BuildingController sc = select.curBuildSelectedS[0].GetComponent<BuildingController>();
        GUI.Box(rectLoc, sc.unitProduction.units[unitProductionIndex].description);
    }

    public void DisplayTechProductionDescription()
    {
        Rect rectLoc = new Rect(techDescriptionLocation.x * ratio.x, techDescriptionLocation.y * ratio.y,
            techDescriptionLocation.width * ratio.x, techDescriptionLocation.height * ratio.y);
        BuildingController sc = select.curBuildSelectedS[0].GetComponent<BuildingController>();
        GUI.Box(rectLoc, sc.techProduction.techs[techProductionIndex].description);
    }

    public void DisplayBuildingProductionDescription()
    {
        Rect rectLoc = new Rect(buildingDescriptionLocation.x * ratio.x, buildingDescriptionLocation.y * ratio.y,
            buildingDescriptionLocation.width * ratio.x, buildingDescriptionLocation.height * ratio.y);
        GUI.Box(rectLoc, group.BuildingList[buildingProductionIndex].description);
    }

    public void AddProgress(Progress progress)
    {
        progressList.Add(progress);
        progressAmount++;
    }
}
