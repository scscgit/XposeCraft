using UnityEngine;

public class ResourceDropOff : MonoBehaviour
{
    public bool[] type;

    void Start()
    {
        GameObject.Find("Player Manager").GetComponent<ResourceManager>().AddDropOff(gameObject, type);
    }
}
