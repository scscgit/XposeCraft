using UnityEngine;

namespace XposeCraft.Core.Misc
{
    public class DuplicateObject : MonoBehaviour
    {
        public GameObject obj;
        public Vector2 dispAmount;
        public int xMax;
        public int zMax;
        public string objName;
        public bool generate;

        void Update()
        {
            if (!generate)
            {
                return;
            }
            GameObject parent = new GameObject {name = "Parent"};
            int y = 0;
            for (int z = 0; z < zMax; z++)
            {
                for (int x = 0; x < xMax; x++)
                {
                    RaycastHit hit;
                    Physics.Raycast(
                        new Vector3(
                            gameObject.transform.position.x + x * dispAmount.x,
                            gameObject.transform.position.y + 100,
                            gameObject.transform.position.z + z * dispAmount.y),
                        Vector3.down,
                        out hit,
                        1000);
                    GameObject clone = Instantiate(
                        obj,
                        new Vector3(hit.point.x, hit.point.y + 1, hit.point.z),
                        Quaternion.identity) as GameObject;
                    clone.name = objName + " " + y;
                    clone.transform.parent = parent.transform;
                    y++;
                }
            }
            generate = false;
        }
    }
}
