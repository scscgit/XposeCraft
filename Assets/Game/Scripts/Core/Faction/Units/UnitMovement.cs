using UnityEngine;
using XposeCraft.Core.Grids;
using XposeCraft.Core.Required;
using XposeCraft.GameInternal;

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
        public int lastValidLocation { get; set; }
        private int _movementStuckCounter;

        private void Awake()
        {
            Physics.IgnoreLayerCollision(layer, layer, true);
            pathing = GameObject.Find("A*").GetComponent<AStarManager>();
            gridScript = GameObject.Find("UGrid").GetComponent<UGrid>();
            myTransform = GetComponent<Transform>();
        }

        private void Start()
        {
            // Default starting position to prevent glitch about running over the entire map to position zero
            lastValidLocation = GameManager.Instance.UGrid.DetermineLocation(transform.position);
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
            var point = gridScript.grids[gridI].points[myPath.list[curPoint]];
            var pointLoc = point.loc;
            float distFromPlace = (
                new Vector3(pointLoc.x, 0, pointLoc.z) - new Vector3(myTransform.position.x, 0, myTransform.position.z)
            ).sqrMagnitude;
            if (distFromPlace < checkDist)
            {
                _movementStuckCounter = 0;
                if (point.children.Length > 0)
                {
                    lastValidLocation = myPath.list[curPoint];
                }
                curPoint++;
                if (curPoint == myPath.list.Length)
                {
                    pathComplete = true;
                    return;
                }
            }
            else if (++_movementStuckCounter > 1000)
            {
                // If stuck under cliff, un-stuck
                myTransform.position = point.loc;
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
            pathing.RequestPathUnitMovement(myTransform.position, target, gameObject, gridI);
            pathComplete = false;
        }

        public void SetPath(UPath path)
        {
            myPath = path;
            curPoint = 0;
            _movementStuckCounter = 0;
        }

        void OnDrawGizmosSelected()
        {
            if (myPath != null && myPath.list.Length > 0)
            {
                myPath.color = Color.green;
                var grid = gridScript.grids[gridI];
                myPath.DisplayPath(curPoint, grid.points, grid.nodeDist);
            }
        }
    }
}
