using System;
using System.Collections.Generic;
using UnityEngine;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Core.Grids;
using XposeCraft.Core.Required;
using XposeCraft.Core.Resources;
using XposeCraft.Game;
using XposeCraft.Game.Actors;
using XposeCraft.GameInternal;
using XposeCraft.GameInternal.Helpers;

namespace XposeCraft.Core.Faction.Buildings
{
    public class BuildingPlacement : MonoBehaviour
    {
        private static ResourceManager ResourceManagerPlayerOnly
        {
            get { return GameManager.Instance.CurrentPlayerResourceManager; }
        }

        UnitSelection unitSelect;
        public bool place { get; set; }
        Building build;
        GameObject obj;
        UGrid uGrid;
        public int gridI;
        int loc;
        int factionIndex;
        public bool placed { get; set; }

        public void Start()
        {
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
                TryPlaceBuild();
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
                if (nBuild.cost[x] > 0 && nBuild.cost[x] > ResourceManagerPlayerOnly.resourceTypes[x].amount)
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
            obj = Instantiate(nBuild.tempObj, Vector3.zero, Quaternion.identity);
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

        public void TryPlaceBuild()
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
                if (build.cost[x] == 0 || ResourceManagerPlayerOnly.resourceTypes[x].amount >= build.cost[x])
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
            int i = uGrid.DetermineLocation(hit.point, gridI);
            if (!uGrid.IsValidLocation(i))
            {
                return;
            }
            if (uGrid.grids[gridI].points[i].state != 2)
            {
                loc = i;
                obj.transform.position = uGrid.grids[gridI].points[i].loc;
            }
            if (!Input.GetButtonDown("LMB"))
            {
                return;
            }
            try
            {
                Actor.Create<Game.Actors.Buildings.Building>(
                    BuildingHelper.DetermineBuildingType(BuildingHelper.PrefabToType(build.obj)),
                    PlaceProgressBuilding(
                        build,
                        unitSelect.curSelectedS,
                        factionIndex,
                        new Position(loc),
                        obj.transform.rotation,
                        ResourceManagerPlayerOnly),
                    GameManager.Instance.GuiPlayer);
            }
            catch (InvalidOperationException)
            {
                return;
            }
            if (!Input.GetButton("ContinuePlace"))
            {
                Destroy(obj);
                place = false;
                placed = true;
            }
        }

        public static GameObject PlaceProgressBuilding(Building building, List<UnitController> builderUnits,
            int factionIndex, Position position, Quaternion rotation, ResourceManager resourceManager)
        {
            Vector3 location = PositionHelper.PositionToLocation(position);
            try
            {
                BuildingHelper.CheckValidPlacement(building, position, location, false);
            }
            catch (Exception)
            {
                // Visualizing the error placement
                if (GameManager.Instance.Debug)
                {
                    building.ClosePoints(GameManager.Instance.Grid, position.PointLocation);
                }
                throw;
            }
            for (int x = 0; x < building.cost.Length; x++)
            {
                if (building.cost[x] > 0 && resourceManager.resourceTypes[x].amount < building.cost[x])
                {
                    throw new InvalidOperationException(
                        "Not enough resources for placing the " + building + " building");
                }
            }
            for (int x = 0; x < building.cost.Length; x++)
            {
                if (building.cost[x] > 0)
                {
                    resourceManager.resourceTypes[x].amount -= building.cost[x];
                }
            }
            GameObject buildingObject;
            if (building.autoBuild)
            {
                buildingObject = BuildingHelper.InstantiateProgressBuilding(
                        building, building.obj, factionIndex, position, rotation)
                    .gameObject;
            }
            else
            {
                buildingObject = BuildingHelper.InstantiateProgressBuilding(
                        building, building.progressObj, factionIndex, position, rotation)
                    .gameObject;
                UnitSelection.SetTarget(builderUnits, buildingObject, buildingObject.transform.position);
            }
            return buildingObject;
        }
    }
}
