using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

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
    int passes = 0;

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
            apath[x] = new APath();
            apath[x].gridScript = gridScript;
        }
        startedThreads = new bool[amountOfThreads];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        passes++;
        int startListAmount = listAmount;
        for (int x = 0; x < amountOfThreads; x++)
        {
            if (x < startListAmount)
            {
                if (!startedThreads[x])
                {
                    Vector3 loc = startList[x];
                    Vector3 loc1 = targetList[x];
                    apath[x].myPath = null;
                    apath[x].start = loc;
                    apath[x].gridI = indexList[x];
                    apath[x].end = loc1;
                    apath[x].generate = true;
                    apath[x].index = returnList[x].name;
                    ThreadPool.QueueUserWorkItem(new WaitCallback(apath[x].FindMTPath));
                    startedThreads[x] = true;
                }
            }
        }
        int y = 0;
        bool allDone = true;
        for (int x = 0; x < amountOfThreads; x++)
        {
            if (apath[x].generate)
            {
                allDone = false;
            }
        }
        if (allDone)
        {
            for (int x = 0; x < amountOfThreads; x++)
            {
                if (startedThreads[x] && !apath[x].generate)
                {
                    if (x < startListAmount)
                    {
                        returnList[y].GetComponent<UnitMovement>().myPath = apath[x].myPath;
                        returnList.RemoveAt(y);
                        targetList.RemoveAt(y);
                        indexList.RemoveAt(y);
                        startList.RemoveAt(y);

                        listAmount--;
                        startedThreads[x] = false;
                        y--;
                    }
                }
                y++;
            }
        }
    }

    public void RequestPath(Vector3 loc, Vector3 loc1, GameObject obj, int index)
    {
        startList.Add(loc);
        indexList.Add(index);
        targetList.Add(loc1);
        returnList.Add(obj);
        listAmount++;
    }
}
