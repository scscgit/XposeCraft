using UnityEngine;
using XposeCraft.Core.Grids;
using XposeCraft.GameInternal;

namespace XposeCraft.Core.Resources
{
    [SelectionBase]
    public class ResourceSource : MonoBehaviour
    {
        public int resourceIndex;
        public int amount;
        public bool deleteWhenExhausted;
        public int closeSize = 1;
        public int gridI;

        public void Start()
        {
            ClosePoints();
        }

        public int RequestResource(int rAmount)
        {
            if (amount >= rAmount)
            {
                amount = amount - rAmount;
                return rAmount;
            }
            amount = 0;
            if (!deleteWhenExhausted)
            {
                return amount;
            }
            OpenPoints();
            Destroy(gameObject);
            var sharedResources = GameManager.Instance.Players[0].Resources;
            for (var playerResoureIndex = 0; playerResoureIndex < sharedResources.Count; playerResoureIndex++)
            {
                if (GameManager.Instance.ActorLookup[gameObject].Equals(sharedResources[playerResoureIndex]))
                {
                    sharedResources.RemoveAt(playerResoureIndex);
                    return amount;
                }
            }
            Log.w("Resource " + gameObject.name + " did not have Actor representation but was removed");
            return amount;
        }

        public void OpenPoints()
        {
            UGrid uGrid = GameObject.Find("UGrid").GetComponent<UGrid>();
            int index = DetermineLoc(transform.position, uGrid);
            Grid grid = uGrid.grids[gridI];
            grid.points[index].state = 0;
            for (int x = -closeSize; x <= closeSize; x++)
            {
                for (int y = -closeSize; y <= closeSize; y++)
                {
                    int i = x + y * grid.size;
                    grid.points[index + i].state = 0;
                }
            }
        }

        public void ClosePoints()
        {
            UGrid uGrid = GameObject.Find("UGrid").GetComponent<UGrid>();
            int index = DetermineLoc(transform.position, uGrid);
            Grid grid = uGrid.grids[gridI];
            grid.points[index].state = 2;
            for (int x = -closeSize; x <= closeSize; x++)
            {
                for (int y = -closeSize; y <= closeSize; y++)
                {
                    int i = x + y * grid.size;
                    grid.points[index + i].state = 2;
                }
            }
        }

        int DetermineLoc(Vector3 loc, UGrid gridScript)
        {
            Grid grid = gridScript.grids[gridI];
            float xLoc = loc.x - grid.startLoc.x;
            float yLoc = loc.z - grid.startLoc.z;
            int x = Mathf.RoundToInt(xLoc / grid.nodeDist);
            int y = Mathf.RoundToInt(yLoc / grid.nodeDist);
            return x + y * grid.size;
        }
    }
}
