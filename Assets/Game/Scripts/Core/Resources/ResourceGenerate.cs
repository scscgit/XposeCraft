using System;
using UnityEngine;
using XposeCraft.Core.Required;

namespace XposeCraft.Core.Resources
{
    public class ResourceGenerate : MonoBehaviour
    {
        public ResourceG[] resource = new ResourceG[0];

        public void Start()
        {
            throw new NotImplementedException(
                typeof(ResourceGenerate) + " needs to be re-implemented to contain" +
                " faction index, which can be then used to lookup for ResourceManager");
//            ResourceManager rm = GameObject.Find("Player Manager").GetComponent<ResourceManager>();
//            for (int x = 0; x < resource.Length; x++)
//            {
//                resource[x].Start(x, rm);
//                StartCoroutine(resource[x].Generate());
//            }
        }
    }
}
