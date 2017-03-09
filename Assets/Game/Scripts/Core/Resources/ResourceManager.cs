using UnityEngine;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour
{
    public ResourceType[] resourceTypes;
    public List<GameObject> dropOffPoints;
    public List<bool[]> dropOffTypes = new List<bool[]>(0);
    public int dropOffAmount;

    public void AddDropOff(GameObject obj, bool[] type)
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
