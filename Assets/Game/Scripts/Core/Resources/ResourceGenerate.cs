using UnityEngine;
using XposeCraft.Core.Required;

namespace XposeCraft.Core.Resources
{
    public class ResourceGenerate : MonoBehaviour
    {
        public ResourceG[] resource = new ResourceG[0];
        ResourceManager rm;

        public void Start()
        {
            rm = GameObject.Find("Player Manager").GetComponent<ResourceManager>();
            for (int x = 0; x < resource.Length; x++)
            {
                resource[x].Start(x, rm);
                StartCoroutine(resource[x].Generate());
            }
        }
    }
}
