using UnityEngine;
using UnityEngine.Serialization;
using XposeCraft.Core.Faction.Buildings;
using XposeCraft.Core.Fog_Of_War;
using XposeCraft.Core.Required;
using XposeCraft.Core.Resources;
using XposeCraft.Game;
using XposeCraft.GameInternal;
using XposeCraft.GameInternal.Helpers;
using XposeCraft.UI.MiniMap;
using EventType = XposeCraft.Game.Enums.EventType;
using Unit = XposeCraft.Game.Actors.Units.Unit;

namespace XposeCraft.Core.Faction.Units
{
    [SelectionBase]
    [RequireComponent(typeof(UnitMovement))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Rigidbody))]
    public class UnitController : MonoBehaviour
    {
        public Unit UnitActor;
        public new string name = "Unit";
        public int maxHealth = 100;
        public int health = 100;
        [FormerlySerializedAs("group")] public int FactionIndex;
        Faction faction;
        int index;
        public UnitType type;
        public SWeapon weapon = new SWeapon();
        public SBuild build = new SBuild();
        public SAnimSounds anim = new SAnimSounds();
        public SResource resource = new SResource();
        public SGUI gui = new SGUI();
        public SRatio[] ratio;
        public TechEffect[] techEffect = new TechEffect[0];
        UnityEngine.Resources[] cost;
        bool selected;
        public VisionSignal vision;
        public MiniMapSignal miniMap;
        public UnitMovement movement;
        UnitSelection selection;
        public GameObject target;
        public Vector3 targetPoint;
        public int size = 1;
        Health healthObj;

        public enum Target
        {
            Resource,
            Unit,
            Location,
            Build,
            DropOff,
            None
        }

        public Target targetType = Target.None;

        public TargetState tState = TargetState.Undecided;

        public enum TargetState
        {
            Ally,
            Neutral,
            Enemy,
            Self,
            Undecided
        }

        void OnMouseDown()
        {
            selection.AddSelectedUnit(gameObject);
        }

        void OnDrawGizmos()
        {
            if (anim.source == null || !anim.source)
            {
                anim.source = gameObject.GetComponent<AudioSource>();
                if (!anim.source)
                {
                    anim.source = gameObject.AddComponent<AudioSource>();
                    anim.source.loop = true;
                }
            }
            if (!weapon.fighterUnit)
            {
                return;
            }
            if (weapon.attackSphere == null || weapon.lookSphere == null)
            {
                weapon.RangeSpheres(gameObject);
            }
            weapon.CheckSpheres();
        }

        void Start()
        {
            selection = GameObject.Find("Player Manager").GetComponent<UnitSelection>();
            vision = gameObject.GetComponent<VisionSignal>();
            miniMap = gameObject.GetComponent<MiniMapSignal>();
            movement = gameObject.GetComponent<UnitMovement>();
            gui.type = "Unit";
            faction = GameObject.Find("Faction Manager")
                .GetComponent<FactionManager>()
                .FactionList[FactionIndex]
                .GetComponent<Faction>();
            gui.Start(gameObject);
            for (int x = 0; x < techEffect.Length; x++)
            {
                var technology = faction.Tech[techEffect[x].index];
                technology.AddListener(gameObject);
                if (technology.active)
                {
                    Upgraded(technology.name);
                }
            }
            gameObject.name = name;
            healthObj = GetComponent<Health>();
        }


        public void DisplayHealth()
        {
            if (healthObj)
            {
                healthObj.Display();
            }
            //healthD = true;
        }

        void FixedUpdate()
        {
            if (target == null)
            {
                targetType = Target.None;
            }
            switch (targetType)
            {
                case Target.None:
                    if (anim.state != "Idle")
                    {
                        anim.state = "Idle";
                        anim.Animate();
                    }
                    break;
                case Target.Resource:
                    if (resource.source == null || resource.source.gameObject != target)
                    {
                        resource.source = target.GetComponent<ResourceSource>();
                        resource.target = target;
                    }
                    if (movement.pathComplete)
                    {
                        transform.LookAt(new Vector3(
                            target.transform.position.x,
                            transform.position.y,
                            target.transform.position.z));

                        if (anim.state != "Gather")
                        {
                            anim.state = "Gather";
                            anim.Animate();
                        }
                        if (!resource.Gather(this, gameObject))
                        {
                            ResetTarget();
                        }
                    }
                    else if (anim.state != "Move")
                    {
                        anim.state = "Move";
                        anim.Animate();
                    }
                    break;
                case Target.Unit:
                    if (movement.pathComplete)
                    {
                        transform.LookAt(new Vector3(
                            target.transform.position.x,
                            transform.position.y,
                            target.transform.position.z));
                        if (!weapon.InRange(target.transform.position, transform.position))
                        {
                            if (anim.state != "Move")
                            {
                                anim.state = "Move";
                                anim.Animate();
                            }
                            movement.target = target.transform.position;
                            movement.RequestPath(target.transform.position);
                            //lastTargetPoint = target.transform.position;
                        }
                        switch (tState)
                        {
                            case TargetState.Undecided:
                                UnitController targetController = target.GetComponent<UnitController>();
                                switch (faction.Relations[targetController.FactionIndex].state)
                                {
                                    case 0:
                                        tState = TargetState.Ally;
                                        break;
                                    case 1:
                                        tState = TargetState.Neutral;
                                        break;
                                    case 2:
                                        tState = TargetState.Enemy;
                                        break;
                                    case 3:
                                        tState = TargetState.Self;
                                        break;
                                }
                                break;
                            case TargetState.Ally:
                                break;
                            case TargetState.Neutral:
                                // I haven't quite figured out what to do here so I recommend against setting anything to neutral at this moment
                                break;
                            case TargetState.Enemy:
                                weapon.AttackObject(target, gameObject, "Unit", type);
                                if (weapon.fighterUnit && anim.state != "Attack")
                                {
                                    anim.state = "Attack";
                                    anim.Animate();
                                }
                                break;
                        }
                    }
                    else if (anim.state != "Move")
                    {
                        anim.state = "Move";
                        anim.Animate();
                    }
                    if (weapon.InRange(target.transform.position, transform.position))
                    {
                        movement.pathComplete = true;
                    }
                    break;
                case Target.Location:
                    if (movement.pathComplete)
                    {
                        ResetTarget();
                    }
                    else if (anim.state != "Move")
                    {
                        anim.state = "Move";
                        anim.Animate();
                    }
                    break;
                case Target.Build:
                    if ((transform.position - target.transform.position).sqrMagnitude < build.buildDist)
                    {
                        movement.pathComplete = true;
                    }
                    if (movement.pathComplete)
                    {
                        transform.LookAt(new Vector3(
                            target.transform.position.x,
                            transform.position.y,
                            target.transform.position.z));
                        if (tState == TargetState.Undecided)
                        {
                            BuildingController targetController = target.GetComponent<BuildingController>();
                            switch (faction.Relations[targetController.FactionIndex].state)
                            {
                                case 0:
                                    tState = TargetState.Ally;
                                    break;
                                case 1:
                                    tState = TargetState.Neutral;
                                    break;
                                case 2:
                                    tState = TargetState.Enemy;
                                    break;
                                case 3:
                                    tState = TargetState.Self;
                                    break;
                            }
                        }
                        switch (tState)
                        {
                            case TargetState.Self:
                                if (build.source == null || build.source.gameObject != target)
                                {
                                    build.source = target.GetComponent<BuildingController>();
                                }
                                else if (build.source.buildingType == BuildingType.ProgressBuilding && build.Build())
                                {
                                    if (anim.state != "Build")
                                    {
                                        anim.state = "Build";
                                        anim.Animate();
                                    }
                                }
                                else
                                {
                                    ResetTarget();
                                }
                                break;
                            case TargetState.Enemy:
                                weapon.AttackObject(target, gameObject, "Building", type);
                                if (anim.state != "Attack")
                                {
                                    anim.state = "Attack";
                                    anim.Animate();
                                }
                                break;
                        }
                    }
                    else if (anim.state != "Move")
                    {
                        anim.state = "Move";
                        anim.Animate();
                    }
                    break;
                case Target.DropOff:
                    if ((transform.position - target.transform.position).sqrMagnitude < build.buildDist)
                    {
                        movement.pathComplete = true;
                    }
                    if (movement.pathComplete)
                    {
                        transform.LookAt(new Vector3(
                            target.transform.position.x,
                            transform.position.y,
                            target.transform.position.z));
                        resource.DropOff(this);
                        var args = new Arguments
                        {
                            MyUnit = UnitActor,
                            Minerals = ResourceHelper.GetMinerals(resource.manager)
                        };
                        GameManager.Instance.FiredEvent(EventType.MineralsChanged, args);
                    }
                    else if (anim.state != "Move")
                    {
                        anim.state = "Move";
                        anim.Animate();
                    }
                    break;
            }
        }

        public int DetermineRelations(int factionIndex)
        {
            return faction == null || factionIndex == FactionIndex
                ? 3
                : faction.Relations[factionIndex].state;
        }

        public void Damage(UnitType nType, int attackDamage)
        {
            for (int x = 0; x < type.weaknesses.Length; x++)
            {
                if (type.weaknesses[x].targetName == nType.name)
                {
                    attackDamage = (int) (attackDamage / type.weaknesses[x].amount);
                }
            }
            for (int x = 0; x < nType.strengths.Length; x++)
            {
                if (nType.strengths[x].targetName == type.name)
                {
                    attackDamage = (int) (attackDamage * type.strengths[x].amount);
                }
            }
            health = health - attackDamage;
            if (health <= 0)
            {
                anim.Die(gameObject);
            }
        }

        public void Select(bool select)
        {
            selected = select;
            gui.Selected(selected);
        }

        public void Upgraded(string tech)
        {
            for (int x = 0; x < techEffect.Length; x++)
            {
                if (techEffect[x].name == tech)
                {
                    techEffect[x].Replace(gameObject);
                }
            }
        }

        public void SetTarget(GameObject nTarget, Vector3 nTargetPoint, string nTargetType)
        {
            target = nTarget;
            targetPoint = nTargetPoint;
            //lastTargetPoint = nTargetPoint;
            switch (nTargetType)
            {
                case "Unit":
                    targetType = Target.Unit;
                    break;
                case "Resource":
                    targetType = Target.Resource;
                    break;
                case "Location":
                    targetType = Target.Location;
                    break;
                case "Building":
                    targetType = Target.Build;
                    break;
                case "DropOff":
                    targetType = Target.DropOff;
                    break;
            }
            movement.target = nTargetPoint;
            movement.RequestPath(targetPoint);
        }

        void ResetTarget()
        {
            target = null;
            targetPoint = Vector3.zero;
            targetType = Target.None;
        }

        public void SphereSignal(int type, GameObject obj)
        {
            if (!weapon.fighterUnit
                || targetType != Target.Location && targetType != Target.None && targetType != Target.Resource)
            {
                return;
            }
            target = obj;
            targetPoint = obj.transform.position;
            targetType = Target.Unit;
        }


        // Getters and Setters
        //{
        // Name

        public string GetName()
        {
            return name;
        }

        public void SetName(string nVal)
        {
            name = nVal;
        }

        // MaxHealth

        public int GetMaxHealth()
        {
            return maxHealth;
        }

        public void SetMaxHealth(float nVal)
        {
            maxHealth = (int) nVal;
        }

        public void AddMaxHealth(float nVal)
        {
            maxHealth += (int) nVal;
        }

        public void SubMaxHealth(float nVal)
        {
            maxHealth -= (int) nVal;
        }

        // Health

        public int GetHealth()
        {
            return health;
        }

        public void SetHealth(float nVal)
        {
            health = (int) nVal;
        }

        public void AddHealth(float nVal)
        {
            health += (int) nVal;
        }

        public void SubHealth(float nVal)
        {
            health -= (int) nVal;
        }

        // Group

        public int GetGroup()
        {
            return FactionIndex;
        }

        public void SetGroup(float nVal)
        {
            FactionIndex = (int) nVal;
            if (miniMap != null)
            {
                miniMap.factionIndex = (int) nVal;
            }
        }

        // AttackRate

        public float GetAttackRate()
        {
            return weapon.attackRate;
        }

        public void SetAttackRate(float nVal)
        {
            weapon.attackRate = nVal;
        }

        public void AddAttackRate(float nVal)
        {
            weapon.attackRate += nVal;
        }

        public void SubAttackRate(float nVal)
        {
            weapon.attackRate -= nVal;
        }

        // AttackRange

        public float GetAttackRange()
        {
            return weapon.attackRange;
        }

        public void SetAttackRange(float nVal)
        {
            weapon.attackRange = nVal;
        }

        public void AddAttackRange(float nVal)
        {
            weapon.attackRange += nVal;
        }

        public void SubAttackRange(float nVal)
        {
            weapon.attackRange -= nVal;
        }

        // AttackDamage

        public int GetAttackDamage()
        {
            return weapon.attackDamage;
        }

        public void SetAttackDamage(float nVal)
        {
            weapon.attackDamage = (int) nVal;
        }

        public void AddAttackDamage(float nVal)
        {
            weapon.attackDamage += (int) nVal;
        }

        public void SubAttackDamage(float nVal)
        {
            weapon.attackDamage -= (int) nVal;
        }

        // LookRange

        public float GetLookRange()
        {
            return weapon.lookRange;
        }

        public void SetLookRange(float nVal)
        {
            weapon.lookRange = nVal;
        }

        public void AddLookRange(float nVal)
        {
            weapon.lookRange += nVal;
        }

        public void SubLookRange(float nVal)
        {
            weapon.lookRange -= nVal;
        }

        // State

        public string GetState()
        {
            return anim.state;
        }

        public void SetState(string nVal)
        {
            anim.state = nVal;
        }

        // Size

        public int GetSize()
        {
            return size;
        }

        public void SetSize(float nVal)
        {
            size = (int) nVal;
        }

        public void AddSize(float nVal)
        {
            size += (int) nVal;
        }

        public void SubSize(float nVal)
        {
            size -= (int) nVal;
        }

        // Vision

        public int GetRadius()
        {
            return vision.radius;
        }

        public void SetRadius(float nVal)
        {
            vision.radius = (int) nVal;
        }

        public void AddRadius(float nVal)
        {
            vision.radius += (int) nVal;
        }

        public void SubRadius(float nVal)
        {
            vision.radius -= (int) nVal;
        }

        // Resource Unit
        public bool GetResourceUnit()
        {
            return resource.resourceUnit;
        }

        public void SetResourceUnit(bool nVal)
        {
            resource.resourceUnit = nVal;
        }

        // CanGather

        public bool GetCanGather()
        {
            return resource.behaviour[index].canGather;
        }

        public void SetCanGather(bool nVal)
        {
            resource.behaviour[index].canGather = nVal;
        }

        // Resource Amount

        public int GetResourceAmount()
        {
            return resource.behaviour[index].amount;
        }

        public void SetResourceAmount(float nVal)
        {
            resource.behaviour[index].amount = (int) nVal;
        }

        public void AddResourceAmount(float nVal)
        {
            resource.behaviour[index].amount += (int) nVal;
        }

        public void SubResourceAmount(float nVal)
        {
            resource.behaviour[index].amount -= (int) nVal;
        }

        // Resource Rate

        public float GetResourceRate()
        {
            return resource.behaviour[index].rate;
        }

        public void SetResourceRate(float nVal)
        {
            resource.behaviour[index].rate = nVal;
        }

        public void AddResourceRate(float nVal)
        {
            resource.behaviour[index].rate += nVal;
        }

        public void SubResourceRate(float nVal)
        {
            resource.behaviour[index].rate -= nVal;
        }

        // Carry Capacity

        public float GetCarryCapacity()
        {
            return resource.behaviour[index].carryCapacity;
        }

        public void SetCarryCapacity(float nVal)
        {
            resource.behaviour[index].carryCapacity = (int) nVal;
        }

        public void AddCarryCapacity(float nVal)
        {
            resource.behaviour[index].carryCapacity += (int) nVal;
        }

        public void SubCarryCapacity(float nVal)
        {
            resource.behaviour[index].carryCapacity -= (int) nVal;
        }

        // Builder Unit
        public bool GetBuilderUnit()
        {
            return build.builderUnit;
        }

        public void SetBuilderUnit(bool nVal)
        {
            build.builderUnit = nVal;
        }

        // CanBuild

        public bool GetCanBuild()
        {
            return build.build[index].canBuild;
        }

        public void SetCanBuild(bool nVal)
        {
            build.build[index].canBuild = nVal;
        }

        // Builder Amount

        public int GetBuilderAmount()
        {
            return build.build[index].amount;
        }

        public void SetBuilderAmount(float nVal)
        {
            build.build[index].amount = (int) nVal;
        }

        public void AddBuilderAmount(float nVal)
        {
            build.build[index].amount += (int) nVal;
        }

        public void SubBuilderAmount(float nVal)
        {
            build.build[index].amount -= (int) nVal;
        }

        // Builder Rate

        public float GetBuilderRate()
        {
            return build.build[index].rate;
        }

        public void SetBuilderRate(float nVal)
        {
            build.build[index].rate = (int) nVal;
        }

        public void AddBuilderRate(float nVal)
        {
            build.build[index].rate += (int) nVal;
        }

        public void SubBuilderRate(float nVal)
        {
            build.build[index].rate -= (int) nVal;
        }

        public void SetIndex(int nVal)
        {
            index = nVal;
        }

        // Speed

        public int GetSpeed()
        {
            return movement != null ? movement.speed : 0;
        }

        public void SetSpeed(float nVal)
        {
            if (movement != null)
            {
                movement.speed = (int) nVal;
            }
        }

        public void AddSpeed(float nVal)
        {
            if (movement != null)
            {
                movement.speed -= (int) nVal;
            }
        }

        public void SubSpeed(float nVal)
        {
            if (movement != null)
            {
                movement.speed -= (int) nVal;
            }
        }

        // Rotate Speed

        public int GetRotateSpeed()
        {
            return movement != null ? movement.rotateSpeed : 0;
        }

        public void SetRotateSpeed(float nVal)
        {
            if (movement != null)
            {
                movement.rotateSpeed = (int) nVal;
            }
        }

        public void AddRotateSpeed(float nVal)
        {
            if (movement != null)
            {
                movement.rotateSpeed -= (int) nVal;
            }
        }

        public void SubRotateSpeed(float nVal)
        {
            if (movement != null)
            {
                movement.rotateSpeed -= (int) nVal;
            }
        }

        // Mini Map Tag

        public string GetMiniMapTag()
        {
            return miniMap != null ? miniMap.miniMapTag : "";
        }

        public void SetMiniMapTag(string nVal)
        {
            if (miniMap != null)
            {
                miniMap.miniMapTag = nVal;
            }
        }

        // Vision Radius

        public int GetVisionRadius()
        {
            return vision != null ? vision.radius : 0;
        }

        public void SetVisionRadius(float nVal)
        {
            if (vision != null)
            {
                vision.radius = (int) nVal;
            }
        }

        public void AddVisionRadius(float nVal)
        {
            if (vision != null)
            {
                vision.radius += (int) nVal;
            }
        }

        public void SubVisionRadius(float nVal)
        {
            if (vision != null)
            {
                vision.radius -= (int) nVal;
            }
        }

        //}
    }
}
