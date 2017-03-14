using UnityEngine;

namespace XposeCraft.Core.Misc
{
    public class TerrainMatReplace : MonoBehaviour
    {
        public Material mat;

        void Start()
        {
            gameObject.GetComponent<Terrain>().materialTemplate = mat;
        }
    }
}
