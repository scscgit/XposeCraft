using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XposeCraft.Core.Faction.Buildings;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Core.Grids;
using XposeCraft.Core.Resources;
using XposeCraft.UI.MiniMap;
using Object = UnityEngine.Object;

namespace XposeCraft.Core.Required
{
    [Serializable]
    public class SWeapon
    {
        public bool fighterUnit = false;
        public float attackRate;
        public float attackRange;
        public int attackDamage;
        public float lookRange;
        public GameObject attackObj;
        float lastAttackTime;
        public Transform attackSphere;
        public Transform lookSphere;

        public void AttackObject(GameObject target, GameObject self, string type, UnitType selfType)
        {
            if (attackRate + lastAttackTime >= Time.time)
            {
                return;
            }
            switch (type)
            {
                case "Unit":
                    target.GetComponent<UnitController>().Damage(selfType, attackDamage);
                    break;
                case "Building":
                    target.GetComponent<BuildingController>().Damage(selfType, attackDamage);
                    break;
            }
            attackObj.transform.position = self.transform.position;
            attackObj.SetActive(true);
            attackObj.SendMessage("Attack", target, SendMessageOptions.DontRequireReceiver);
            lastAttackTime = Time.time;
        }

        public bool InRange(Vector3 target, Vector3 self)
        {
            return (self - target).magnitude < attackRange;
        }

        public void RangeSpheres(GameObject self)
        {
            GameObject obj;
            RangeSignal sig;
            if (attackSphere == null)
            {
                obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                obj.name = "Attack Range";
                obj.GetComponent<SphereCollider>().isTrigger = true;
                obj.transform.parent = self.transform;
                obj.layer = 2;
                Object.DestroyImmediate(obj.GetComponent<MeshRenderer>());
                sig = obj.AddComponent<RangeSignal>();
                sig.cont = self.GetComponent<UnitController>();
                sig.type = 0;
                attackSphere = obj.transform;
                attackSphere.localScale = new Vector3(attackRange, attackRange, attackRange);
            }
            if (lookSphere != null)
            {
                return;
            }
            obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.name = "Look Range";
            obj.GetComponent<SphereCollider>().isTrigger = true;
            obj.transform.parent = self.transform;
            obj.layer = 2;
            Object.DestroyImmediate(obj.GetComponent<MeshRenderer>());

            lookSphere = obj.transform;
            lookSphere.localScale = new Vector3(lookRange, lookRange, lookRange);
            sig = obj.AddComponent<RangeSignal>();
            sig.type = 1;
            sig.cont = self.GetComponent<UnitController>();
        }

        public void CheckSpheres()
        {
            attackSphere.localScale = new Vector3(attackRange, attackRange, attackRange);
            attackSphere.localPosition = Vector3.zero;
            lookSphere.localScale = new Vector3(lookRange, lookRange, lookRange);
            lookSphere.localPosition = Vector3.zero;
        }
    }

    [Serializable]
    public class SAnimSounds
    {
        public Animator manager;
        string lastState = "";
        public string state;
        public AudioClip idleAudio;
        public AudioClip moveAudio;
        public AudioClip gatherAudio;
        public AudioClip buildAudio;
        public AudioClip attackAudio;
        public AudioSource source;
        public GameObject deathObject;

        private void SetAnimationState(int value)
        {
            if (manager && manager.runtimeAnimatorController != null)
            {
                manager.SetInteger("State", value);
            }
        }

        private void PlayAudio(AudioClip value)
        {
            if (gatherAudio)
            {
                source.clip = value;
                source.Play();
            }
        }

        public void Animate()
        {
            if (lastState == state)
            {
                return;
            }
            switch (state)
            {
                case "Gather":
                    SetAnimationState(2);
                    PlayAudio(gatherAudio);
                    break;
                case "Attack":
                    SetAnimationState(3);
                    PlayAudio(attackAudio);
                    break;
                case "Move":
                    SetAnimationState(1);
                    PlayAudio(moveAudio);
                    break;
                case "Build":
                    SetAnimationState(4);
                    PlayAudio(buildAudio);
                    break;
                case "Idle":
                    SetAnimationState(0);
                    PlayAudio(idleAudio);
                    break;
            }
            lastState = state;
        }

        public void Die(GameObject obj)
        {
            if (deathObject)
            {
                Object.Instantiate(deathObject, obj.transform.position, obj.transform.rotation);
            }
            Object.Destroy(obj);
        }
    }

    [Serializable]
    public class SResource
    {
        public bool resourceUnit;
        public ResourceBehaviour[] behaviour;
        public ResourceSource source;
        public int sourceIndex;
        public GameObject target;
        public ResourceManager manager;
        public GameObject nearestDropOff;

        public bool Gather(UnitController cont, GameObject obj)
        {
            if (manager == null)
            {
                manager = GameObject.Find("Player Manager").GetComponent<ResourceManager>();
            }
            if (!resourceUnit || source == null)
            {
                return false;
            }
            sourceIndex = source.resourceIndex;
            if (!behaviour[sourceIndex].canGather)
            {
                return false;
            }
            if (behaviour[sourceIndex].lastGather + behaviour[sourceIndex].rate >= Time.time)
            {
                return true;
            }
            int amount;
            behaviour[sourceIndex].lastGather = Time.time;
            if (behaviour[sourceIndex].returnWhenFull)
            {
                int drainAmount = 0;
                if (drainAmount <= behaviour[sourceIndex].carryCapacity - behaviour[sourceIndex].carrying)
                {
                    drainAmount = behaviour[sourceIndex].amount;
                }
                else
                {
                    drainAmount = behaviour[sourceIndex].carryCapacity - behaviour[sourceIndex].carrying;
                }
                amount = source.RequestResource(drainAmount);
                if (nearestDropOff == null)
                {
                    FindNearestDropOff(obj);
                }
                behaviour[sourceIndex].carrying += amount;
                if (source == null || behaviour[sourceIndex].carrying >= behaviour[sourceIndex].carryCapacity)
                {
                    ReturnToDropOff(cont);
                }
            }
            else
            {
                amount = source.RequestResource(behaviour[sourceIndex].amount);
                manager.resourceTypes[sourceIndex].amount += amount;
            }
            return amount > 0;
        }

        public void FindNearestDropOff(GameObject obj)
        {
            if (manager.dropOffAmount < 1)
            {
                return;
            }
            float closeDist = -1;
            int index = -1;
            for (int x = 0; x < manager.dropOffAmount; x++)
            {
                if (!manager.dropOffTypes[x][sourceIndex])
                {
                    continue;
                }
                float dist = (manager.dropOffPoints[x].transform.position - obj.transform.position).sqrMagnitude;
                if (!(dist < closeDist) && closeDist != -1)
                {
                    continue;
                }
                index = x;
                closeDist = dist;
            }
            nearestDropOff = index != -1 ? manager.dropOffPoints[index] : null;
        }

        public void ReturnToDropOff(UnitController cont)
        {
            if (nearestDropOff != null)
            {
                cont.SetTarget(nearestDropOff, nearestDropOff.transform.position, "DropOff");
            }
        }

        public void DropOff(UnitController cont)
        {
            if (target != null)
            {
                cont.SetTarget(target, target.transform.position, "Resource");
            }
            manager.resourceTypes[sourceIndex].amount += behaviour[sourceIndex].carrying;
            behaviour[sourceIndex].carrying = 0;
        }
    }

    [Serializable]
    public class ResourceBehaviour
    {
        public bool canGather;
        public int amount;
        public float rate;

        public float lastGather;

        //Optional Features

        public bool returnWhenFull;
        public int carrying;
        public int carryCapacity = 15;
    }


    [Serializable]
    public class SBuild
    {
        public bool builderUnit;
        public float buildDist = 3;
        public BuildBehaviour[] build = new BuildBehaviour[0];
        public BuildingController source;

        public bool Build()
        {
            if (!builderUnit || !build[source.buildIndex].canBuild)
            {
                return false;
            }
            if (build[source.buildIndex].lastBuild + build[source.buildIndex].rate >= Time.time)
            {
                return true;
            }
            bool returnVal = source.RequestBuild(build[source.buildIndex].amount);
            build[source.buildIndex].lastBuild = Time.time;
            return returnVal;
        }
    }

    [Serializable]
    public class BuildBehaviour
    {
        public bool canBuild;
        public float rate = 1;
        public int amount = 1;
        public float lastBuild;
    }

    [Serializable]
    public class SGUI
    {
        public GameObject[] selectObjs = new GameObject[0];
        public Texture2D image;
        bool lastState;
        public string type = "Unit";
        public UnitSelection select;
        public bool display = false;
        public bool onlyDisplayed = false;

        public void SetType(string ntype)
        {
            type = ntype == "Unit" ? "Unit" : "Building";
        }

        public void Start(GameObject obj)
        {
            select = GameObject.Find("Player Manager").GetComponent<UnitSelection>();
            switch (type)
            {
                case "Unit":
                    select.AddUnit(obj);
                    break;
                case "Building":
                    select.AddBuilding(obj);
                    break;
            }
        }

        public void Selected(bool state)
        {
            if (lastState != state)
            {
                foreach (var selectObj in selectObjs)
                {
                    selectObj.SetActive(state);
                }
            }
            lastState = state;
        }
    }

    [Serializable]
    public class SRatio
    {
        public string name;
        public float amount;
    }

    [Serializable]
    public class Seed
    {
        public GameObject obj;
        public int amount;
        public Rect area;
    }

    [Serializable]
    public class ProduceUnit
    {
        [FormerlySerializedAs("groupIndex")] public int UnitIndex;
        public bool canProduce = true;
        public int[] cost = new int[0];
        public Texture customTexture;
        public string customName = "Unit";
        public float dur = 5;
        public float rate = 1;
        public float amount = 1;
        public string description = "Description";
        float curDur;
        float lastTime;

        public ProduceUnit(ProduceUnit unit)
        {
            canProduce = unit.canProduce;
            cost = unit.cost;
            customTexture = unit.customTexture;
            customName = unit.customName;
            dur = unit.dur;
            rate = unit.rate;
            amount = unit.amount;
            curDur = 0;
            UnitIndex = unit.UnitIndex;
            lastTime = 0;
        }

        public ProduceUnit()
        {
        }

        public bool Produce()
        {
            if (lastTime + rate > Time.time)
            {
                return false;
            }
            lastTime = Time.time;
            curDur += amount;
            return curDur >= dur;
        }
    }

    [Serializable]
    public class ProduceTech
    {
        public int index;
        public string techName = "";
        public bool canProduce = true;
        public int[] cost = new int[0];
        public Texture customTexture;
        public string customName;
        public float dur = 10;
        public float rate = 5;
        public float amount;
        public string description = "Description";
        float curDur;
        float lastTime;

        public ProduceTech(ProduceTech tech)
        {
            canProduce = tech.canProduce;
            cost = tech.cost;
            customTexture = tech.customTexture;
            customName = tech.customName;
            dur = tech.dur;
            rate = tech.rate;
            amount = tech.amount;
            curDur = 0;
            index = tech.index;
            techName = tech.techName;
            lastTime = 0;
        }

        public ProduceTech()
        {
            customName = "";
        }

        public bool Produce()
        {
            if (lastTime + rate > Time.time)
            {
                return false;
            }
            lastTime = Time.time;
            curDur += amount;
            return curDur >= dur;
        }
    }

    [Serializable]
    public class STechBuilding
    {
        public bool canProduce = false;
        public ProduceTech[] techs;
        public List<ProduceTech> jobs = new List<ProduceTech>(0);
        public int jobsAmount;
        public int maxAmount = 10;
        public int canBuildAtOnce = 1;

        public void StartProduction(int x, ResourceManager resourceManager)
        {
            var job = new ProduceTech(techs[x]);
            // If there are available resources, uses them before starting the production, otherwise returns
            for (var index = 0; index < resourceManager.resourceTypes.Length; index++)
            {
                if (resourceManager.resourceTypes[index].amount < job.cost[index])
                {
                    return;
                }
            }
            for (var index = 0; index < resourceManager.resourceTypes.Length; index++)
            {
                resourceManager.resourceTypes[index].amount -= job.cost[index];
            }
            if (jobsAmount < maxAmount)
            {
                jobs.Add(job);
                jobsAmount++;
            }
        }

        public void CancelProduction(int x, Faction.Faction faction, ResourceManager resourceManager)
        {
            var job = jobs[x];
            // Returning the resource cost
            for (var index = 0; index < resourceManager.resourceTypes.Length; index++)
            {
                resourceManager.resourceTypes[index].amount += job.cost[index];
            }
            jobs.RemoveAt(x);
            jobsAmount--;
            faction.Tech[jobs[x].index].beingProduced = false;
        }

        public void Produce(Faction.Faction faction)
        {
            for (int x = 0; x < canBuildAtOnce; x++)
            {
                if (x < jobsAmount)
                {
                    faction.Tech[jobs[x].index].beingProduced = true;
                    if (jobs[x].Produce())
                    {
                        faction.Tech[jobs[x].index].active = true;
                        faction.Tech[jobs[x].index].Researched();
                        jobs.RemoveAt(x);
                        x--;
                        jobsAmount--;
                    }
                }
            }
        }
    }

    [Serializable]
    public class SBuildingGUI
    {
        public BGUISetting unitGUI = new BGUISetting
        {
            startPos = new Vector2(200, 50),
            buttonSize = new Vector2(150, 50)
        };

        public BGUISetting technologyGUI = new BGUISetting
        {
            startPos = new Vector2(350, 50),
            buttonSize = new Vector2(150, 50)
        };

        public BGUISetting jobsGUI = new BGUISetting
        {
            startPos = new Vector2(50, 50),
            buttonSize = new Vector2(150, 50)
        };
    }

    [Serializable]
    public class BGUISetting
    {
        public Vector2 startPos { get; set; }
        public Vector2 buttonSize { get; set; }
        public int buttonPerRow;
        public Vector2 displacement;
        public bool contains { get; set; }

        public bool Display(int x, int y, string text, Texture image, float ratioX, float ratioY)
        {
            Rect loc = new Rect(
                (startPos.x + (buttonSize.x + displacement.x) * x) * ratioX,
                (startPos.y + (buttonSize.y + displacement.y) * y) * ratioY,
                buttonSize.x * ratioX,
                buttonSize.y * ratioY);
            contains = loc.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y));
            return GUI.Button(loc, new GUIContent(text, image));
        }
    }

    [Serializable]
    public class SBuildingAnim
    {
        public Animator manager;
        string lastState = "";
        public string state;
        public AudioClip idleAudio;
        public AudioClip buildUnitAudio;
        public AudioClip buildTechAudio;
        public AudioSource source;
        public GameObject deathObject;

        public void Animate()
        {
            if (lastState == state)
            {
                return;
            }
            switch (state)
            {
                case "Idle":
                    if (manager)
                    {
                        manager.SetInteger("State", 0);
                    }
                    if (idleAudio)
                    {
                        source.clip = idleAudio;
                        source.Play();
                    }
                    break;
                case "Build Unit":
                    if (manager)
                    {
                        manager.SetInteger("State", 1);
                    }
                    if (buildUnitAudio)
                    {
                        source.clip = buildUnitAudio;
                        source.Play();
                    }
                    break;
                case "Build Tech":
                    if (manager)
                    {
                        manager.SetInteger("State", 2);
                    }
                    if (buildTechAudio)
                    {
                        source.clip = buildTechAudio;
                        source.Play();
                    }
                    break;
            }
            lastState = state;
        }

        public void Die(GameObject obj)
        {
            if (deathObject)
            {
                Object.Instantiate(deathObject, obj.transform.position, obj.transform.rotation);
            }
            Object.Destroy(obj);
        }
    }


    [Serializable]
    public class APath
    {
        public Vector3 start;
        public Vector3 end;
        public UGrid gridScript;
        public int gridI;
        public UPath myPath;
        public GridPoint[] mGrid;
        public string index = "";

        // TODO add in auto copy of the Grid Array
        public bool generate;

        // The Interior Implementation
        public void FindMTPath(object x)
        {
            try
            {
                if (mGrid == null)
                {
                    int l = gridScript.grids[gridI].points.Length;
                    mGrid = new GridPoint[l];
                    for (int z = 0; z < mGrid.Length; z++)
                    {
                        mGrid[z] = new GridPoint(gridScript.grids[gridI].points[z]);
                    }
                }
                else if (mGrid.Length == 0)
                {
                    int l = gridScript.grids[gridI].points.Length;
                    mGrid = new GridPoint[l];
                    for (int z = 0; z < mGrid.Length; z++)
                    {
                        mGrid[z] = new GridPoint(gridScript.grids[gridI].points[z]);
                    }
                }
                if (!generate)
                {
                    return;
                }
                myPath = FindPath(end, start);
            }
            catch (Exception e)
            {
                // Unity does not catch exceptions that occur in threads other than the main thread
                Debug.LogError(e);
                throw;
            }
            finally
            {
                generate = false;
            }
        }

        // The Vector3 based implementation
        public UPath FindPath(Vector3 startLoc, Vector3 endLoc)
        {
            Vector3 loc1 = gridScript.DetermineNearestPoint(startLoc, endLoc, 0);
            Vector3 loc2 = gridScript.DetermineNearestPoint(endLoc, startLoc, 0);
            int pointLoc1 = DetermineLoc(loc1);
            int pointLoc2 = DetermineLoc(loc2);
            return myPath = FindNormalPath(pointLoc1, pointLoc2);
        }

        // Finds the First Path
        public UPath FindFirstPath(Vector3 startLoc, Vector3 endLoc)
        {
            return FindFastPath(DetermineLoc(startLoc), DetermineLoc(endLoc));
        }

        // Finds a normal A* Path
        UPath FindNormalPath(int startLoc, int endLoc)
        {
            int[] gcostList = new int[gridScript.grids[gridI].points.Length];
            bool[] checkedList = new bool[gridScript.grids[gridI].points.Length];
            bool[] addedList = new bool[gridScript.grids[gridI].points.Length];
            for (int x = 0; x < mGrid.Length; x++)
            {
                mGrid[x].state = gridScript.grids[gridI].points[x].state;
            }

            BinaryHeap openList = new BinaryHeap();
            openList.Add(startLoc, 0);
            int oLLength = 1;
            checkedList[startLoc] = true;
            UPath mp = null;
            int g_cost;
            int h_cost;
            int f_cost;
            int point;
            while (oLLength > 0)
            {
                point = openList.binaryHeap[0].index;
                checkedList[point] = true;
                openList.Remove();
                if (point == endLoc)
                {
                    UPath lp = BackTrack(startLoc, endLoc, mGrid);
                    if (lp != null)
                    {
                        mp = lp;
                        break;
                    }
                }
                oLLength--;
                for (var x = 0; x < mGrid[point].children.Length; x++)
                {
                    int lPoint = mGrid[point].children[x];
                    if (checkedList[lPoint] || mGrid[lPoint].state == 2)
                    {
                        continue;
                    }
                    g_cost = (int) ((mGrid[lPoint].loc - mGrid[point].loc).sqrMagnitude + gcostList[point]);
                    if (addedList[lPoint] && gcostList[lPoint] <= g_cost)
                    {
                        continue;
                    }
                    h_cost = (int) ((mGrid[lPoint].loc - mGrid[endLoc].loc).sqrMagnitude);
                    f_cost = g_cost + h_cost;
                    mGrid[lPoint].parent = point;
                    gcostList[lPoint] = g_cost;
                    if (addedList[lPoint])
                    {
                        continue;
                    }
                    addedList[lPoint] = true;
                    openList.Add(lPoint, f_cost);
                    oLLength++;
                }
            }
            return mp;
        }

        // Finds the first Path
        UPath FindFastPath(int startLoc, int endLoc)
        {
            float[] fcostList = new float[gridScript.grids[gridI].points.Length];
            float[] gcostList = new float[gridScript.grids[gridI].points.Length];
            bool[] checkedList = new bool[gridScript.grids[gridI].points.Length];
            bool[] addedList = new bool[gridScript.grids[gridI].points.Length];
            int l = gridScript.grids[gridI].points.Length;
            GridPoint[] lGrid = new GridPoint[l];
            for (int x = 0; x < lGrid.Length; x++)
            {
                lGrid[x] = new GridPoint(gridScript.grids[gridI].points[x]);
            }
            List<int> openList = new List<int> {startLoc};
            int oLLength = 1;
            checkedList[openList[0]] = true;
            UPath mp = null;
            while (oLLength > 0)
            {
                int point = openList[0];
                checkedList[point] = true;
                openList.RemoveAt(0);
                if (point == endLoc)
                {
                    UPath lp = BackTrack(startLoc, endLoc, lGrid);
                    if (lp != null)
                    {
                        mp = lp;
                        break;
                    }
                }
                oLLength--;
                foreach (int lPoint in lGrid[point].children)
                {
                    if (checkedList[lPoint] || lGrid[lPoint].state == 2)
                    {
                        continue;
                    }
                    float g_cost = (lGrid[lPoint].loc - lGrid[point].loc).sqrMagnitude + gcostList[point];
                    float h_cost = (lGrid[lPoint].loc - lGrid[endLoc].loc).sqrMagnitude;
                    float f_cost = g_cost + h_cost;
                    if (addedList[lPoint] && !(fcostList[lPoint] > f_cost))
                    {
                        continue;
                    }
                    lGrid[lPoint].parent = point;
                    fcostList[lPoint] = f_cost;
                    gcostList[lPoint] = g_cost;
                    if (addedList[lPoint])
                    {
                        continue;
                    }
                    addedList[lPoint] = true;
                    openList.Add(lPoint);
                    oLLength++;
                }
            }
            return mp;
        }

        // Finds the first Path
        UPath FindFastMTPath(int startLoc, int endLoc)
        {
            float[] fcostList = new float[gridScript.grids[gridI].points.Length];
            float[] gcostList = new float[gridScript.grids[gridI].points.Length];
            bool[] checkedList = new bool[gridScript.grids[gridI].points.Length];
            bool[] addedList = new bool[gridScript.grids[gridI].points.Length];
            for (int x = 0; x < mGrid.Length; x++)
            {
                mGrid[x].state = gridScript.grids[gridI].points[x].state;
            }
            List<int> openList = new List<int> {startLoc};
            int oLLength = 1;
            checkedList[openList[0]] = true;
            UPath mp = null;
            while (oLLength > 0)
            {
                int point = openList[0];
                checkedList[point] = true;
                openList.RemoveAt(0);
                if (point == endLoc)
                {
                    UPath lp = BackTrack(startLoc, endLoc, mGrid);
                    if (lp != null)
                    {
                        mp = lp;
                        break;
                    }
                }
                oLLength--;
                for (var x = 0; x < mGrid[point].children.Length; x++)
                {
                    int lPoint = mGrid[point].children[x];
                    if (lPoint != endLoc && (checkedList[lPoint] || mGrid[lPoint].state == 2))
                    {
                        continue;
                    }
                    float g_cost = (new Vector3(mGrid[lPoint].loc.x, 0, mGrid[lPoint].loc.z)
                                    - new Vector3(mGrid[point].loc.x, 0, mGrid[point].loc.z)
                                   ).sqrMagnitude + gcostList[point];
                    float h_cost = (new Vector3(mGrid[lPoint].loc.x, 0, mGrid[lPoint].loc.z)
                                    - new Vector3(mGrid[endLoc].loc.x, 0, mGrid[endLoc].loc.z)).sqrMagnitude;
                    float f_cost = g_cost + h_cost;
                    if (addedList[lPoint] && !(fcostList[lPoint] > f_cost))
                    {
                        continue;
                    }
                    mGrid[lPoint].parent = point;
                    fcostList[lPoint] = f_cost;
                    gcostList[lPoint] = g_cost;
                    if (addedList[lPoint])
                    {
                        continue;
                    }
                    addedList[lPoint] = true;
                    openList.Add(lPoint);
                    oLLength++;
                }
            }
            return mp;
        }

        UPath BackTrack(int startLoc, int endLoc, int[] lGridLookup, GridPoint[] lGrid)
        {
            int pathSize = 1;
            int loc = endLoc;
            while (loc != startLoc)
            {
                loc = lGrid[lGridLookup[loc]].parent;
                pathSize++;
            }
            UPath mp = new UPath {list = new int[pathSize]};
            loc = endLoc;
            for (int x = pathSize - 1; x >= 0; x--)
            {
                mp.list[x] = loc;
                loc = lGrid[lGridLookup[loc]].parent;
            }
            return mp;
        }

        UPath BackTrack(int startLoc, int endLoc, GridPoint[] lGrid)
        {
            int pathSize = 1;
            int loc = endLoc;
            while (loc != startLoc)
            {
                loc = lGrid[loc].parent;
                pathSize++;
            }
            UPath mp = new UPath {list = new int[pathSize]};
            loc = endLoc;
            for (int x = pathSize - 1; x >= 0; x--)
            {
                mp.list[x] = loc;
                loc = lGrid[loc].parent;
            }
            return mp;
        }

        int DetermineLoc(Vector3 loc)
        {
            float xLoc = loc.x - gridScript.grids[gridI].startLoc.x;
            float yLoc = loc.z - gridScript.grids[gridI].startLoc.z;
            int x = Mathf.RoundToInt(xLoc / gridScript.grids[gridI].nodeDist);
            int y = Mathf.RoundToInt(yLoc / gridScript.grids[gridI].nodeDist);
            return x + y * gridScript.grids[gridI].size;
        }

        int DetermineSector(int loc, int gridSize, int sectorSize)
        {
            gridSize = (int) Mathf.Sqrt(gridSize);
            int x = (loc - (loc / gridSize * gridSize)) / sectorSize;
            int size = gridSize / sectorSize;
            int y = loc / gridSize / sectorSize;
            return x + y * size;
        }
    }

    [Serializable]
    public class UPath
    {
        public int[] list = new int[0];

        // TODO Implement functionality for Cost
        public float cost = 0;

        public Color color = Color.white;
        public bool displayPath = false;

        public void DisplayPath(int loc, GridPoint[] grid, float nodeDist)
        {
            for (int x = loc; x < list.Length; x++)
            {
                Gizmos.color = color;
                Vector3 l = grid[list[x]].loc;
                Gizmos.DrawCube(new Vector3(l.x, l.y + 1, l.z), new Vector3(nodeDist, nodeDist, nodeDist));
            }
        }

        // Add Path list values to the beginning of the current list
        public void Shift(UPath oPath)
        {
            int[] oList = new int[list.Length];
            for (int x = 0; x < list.Length; x++)
            {
                oList[x] = list[x];
            }
            list = new int[list.Length + oPath.list.Length];
            int y = 0;
            for (int x = 0; x < oPath.list.Length; x++)
            {
                list[y] = oPath.list[x];
                y++;
            }
            for (int x = 0; x < oList.Length; x++)
            {
                list[y] = oList[x];
                y++;
            }
        }

        // Add Path list values to the end of the current list
        public void Add(UPath oPath)
        {
            int[] oList = new int[list.Length];
            for (int x = 0; x < list.Length; x++)
            {
                oList[x] = list[x];
            }
            list = new int[list.Length + oPath.list.Length];
            int y = 0;
            for (int x = 0; x < oList.Length; x++)
            {
                list[y] = oList[x];
                y++;
            }
            for (int x = 0; x < oPath.list.Length; x++)
            {
                list[y] = oPath.list[x];
                y++;
            }
        }
    }


    [Serializable]
    public class UnitType
    {
        public string name = "Name";
        public Ratio[] strengths = new Ratio[0];
        public Ratio[] weaknesses = new Ratio[0];
    }

    [Serializable]
    public class Ratio
    {
        public string name = "Name";
        public int target = 0;
        public string targetName = "";
        public float amount = 1;
    }

    [Serializable]
    public class GUIElement
    {
        public Rect loc;
        public Texture2D texture;
        public Type type;
        public bool allowClickThrough = false;

        public enum Type
        {
            Texture
        }

        public void Display(Vector2 ratio)
        {
            if (type == Type.Texture && texture)
            {
                GUI.DrawTexture(
                    new Rect(loc.x * ratio.x, loc.y * ratio.y, loc.width * ratio.x, loc.height * ratio.y),
                    texture);
            }
        }
    }

    [Serializable]
    public class MiniMapElement
    {
        public Vector2 size;
        public Texture image;
        public string tag;
        public Color[] tints = {Color.white};
        public bool factionElement = false;
        public bool moveUp;
        public List<GameObject> elementObj { get; set; }
        public List<Transform> elementTransform { get; set; }
        public List<MiniMapSignal> elementMap { get; set; }
        public List<Vector2> elementLoc { get; set; }
        public List<int> elementFaction { get; set; }
        public int objAmount { get; set; }

        public void AddElement(GameObject obj, string tag, MiniMapSignal map, int faction, Vector2 loc)
        {
            elementObj.Add(obj);
            elementMap.Add(map);
            elementTransform.Add(obj.transform);
            elementLoc.Add(loc);
            elementFaction.Add(faction);
            objAmount++;
        }

        public void ModifyLoc(int index, Vector2 loc)
        {
            elementLoc[index] = loc;
        }
    }


    [Serializable]
    public class ResourceG
    {
        public int amount = 0;
        public float rate = 0;
        public int index;
        ResourceManager rm;

        public void Start(int i, ResourceManager r)
        {
            rm = r;
            index = i;
        }

        public IEnumerator Generate()
        {
            while (true)
            {
                yield return new WaitForSeconds(rate);
                rm.resourceTypes[index].amount += amount;
            }
        }
    }

    [Serializable]
    public class ResourceType
    {
        public string name;
        public int amount;
        public Texture texture;
    }

    [Serializable]
    public class CursorType
    {
        public Texture2D texture;
        public string tag;
        public bool moveUp;
    }

    [Serializable]
    public class Relation
    {
        public int state = 3;
    }

    [Serializable]
    public class FGUI
    {
        Texture2D factionIcon;
    }

    [Serializable]
    public class Unit
    {
        public GameObject obj;
        public int[] cost = new int[0];
        public bool[] technologyEffect;
        public GameObject[] technologyUpgrade;
        public bool available = false;
    }

    [Serializable]
    public class Building
    {
        public bool autoBuild = false;
        public GameObject tempObj = null;
        public GameObject progressObj = null;
        public GameObject obj = null;
        public int[] cost = new int[0];
        public bool available = true;

        //TODO add comment

        public int closeWidth = 0;
        public int closeLength = 0;
        public int[] closePoints = new int[1];
        public string description = "Description";

        public void ClosePoints(Grid grid, int index)
        {
            for (int x = -closeWidth; x <= closeWidth; x++)
            {
                for (int y = -closeLength; y <= closeLength; y++)
                {
                    int i = x + y * grid.size;
                    grid.points[index + i].state =
                        closePoints[(x + closeWidth) * (closeLength * 2 + 1) + (y + closeLength)];
                }
            }
        }

        public void OpenPoints(UGrid grid, int gridI, int index)
        {
            for (int x = -closeWidth; x <= closeWidth; x++)
            {
                for (int y = -closeLength; y <= closeLength; y++)
                {
                    int i = x + y * grid.grids[gridI].size;
                    grid.grids[gridI].points[index + i].state = 0;
                }
            }
        }

        public bool CheckPoints(Grid grid, int index)
        {
            bool state = true;
            for (int x = -closeWidth; x <= closeWidth; x++)
            {
                for (int y = -closeLength; y <= closeLength; y++)
                {
                    if (closePoints[(x + closeWidth) * closeLength + (y + closeLength)] != 0)
                    {
                        if (grid.points[index + x + y * grid.size].state == 2)
                        {
                            state = false;
                        }
                    }
                }
            }
            return state;
        }

        int DetermineSector(int loc, int gridSize, int sectorSize)
        {
            gridSize = (int) Mathf.Sqrt(gridSize);
            int x = (loc - (loc / gridSize * gridSize)) / sectorSize;
            int size = gridSize / sectorSize;
            int y = loc / gridSize / sectorSize;
            return x + y * size;
        }
    }

    [Serializable]
    public class Technology
    {
        public string name = "";
        public Texture texture;
        public bool active;
        public bool beingProduced;
        public bool backTrackUpdate = true;
        List<GameObject> childListeners = new List<GameObject>(0);
        int childListenersLength;

        public void Researched()
        {
            for (int x = 0; x < childListenersLength; x++)
            {
                if (childListeners[x])
                {
                    childListeners[x].SendMessage("Upgraded", name);
                }
            }
        }

        public void AddListener(GameObject obj)
        {
            childListeners.Add(obj);
            childListenersLength++;
        }
    }

    [Serializable]
    public class TechEffect
    {
        public string name;
        public int index;
        public GameObject replacementObject;
        public Effects[] effects = new Effects[0];

        public void Replace(GameObject obj)
        {
            if (replacementObject)
            {
                UnitController script = obj.GetComponent<UnitController>();
                GameObject rObj =
                    Object.Instantiate(replacementObject, obj.transform.position, obj.transform.rotation) as GameObject;
                UnitController rScript = rObj.GetComponent<UnitController>();
                if (rScript)
                {
                    rScript.FactionIndex = script.FactionIndex;
                }
                Object.Destroy(obj);
            }
            else
            {
                foreach (Effects eff in effects)
                {
                    //Units
                    if (obj.GetComponent<UnitController>() != null)
                    {
                        switch (eff.effectName)
                        {
                            // String
                            case 0:
                            case 21:
                                obj.SendMessage(eff.funcName, eff.text);
                                break;
                            // Boolean
                            case 4:
                            case 11:
                            case 15:
                                obj.SendMessage(eff.funcName, eff.toggle);
                                break;
                            // Boolean Index
                            case 12:
                            case 16:
                                obj.SendMessage("SetIndex", eff.index);
                                obj.SendMessage(eff.funcName, eff.toggle);
                                break;
                            // Int Index
                            case 13:
                            case 14:
                            case 17:
                            case 18:
                            case 23:
                                obj.SendMessage("SetIndex", eff.index);
                                obj.SendMessage(eff.funcName, eff.amount);
                                break;
                            // Int
                            default:
                                obj.SendMessage(eff.funcName, eff.amount);
                                break;
                        }
                    }
                    //Buildings
                    else
                    {
                        switch (eff.effectName)
                        {
                            // String
                            case 0:
                                obj.SendMessage(eff.funcName, eff.text);
                                break;
                            // Boolean
                            case 4:
                            case 11:
                                obj.SendMessage(eff.funcName, eff.toggle);
                                break;
                            // Boolean Index
                            case 5:
                            case 13:
                                obj.SendMessage("SetIndex", eff.index);
                                obj.SendMessage(eff.funcName, eff.toggle);
                                break;
                            // Int Index
                            case 7:
                            case 8:
                            case 14:
                            case 15:
                                obj.SendMessage("SetIndex", eff.index);
                                obj.SendMessage(eff.funcName, eff.amount);
                                break;
                            // Int Index Index1
                            case 6:
                            case 12:
                                obj.SendMessage("SetIndex", eff.index);
                                obj.SendMessage("SetIndex1", eff.index1);
                                obj.SendMessage(eff.funcName, eff.amount);
                                break;
                            // Int
                            default:
                                obj.SendMessage(eff.funcName, eff.amount);
                                break;
                        }
                    }
                }
            }
        }
    }

    [Serializable]
    public class Effects
    {
        public int effectName = 0;
        public string funcName = "Function";
        public int funcType = 0;
        public int type = 0;
        public float amount = 0;
        public bool toggle = false;
        public int index = 0;
        public int index1 = 0;
        public string text = "";
    }

    [Serializable]
    public class SUnitBuilding
    {
        public bool canProduce = false;
        public ProduceUnit[] units = new ProduceUnit[0];
        public List<ProduceUnit> jobs = new List<ProduceUnit>(0);
        public int jobsAmount;
        public int canBuildAtOnce = 1;
        public int maxAmount = 10;
        public GameObject buildLoc;
        public int productionDistance = 6;
        int curLoc;

        public void StartProduction(int x, ResourceManager resourceManager)
        {
            var job = new ProduceUnit(units[x]);
            // If there are available resources, uses them before starting the production, otherwise returns
            for (var index = 0; index < resourceManager.resourceTypes.Length; index++)
            {
                if (resourceManager.resourceTypes[index].amount < job.cost[index])
                {
                    return;
                }
            }
            for (var index = 0; index < resourceManager.resourceTypes.Length; index++)
            {
                resourceManager.resourceTypes[index].amount -= job.cost[index];
            }
            if (jobsAmount < maxAmount)
            {
                jobs.Add(job);
                jobsAmount++;
            }
        }

        public void CancelProduction(int x, ResourceManager resourceManager)
        {
            var job = jobs[x];
            // Returning the resource cost
            for (var index = 0; index < resourceManager.resourceTypes.Length; index++)
            {
                resourceManager.resourceTypes[index].amount += job.cost[index];
            }
            jobs.RemoveAt(x);
            jobsAmount--;
        }

        public void Produce(Faction.Faction faction, int factionIndex, UGrid grid, int gridI)
        {
            for (int x = 0; x < canBuildAtOnce; x++)
            {
                if (x >= jobsAmount || !jobs[x].Produce())
                {
                    continue;
                }
                Vector3 loc =
                    grid.DetermineNearestPoint(buildLoc.transform.position, buildLoc.transform.position, gridI);
                GameObject obj =
                    Object.Instantiate(faction.UnitList[jobs[x].UnitIndex].obj, loc, Quaternion.identity) as GameObject;
                UnitController script = obj.GetComponent<UnitController>();
                if (script)
                {
                    script.FactionIndex = factionIndex;
                }
                jobs.RemoveAt(x);
                x--;
                jobsAmount--;
            }
        }
    }

    [Serializable]
    public class HeapElement
    {
        public int index;
        public int cost;

        public static bool operator <(HeapElement arg1, HeapElement arg2)
        {
            return arg1.cost < arg2.cost;
        }

        public static bool operator >(HeapElement arg1, HeapElement arg2)
        {
            return arg1.cost > arg2.cost;
        }

        public static bool operator >=(HeapElement arg1, HeapElement arg2)
        {
            return arg1.cost >= arg2.cost;
        }

        public static bool operator <=(HeapElement arg1, HeapElement arg2)
        {
            return arg1.cost <= arg2.cost;
        }

        public void Equals(HeapElement arg1)
        {
            cost = arg1.cost;
            index = arg1.index;
        }
    }

    [Serializable]
    public class BinaryHeap
    {
        public List<HeapElement> binaryHeap = new List<HeapElement>(0);
        public int numberOfItems;

        public void Add(int index, int fCost)
        {
            binaryHeap.Add(new HeapElement());
            binaryHeap[numberOfItems].cost = fCost;
            binaryHeap[numberOfItems].index = index;

            int bubbleIndex = numberOfItems;
            while (bubbleIndex != 0)
            {
                int parentIndex = (bubbleIndex - 1) / 2;
                if (binaryHeap[bubbleIndex] <= binaryHeap[parentIndex])
                {
                    int tmpIndex = binaryHeap[parentIndex].index;
                    int tmpValue = binaryHeap[parentIndex].cost;
                    binaryHeap[parentIndex].Equals(binaryHeap[bubbleIndex]);
                    binaryHeap[bubbleIndex].cost = tmpValue;
                    binaryHeap[bubbleIndex].index = tmpIndex;
                    bubbleIndex = parentIndex;
                }
                else
                {
                    break;
                }
            }
            numberOfItems++;
        }

        public void Recalculate(int index)
        {
            int bubbleIndex = index;
            while (bubbleIndex != 1)
            {
                int parentIndex = bubbleIndex / 2;
                if (binaryHeap[bubbleIndex] <= binaryHeap[parentIndex])
                {
                    int tmpIndex = binaryHeap[parentIndex].index;
                    int tmpValue = binaryHeap[parentIndex].cost;
                    binaryHeap[parentIndex].Equals(binaryHeap[bubbleIndex]);
                    binaryHeap[bubbleIndex].cost = tmpValue;
                    binaryHeap[bubbleIndex].index = tmpIndex;
                    bubbleIndex = parentIndex;
                }
                else
                {
                    break;
                }
            }
        }

        public void Remove()
        {
            numberOfItems--;
            binaryHeap[0].Equals(binaryHeap[numberOfItems]);

            int swap = 0;
            int parent;
            do
            {
                parent = swap;
                if (2 * parent + 2 <= numberOfItems)
                {
                    if (binaryHeap[parent] >= binaryHeap[2 * parent + 1])
                    {
                        swap = 2 * parent + 1;
                    }
                    if (binaryHeap[swap] >= binaryHeap[2 * parent + 2])
                    {
                        swap = 2 * parent + 2;
                    }
                }
                else if (2 * parent + 1 <= numberOfItems)
                {
                    // Only one child exists
                    if (binaryHeap[parent] >= binaryHeap[2 * parent + 1])
                    {
                        swap = 2 * parent + 1;
                    }
                }
                // One if the parent's children are smaller or equal, swap them
                if (parent != swap)
                {
                    int tmpCost = binaryHeap[parent].cost;
                    int tmpIndex = binaryHeap[parent].index;
                    binaryHeap[parent].Equals(binaryHeap[swap]);
                    binaryHeap[swap].cost = tmpCost;
                    binaryHeap[swap].index = tmpIndex;
                }
            }
            while (parent != swap);
            binaryHeap.RemoveAt(numberOfItems);
        }
    }
}
