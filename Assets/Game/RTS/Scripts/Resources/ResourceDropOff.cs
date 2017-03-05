using UnityEngine;

public class ResourceDropOff : MonoBehaviour
{
    public bool[] type = new bool[0];

    void Start()
    {
        GameObject.Find("Player Manager").GetComponent<ResourceManager>().AddDropOff(gameObject, type);
    }
}
