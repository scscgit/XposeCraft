using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XposeCraft.Core.Faction;
using XposeCraft.Core.Faction.Buildings;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Core.Grids;
using XposeCraft.GameInternal;
using XposeCraft.UI.MiniMap;

namespace XposeCraft.Core.Required
{
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

        // Hidden Info
        //{

        bool mouseDown;
        bool largeSelect;
        float disp = 0.5f;
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
            if (map
                && !map.localBounds.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y))
                && Input.GetButton("LMB")
                || Input.GetButton("LMB"))
            {
                if (mouseDown)
                {
                    endLoc = Input.mousePosition;
                    endLoc = WithinScreen(endLoc);
                    if ((endLoc - startLoc).magnitude > disp)
                    {
                        largeSelect = true;
                    }
                }
                else if (!guiManager.mouseOverGUI && !placement.place && !placement.placed)
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
                if (map && map.localBounds.Contains(new Vector2(
                        Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
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
                if (hit.collider)
                {
                    SetTarget(hit.collider.gameObject, hit.point);
                }
            }
            deselected = false;
        }

        public void SetTarget(GameObject obj, Vector3 loc)
        {
            SetTarget(curSelectedS, obj, loc);
        }

        public static void SetTarget(List<UnitController> units, GameObject obj, Vector3 loc)
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
            foreach (UnitController unit in units)
            {
                if (unit == null)
                {
                    Log.e(typeof(UnitSelection), "UnitController with null value has attempted to set its target");
                    continue;
                }
                unit.SetTarget(obj, loc, type);
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
            SelectArea(startLocV3, endLocV3);
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
                curBuildSelected[x].GetComponent<BuildingController>().Select(false);
            }
            curSelected = new List<GameObject>();
            curSelectedS = new List<UnitController>();
            curSelectedLength = 0;
            curBuildSelected = new List<GameObject>();
            curBuildSelectedS = new List<BuildingController>();
            curBuildSelectedLength = 0;
        }

        public void SelectArea(Vector3 startLocation, Vector3 endLocation)
        {
            var units = new List<GameObject>();
            var buildings = new List<GameObject>();
            SelectionFind(startLocation, endLocation, units, buildings, true);
            for (int unitIndex = 0; unitIndex < units.Count; unitIndex++)
            {
                var unit = units[unitIndex];
                curSelected.Add(unit);
                var controller = unit.GetComponent<UnitController>();
                controller.Select(true);
                curSelectedS.Add(controller);
                curSelectedLength++;
            }
            for (int buildingIndex = 0; buildingIndex < buildings.Count; buildingIndex++)
            {
                var building = buildings[buildingIndex];
                curBuildSelected.Add(building);
                var controller = building.GetComponent<BuildingController>();
                controller.Select(true);
                curBuildSelectedS.Add(controller);
                curBuildSelectedLength++;
            }
        }

        /// <summary>
        /// Adds GameObjects representing units and buildings from the map within the rectangle size to the lists.
        /// </summary>
        /// <param name="startLocation">Start of rectangle for selection within the scene.</param>
        /// <param name="endLocation">End of rectangle for selection within the scene.</param>
        /// <param name="units">Output list for matched units.</param>
        /// <param name="buildings">Output list for matched buildings.</param>
        /// <param name="limit">True if this should respect UnitSelection's limit of selection amount.</param>
        public void SelectionFind(
            Vector3 startLocation, Vector3 endLocation, List<GameObject> units, List<GameObject> buildings, bool limit)
        {
            Rect selectionRect = MapRectangle(startLocation, endLocation);
            int selectedAmount = 0;
            for (int x = 0; x < unitListLength; x++)
            {
                var unit = unitList[x];
                if (unit == null)
                {
                    Debug.LogWarning("A unit was not safely removed from UnitSelection script");
                    unitList.RemoveAt(x);
                    x--;
                    unitListLength--;
                    continue;
                }
                if (!WithinRectangleBounds(unit, selectionRect) || limit && selectedAmount >= maxUnitSelect)
                {
                    continue;
                }
                selectedAmount++;
                units.Add(unit);
            }
            selectedAmount = 0;
            for (int x = 0; x < buildingListLength; x++)
            {
                var building = buildingList[x];
                if (building == null)
                {
                    Debug.LogWarning("A building was not safely removed from UnitSelection script");
                    buildingList.RemoveAt(x);
                    x--;
                    buildingListLength--;
                    continue;
                }
                if (!WithinRectangleBounds(building, selectionRect) || limit && selectedAmount >= maxBuildingSelect)
                {
                    continue;
                }
                buildings.Add(building);
                selectedAmount++;
            }
        }

        private static Rect MapRectangle(Vector3 startLocation, Vector3 endLocation)
        {
            Rect selectionRect = startLocation.x > endLocation.x
                ? new Rect(endLocation.x, 0, startLocation.x, 0)
                : new Rect(startLocation.x, 0, endLocation.x, 0);
            selectionRect = startLocation.z > endLocation.z
                ? new Rect(selectionRect.x, endLocation.z, selectionRect.width, startLocation.z)
                : new Rect(selectionRect.x, startLocation.z, selectionRect.width, endLocation.z);
            return selectionRect;
        }

        public static bool WithinRectangleBounds(GameObject unit, Rect rectangle)
        {
            return unit.transform.position.x > rectangle.x
                   && unit.transform.position.x < rectangle.width
                   && unit.transform.position.z > rectangle.y
                   && unit.transform.position.z < rectangle.height;
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
                    curBuildSelectedS[x].Select(false);
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
                    curBuildSelectedS[x].Select(false);
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
                cont.Select(true);
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
                    buildingList[x].GetComponent<BuildingController>().Select(true);
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
}
