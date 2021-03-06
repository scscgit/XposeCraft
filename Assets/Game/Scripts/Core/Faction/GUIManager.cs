using UnityEngine;
using XposeCraft.Core.Faction.Buildings;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Core.Required;
using XposeCraft.Core.Resources;
using XposeCraft.GameInternal;
using XposeCraft.UI.MiniMap;
using GUIElement = XposeCraft.Core.Required.GUIElement;

namespace XposeCraft.Core.Faction
{
    public class GUIManager : MonoBehaviour
    {
        public GUIElement[] elements;

        public Vector2 standardSize = new Vector2(1720, 1080);
        public Rect unitGraphic = new Rect(0, 0, 0, 0);
        public Vector2 UColumnsXRows = new Vector2(10, 5);
        public Vector2 unitBDisp = new Vector2(5, 10);

        public bool universalBuild;
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
        public Faction faction { get; set; }

        private static ResourceManager ResourceManagerPlayerOnly
        {
            get { return GameManager.Instance.CurrentPlayerResourceManager; }
        }

        Vector2 ratio;
        Vector2 lastWindowSize = Vector2.zero;
        public bool mouseOverGUI;
        public bool mouseOverUnitProduction { get; set; }
        public int unitProductionIndex { get; set; }
        public bool mouseOverTechProduction { get; set; }
        public int techProductionIndex { get; set; }
        public bool mouseOverBuildingProduction { get; set; }
        public int buildingProductionIndex { get; set; }
        public bool lastState;
        int progressAmount;
        MiniMap miniMap;

        void OnDrawGizmos()
        {
            if (gameObject.name != "Player Manager")
            {
                gameObject.name = "Player Manager";
            }
        }

        private void Awake()
        {
            place = gameObject.GetComponent<BuildingPlacement>();
            select = gameObject.GetComponent<UnitSelection>();
            miniMap = GameObject.Find("MiniMap").GetComponent<MiniMap>();
            faction = GameObject.Find("Faction Manager")
                .GetComponent<FactionManager>()
                .FactionList[select.FactionIndex]
                .GetComponent<Faction>();
            ReconfigureWindows();
        }

        void ReconfigureWindows()
        {
            ratio = new Vector2(Screen.width / standardSize.x, Screen.height / standardSize.y);
            miniMap.localBounds = new Rect(
                ratio.x * miniMapSize.x,
                ratio.y * miniMapSize.y,
                ratio.x * miniMapSize.width,
                ratio.y * miniMapSize.height);
            miniMap.SetSize();
            select.ResizeSelectionWindow(ratio);
            lastWindowSize = new Vector2(Screen.width, Screen.height);
            Log.d("Reconfiguring Windows");
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
            if (GameManager.Instance.DisplayAllHealthBars)
            {
                foreach (var unit in select.UnitList)
                {
                    if (GameManager.Instance.DisplayOnlyDamagedHealthBars
                        && unit.GetHealth() == unit.GetMaxHealth())
                    {
                        continue;
                    }
                    unit.DisplayHealth();
                }
                foreach (var building in select.BuildingList)
                {
                    if (GameManager.Instance.DisplayOnlyDamagedHealthBars
                        && building.GetHealth() == building.GetMaxHealth())
                    {
                        continue;
                    }
                    building.DisplayHealth();
                }
            }
            else
            {
                for (int x = 0; x < select.curSelectedLength; x++)
                {
                    select.curSelectedS[x].DisplayHealth();
                }
                for (int x = 0; x < select.curBuildSelectedLength; x++)
                {
                    select.curBuildSelectedS[x].DisplayHealth();
                }
            }
            foreach (GUIElement element in elements)
            {
                element.Display(ratio);
                if (element.allowClickThrough)
                {
                    continue;
                }
                if (element.loc.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
                {
                    mouseOverGUI = true;
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
                bool[] buildingCanBuild = new bool[faction.BuildingList.Length];
                for (int x = 0; x < select.curSelectedLength; x++)
                {
                    UnitController cont = select.curSelectedS[x];
                    if (!cont.build.builderUnit)
                    {
                        continue;
                    }
                    for (int a = 0; a < cont.build.build.Length; a++)
                    {
                        if (buildingCanBuild[a])
                        {
                            continue;
                        }
                        if (cont.build.build[a].canBuild)
                        {
                            buildingCanBuild[a] = true;
                        }
                    }
                    canBuild = true;
                }
                if (canBuild)
                {
                    DisplayBuild(buildingCanBuild);
                }
            }

            // Resource

            int y = 0;
            int z = 0;
            for (int x = 0; x < ResourceManagerPlayerOnly.resourceTypes.Length; x++)
            {
                // Displays the Resource and the Amount
                GUI.Box(new Rect(
                        (resourceSize.x + y * resourceBDisp.x) * ratio.x,
                        (resourceSize.y + z * resourceBDisp.y) * ratio.y,
                        resourceSize.width * ratio.x,
                        resourceSize.height * ratio.y),
                    ResourceManagerPlayerOnly.resourceTypes[x].name
                    + " : " + ResourceManagerPlayerOnly.resourceTypes[x].amount);
                y = y + 1;
                if (y < RColumnsXRows.x)
                {
                    continue;
                }
                y = 0;
                z++;
                if (z >= RColumnsXRows.y)
                {
                    break;
                }
            }

            y = 0;
            z = 0;

            // UnitController
            for (int x = 0; x < select.curSelectedLength; x++)
            {
                Rect rectLoc = new Rect(
                    (unitGraphic.x + y * unitBDisp.x) * ratio.x,
                    (unitGraphic.y + z * unitBDisp.y) * ratio.y,
                    unitGraphic.width * ratio.x,
                    unitGraphic.height * ratio.y);
                if (GUI.Button(rectLoc, ""))
                {
                    select.Select(x, "Unit");
                }
                if (rectLoc.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
                {
                    mouseOverGUI = true;
                }
                // Hotfix for GUI trying to render remaining units after clicking on a single one, getting out of bounds
                if (select.curSelectedS.Count > x && select.curSelectedS[x].gui.image)
                {
                    GUI.DrawTexture(rectLoc, select.curSelectedS[x].gui.image);
                }
                y++;
                if (y < UColumnsXRows.x)
                {
                    continue;
                }
                y = 0;
                z++;
                if (z >= UColumnsXRows.y)
                {
                    break;
                }
            }

            // Building Controller
            for (int x = 0; x < select.curBuildSelectedLength; x++)
            {
                Rect rectLoc = new Rect(
                    (unitGraphic.x + (y * unitBDisp.x)) * ratio.x,
                    (unitGraphic.y + (z * unitBDisp.y)) * ratio.y,
                    unitGraphic.width * ratio.x,
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
                if (y < UColumnsXRows.x)
                {
                    continue;
                }
                y = 0;
                z++;
                if (z >= UColumnsXRows.y)
                {
                    break;
                }
            }

            // Building

            if (select.curBuildSelectedLength > 0)
            {
                select.curBuildSelectedS[0].DisplayGUI(ratio.x, ratio.y);
            }
            GUI.skin.box.wordWrap = true;
            if (!mouseOverGUI)
            {
                return;
            }
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

        public void DisplayBuild()
        {
            Vector2 ratio = new Vector2(Screen.width / standardSize.x, Screen.height / standardSize.y);
            int y = 0;
            int z = 0;
            for (int x = 0; x < faction.BuildingList.Length; x++)
            {
                if (!faction.BuildingList[x].obj)
                {
                    continue;
                }
                // Displays the Building Name
                Rect rectLoc = new Rect(
                    (buildButtonSize.x + y * buildingBDisp.x) * ratio.x,
                    (buildButtonSize.y + z * buildingBDisp.y) * ratio.y,
                    buildButtonSize.width * ratio.x,
                    buildButtonSize.height * ratio.y);
                if (GUI.Button(rectLoc, faction.BuildingList[x].obj.GetComponent<BuildingController>().name))
                {
                    place.BeginPlace(faction.BuildingList[x]);
                }
                if (rectLoc.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
                {
                    mouseOverGUI = true;
                    mouseOverBuildingProduction = true;
                    buildingProductionIndex = x;
                }
                y = y + 1;
                if (y < BColumnsXRows.x)
                {
                    continue;
                }
                y = 0;
                z++;
                if (z >= BColumnsXRows.y)
                {
                    break;
                }
            }
        }

        public void DisplayBuild(bool[] canBuild)
        {
            Vector2 ratio = new Vector2(Screen.width / standardSize.x, Screen.height / standardSize.y);
            int y = 0;
            int z = 0;
            for (int x = 0; x < faction.BuildingList.Length; x++)
            {
                if (!faction.BuildingList[x].obj || !canBuild[x])
                {
                    continue;
                }
                // Displays the Building Name
                Rect rectLoc = new Rect(
                    (buildButtonSize.x + y * buildingBDisp.x) * ratio.x,
                    (buildButtonSize.y + z * buildingBDisp.y) * ratio.y,
                    buildButtonSize.width * ratio.x,
                    buildButtonSize.height * ratio.y);
                if (GUI.Button(rectLoc, faction.BuildingList[x].obj.GetComponent<BuildingController>().name))
                {
                    place.BeginPlace(faction.BuildingList[x]);
                }
                if (rectLoc.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
                {
                    mouseOverGUI = true;
                    mouseOverBuildingProduction = true;
                    buildingProductionIndex = x;
                }
                y = y + 1;
                if (y < BColumnsXRows.x)
                {
                    continue;
                }
                y = 0;
                z++;
                if (z >= BColumnsXRows.y)
                {
                    break;
                }
            }
        }

        public void DisplayUnitProductionDescription()
        {
            Rect rectLoc = new Rect(
                unitDescriptionLocation.x * ratio.x,
                unitDescriptionLocation.y * ratio.y,
                unitDescriptionLocation.width * ratio.x,
                unitDescriptionLocation.height * ratio.y);
            BuildingController sc = select.curBuildSelectedS[0].GetComponent<BuildingController>();
            GUI.Box(rectLoc, sc.unitProduction.units[unitProductionIndex].description);
        }

        public void DisplayTechProductionDescription()
        {
            Rect rectLoc = new Rect(
                techDescriptionLocation.x * ratio.x,
                techDescriptionLocation.y * ratio.y,
                techDescriptionLocation.width * ratio.x,
                techDescriptionLocation.height * ratio.y);
            BuildingController sc = select.curBuildSelectedS[0].GetComponent<BuildingController>();
            GUI.Box(rectLoc, sc.techProduction.techs[techProductionIndex].description);
        }

        public void DisplayBuildingProductionDescription()
        {
            Rect rectLoc = new Rect(
                buildingDescriptionLocation.x * ratio.x,
                buildingDescriptionLocation.y * ratio.y,
                buildingDescriptionLocation.width * ratio.x,
                buildingDescriptionLocation.height * ratio.y);
            GUI.Box(rectLoc, faction.BuildingList[buildingProductionIndex].description);
        }
    }
}
