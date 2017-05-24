using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using XposeCraft.Core.Faction.Units;
using XposeCraft.GameInternal;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor.Events;

#endif

namespace XposeCraft.Core.Required
{
    public class AStarManager : MonoBehaviour
    {
        public class PathFoundArguments : Object
        {
            public APath APath;
            public GameObject GameObject;
        }

        [Serializable]
        public class PathFoundEvent : UnityEvent<PathFoundArguments>
        {
        }

        public const string ScriptName = "A*";

        APath[] apath;
        List<Vector3> targetList;
        List<int> indexList;
        List<Vector3> startList;
        List<GameObject> returnList;
        public List<PathFoundEvent> _finishActionList;
        int listAmount;
        public int amountOfThreads;
        bool[] startedThreads;

        void OnDrawGizmos()
        {
            if (gameObject.name != ScriptName)
            {
                gameObject.name = ScriptName;
            }
        }

        void Start()
        {
            //pathList = new UPath[amountOfThreads];
            targetList = new List<Vector3>();
            startList = new List<Vector3>();
            indexList = new List<int>();
            returnList = new List<GameObject>();
            _finishActionList = new List<PathFoundEvent>();
            apath = new APath[amountOfThreads];
            for (int x = 0; x < apath.Length; x++)
            {
                apath[x] = new APath {gridScript = GameManager.Instance.UGrid};
            }
            startedThreads = new bool[amountOfThreads];
        }

        void FixedUpdate()
        {
            int startListAmount = listAmount;
            for (int x = 0; x < amountOfThreads && x < startListAmount; x++)
            {
                if (startedThreads[x])
                {
                    continue;
                }
                // If the unit already died, skip the calculation
                if (returnList[x] == null)
                {
                    apath[x].generate = false;
                    startedThreads[x] = true;
                    continue;
                }
                apath[x].myPath = null;
                apath[x].start = startList[x];
                apath[x].gridI = indexList[x];
                apath[x].end = targetList[x];
                apath[x].generate = true;
                apath[x].index = returnList[x].name;
                apath[x].lastValidLocation = returnList[x].GetComponent<UnitMovement>().lastValidLocation;
                ThreadPool.QueueUserWorkItem(apath[x].FindMTPath);
                startedThreads[x] = true;
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
                // If the GameObject is null, the unit has died during calculation and the result should be discarded
                if (_finishActionList[y] != null && returnList[y] != null)
                {
                    _finishActionList[y].Invoke(new PathFoundArguments {APath = apath[x], GameObject = returnList[y]});
                }
                _finishActionList.RemoveAt(y);
                returnList.RemoveAt(y);
                targetList.RemoveAt(y);
                indexList.RemoveAt(y);
                startList.RemoveAt(y);
                listAmount--;
                startedThreads[x] = false;
                y--;
            }
        }

        private void SetPathToUnit(PathFoundArguments arguments)
        {
            arguments.GameObject.GetComponent<UnitMovement>().SetPath(arguments.APath.myPath);
        }

        public void RequestPathUnitMovement(Vector3 start, Vector3 target, GameObject obj, int gridIndex)
        {
            RequestPath(start, target, SetPathToUnit, obj, gridIndex);
        }

        public void RequestPath(
            Vector3 start, Vector3 target, UnityAction<PathFoundArguments> finishAction, GameObject obj, int gridIndex)
        {
            // If there is an old duplicate pending request for the same object, invalidate it to improve performance
            int previousRequestIndex = returnList.IndexOf(obj);
            if (previousRequestIndex >= 0)
            {
                if (previousRequestIndex < amountOfThreads && startedThreads[previousRequestIndex])
                {
                    _finishActionList[previousRequestIndex] = null;
                    returnList[previousRequestIndex] = null;
                }
                else
                {
                    _finishActionList.RemoveAt(previousRequestIndex);
                    returnList.RemoveAt(previousRequestIndex);
                    targetList.RemoveAt(previousRequestIndex);
                    indexList.RemoveAt(previousRequestIndex);
                    startList.RemoveAt(previousRequestIndex);
                    listAmount--;
                }
            }
            // Enqueue the new request
            var pathFoundEvent = new PathFoundEvent();
#if UNITY_EDITOR
            // When in the Editor, the listener is persisted during a hot-swap
            UnityEventTools.AddPersistentListener(pathFoundEvent, finishAction);
#else
            pathFoundEvent.AddListener(finishAction);
#endif
            _finishActionList.Add(pathFoundEvent);
            startList.Add(start);
            indexList.Add(gridIndex);
            targetList.Add(target);
            returnList.Add(obj);
            listAmount++;
        }
    }
}
