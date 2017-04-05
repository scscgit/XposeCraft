using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Core.Grids;

namespace XposeCraft.Core.Required
{
    public class AStarManager : MonoBehaviour
    {
        APath[] apath;
        List<Vector3> targetList;
        List<int> indexList;
        List<Vector3> startList;
        List<GameObject> returnList;
        int listAmount;
        public int amountOfThreads;
        bool[] startedThreads;

        void OnDrawGizmos()
        {
            if (gameObject.name != "A*")
            {
                gameObject.name = "A*";
            }
        }

        void Start()
        {
            //pathList = new UPath[amountOfThreads];
            targetList = new List<Vector3>();
            startList = new List<Vector3>();
            indexList = new List<int>();
            returnList = new List<GameObject>();
            apath = new APath[amountOfThreads];
            UGrid gridScript = GameObject.Find("UGrid").GetComponent<UGrid>();
            for (int x = 0; x < apath.Length; x++)
            {
                apath[x] = new APath {gridScript = gridScript};
            }
            startedThreads = new bool[amountOfThreads];
        }

        void FixedUpdate()
        {
            int startListAmount = listAmount;
            for (int x = 0; x < amountOfThreads; x++)
            {
                if (x < startListAmount && !startedThreads[x])
                {
                    Vector3 loc = startList[x];
                    Vector3 loc1 = targetList[x];
                    apath[x].myPath = null;
                    apath[x].start = loc;
                    apath[x].gridI = indexList[x];
                    apath[x].end = loc1;
                    apath[x].generate = true;
                    apath[x].index = returnList[x].name;
                    ThreadPool.QueueUserWorkItem(apath[x].FindMTPath);
                    startedThreads[x] = true;
                }
            }
            for (int x = 0; x < amountOfThreads; x++)
            {
                if (apath[x].generate)
                {
                    return;
                }
            }

            for (int x = 0, y = 0; x < amountOfThreads; x++, y++)
            {
                if (!startedThreads[x] || apath[x].generate || x >= startListAmount)
                {
                    continue;
                }
                returnList[y].GetComponent<UnitMovement>().SetPath(apath[x].myPath);
                returnList.RemoveAt(y);
                targetList.RemoveAt(y);
                indexList.RemoveAt(y);
                startList.RemoveAt(y);
                listAmount--;
                startedThreads[x] = false;
                y--;
            }
        }

        public void RequestPath(Vector3 loc, Vector3 loc1, GameObject obj, int index)
        {
            // If there was an old request for the same object, that is not yet being processed
            // (based on the maximum amount of active parallel threads), invalidate it to prevent duplicate pending
            // requests. This will both improve performance and prevent the need for synchronization.
            int previousRequestIndex = returnList.IndexOf(obj);
            if (previousRequestIndex > amountOfThreads)
            {
                returnList.RemoveAt(previousRequestIndex);
                targetList.RemoveAt(previousRequestIndex);
                indexList.RemoveAt(previousRequestIndex);
                startList.RemoveAt(previousRequestIndex);
                listAmount--;
            }

            startList.Add(loc);
            indexList.Add(index);
            targetList.Add(loc1);
            returnList.Add(obj);
            listAmount++;
        }
    }
}
