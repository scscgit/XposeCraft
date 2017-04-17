using UnityEngine;
using XposeCraft.Core.Grids;
using XposeCraft.Core.Required;

namespace XposeCraft.Core.Faction.Units
{
    public class UnitMovement : MonoBehaviour
    {
        public int speed;
        public int rotateSpeed;
        public Vector3 target;
        UGrid gridScript;
        public int gridI;
        public UPath myPath;
        public int depth;
        public bool pathComplete;
        int curPoint;
        AStarManager pathing;
        public float checkDist = 1;
        public int layer;
        Transform myTransform;

        private void Awake()
        {
            Physics.IgnoreLayerCollision(layer, layer, true);
            pathing = GameObject.Find("A*").GetComponent<AStarManager>();
            gridScript = GameObject.Find("UGrid").GetComponent<UGrid>();
            myTransform = GetComponent<Transform>();
        }

        void FixedUpdate()
        {
            if (pathComplete || myPath == null || myPath.list.Length <= 0)
            {
                return;
            }
            for (int x = 1; x < myPath.list.Length - 1; x++)
            {
                if (gridScript.grids[gridI].points[myPath.list[x]].state != 2)
                {
                    continue;
                }
                RequestPath(target);
                return;
            }
            var pointLoc = gridScript.grids[gridI].points[myPath.list[curPoint]].loc;
            float distFromPlace = (
                new Vector3(pointLoc.x, 0, pointLoc.z) - new Vector3(myTransform.position.x, 0, myTransform.position.z)
            ).sqrMagnitude;
            if (distFromPlace < checkDist)
            {
                curPoint++;
                if (curPoint == myPath.list.Length)
                {
                    pathComplete = true;
                    return;
                }
            }
            // Lerp Rotation
            Quaternion targetRotation = Quaternion.LookRotation(
                new Vector3(pointLoc.x, 0, pointLoc.z)
                - new Vector3(myTransform.position.x, 0, myTransform.position.z));
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
            myTransform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void RequestPath(Vector3 target)
        {
            myPath = null;
            pathing.RequestPath(myTransform.position, target, gameObject, gridI);
            pathComplete = false;
        }

        public void SetPath(UPath path)
        {
            myPath = path;
            curPoint = 0;
        }

        void OnDrawGizmosSelected()
        {
            if (myPath != null && myPath.list.Length > 0)
            {
                myPath.color = Color.green;
                myPath.DisplayPath(curPoint, gridScript.grids[gridI].points, gridScript.grids[gridI].nodeDist);
            }
        }
    }
}
