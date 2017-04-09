using System;
using System.Collections.Generic;
using UnityEngine;
using XposeCraft.Core.Required;

namespace XposeCraft.Core.Resources
{
    public class ResourceManager : MonoBehaviour
    {
        [Serializable]
        public class BoolList : List<bool>
        {
        }

        public ResourceType[] resourceTypes;
        public List<GameObject> dropOffPoints;
        public List<BoolList> dropOffTypes;
        public int dropOffAmount;

        public void AddDropOff(GameObject obj, BoolList type)
        {
            dropOffPoints.Add(obj);
            dropOffTypes.Add(type);
            dropOffAmount++;
        }

        public void RemoveDropOff(GameObject obj)
        {
            for (int x = 0; x < dropOffAmount; x++)
            {
                if (obj == dropOffPoints[x])
                {
                    dropOffPoints.RemoveAt(x);
                    dropOffTypes.RemoveAt(x);
                    dropOffAmount--;
                    //x--;
                    break;
                }
            }
        }

        public void FixedUpdate()
        {
            for (int x = 0; x < dropOffAmount; x++)
            {
                if (dropOffPoints[x] == null)
                {
                    dropOffPoints.RemoveAt(x);
                    dropOffTypes.RemoveAt(x);
                    x--;
                    dropOffAmount--;
                }
            }
        }
    }
}
