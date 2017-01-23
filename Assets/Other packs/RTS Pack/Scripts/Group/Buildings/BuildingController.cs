using UnityEngine;

public enum BuildingType
{
	TempBuilding,
	ProgressBuilding,
	CompleteBuilding
}

public class BuildingController : MonoBehaviour
{
	public UnitType type;
	public BuildingType buildingType;
	public Building building;
	public new string name = "Building";
	public int maxHealth = 100;
	public int health = 100;
	public int group;
	public SUnitBuilding unitProduction = new SUnitBuilding();
	public STechBuilding techProduction = new STechBuilding();
	public SBuildingGUI bGUI = new SBuildingGUI();
	public SBuildingAnim anim = new SBuildingAnim();
	public SGUI gui = new SGUI();
	public TechEffect[] techEffect = new TechEffect[0];
	public int loc;
	public float progressReq = 100;
	public float progressCur;
	public float progressPerRate;
	public int buildIndex;
	public float progressRate = 0.5f;
	public int garrison;
	public GameObject nextBuild;
	public int size = 1;
	public int gridI;
	[HideInInspector] public int index;
	[HideInInspector] public int index1;
	[HideInInspector] public GUIManager manager;
	bool selected;
	int[] nodeLoc;
	//bool displayGUI = false;
	Faction g;
	Health healthObj;
	//Progress progressObj;
	UGrid grid;
	UnitSelection selection;

	void Start()
	{
		selection = GameObject.Find("Player Manager").GetComponent<UnitSelection>();
		manager = GameObject.Find("Player Manager").GetComponent<GUIManager>();
		gui.type = "Building";
		g = GameObject.Find("Player Manager").GetComponent<GUIManager>().group;
		if (buildingType == BuildingType.ProgressBuilding)
		{
			InvokeRepeating("Progress", 0, progressRate);
		}

		else if (buildingType == BuildingType.CompleteBuilding)
		{
			gameObject.name = name;
			gui.Start(gameObject);
			for (int x = 0; x < techEffect.Length; x++)
			{
				g.Tech[techEffect[x].index].AddListener(gameObject);
				if (g.Tech[techEffect[x].index].active)
				{
					Upgraded(g.Tech[techEffect[x].index].name);
				}
			}
		}
		grid = GameObject.Find("UGrid").GetComponent<UGrid>();
		healthObj = GetComponent<Health>();
		//progressObj = GetComponent<Progress>();
	}

	void Progress()
	{
		progressCur = progressCur + progressPerRate;
	}

	public bool RequestBuild(float amount)
	{
		progressCur = progressCur + amount;
		return true;
	}

	public void FixedUpdate()
	{
		gui.Selected(selected);
		if (progressCur >= progressReq)
		{
			Place();
		}
		if (buildingType == BuildingType.CompleteBuilding)
		{
			if (unitProduction.canProduce)
			{
				unitProduction.Produce(g, group, grid, gridI);
			}
			if (techProduction.canProduce)
			{
				techProduction.Produce(g);
			}
		}
	}

	public void Select(bool state, int index)
	{
		if (index == 0)
		{
			selected = state;
			//displayGUI = state;
		}
		else
		{
			selected = state;
			//displayGUI = false;
		}
	}

	public void Place()
	{
		GameObject obj = Instantiate(nextBuild, transform.position, Quaternion.identity) as GameObject;
		BuildingController build = obj.GetComponent<BuildingController>();
		build.building = building;
		build.loc = loc;
		Destroy(gameObject);
	}

	public void Damage(UnitType nType, int damage)
	{
		for (int x = 0; x < type.weaknesses.Length; x++)
		{
			if (type.weaknesses[x].targetName == nType.name)
			{
				damage = (int) (damage / type.weaknesses[x].amount);
			}
		}
		for (int x = 0; x < nType.strengths.Length; x++)
		{
			if (nType.strengths[x].targetName == type.name)
			{
				damage = (int) (damage * type.strengths[x].amount);
			}
		}
		health = health - damage;
		if (health <= 0)
		{
			building.OpenPoints(GameObject.Find("UGrid").GetComponent<UGrid>(), gridI, loc);
			Destroy(gameObject);
		}
	}

	public void DisplayHealth()
	{
		if (healthObj)
		{
			healthObj.Display();
		}
	}

	public void DisplayGUI(float ratioX, float ratioY)
	{
		int x1 = 0;
		int y1 = 0;
		for (int x = 0; x < unitProduction.units.Length; x++)
		{
			if (unitProduction.units[x].canProduce)
			{
				if (bGUI.unitGUI.Display(x1, y1, unitProduction.units[x].customName, unitProduction.units[x].customTexture, ratioX,
					ratioY))
				{
					unitProduction.StartProduction(x);
				}
				if (bGUI.unitGUI.contains)
				{
					manager.mouseOverGUI = true;
					manager.mouseOverUnitProduction = true;
					manager.unitProductionIndex = x;
				}
				x1++;
				if (x1 >= bGUI.unitGUI.buttonPerRow)
				{
					x1 = 0;
					y1++;
				}
			}
		}
		x1 = 0;
		y1 = 0;
		for (int x = 0; x < techProduction.techs.Length; x++)
		{
			if (techProduction.techs[x].canProduce && !g.Tech[techProduction.techs[x].index].beingProduced)
			{
				if (bGUI.technologyGUI.Display(x1, y1, techProduction.techs[x].customName, techProduction.techs[x].customTexture,
					ratioX, ratioY))
				{
					techProduction.StartProduction(x);
				}
				if (bGUI.technologyGUI.contains)
				{
					manager.mouseOverGUI = true;
					manager.mouseOverTechProduction = true;
					manager.techProductionIndex = x;
				}
				x1++;
				if (x1 >= bGUI.technologyGUI.buttonPerRow)
				{
					x1 = 0;
					y1++;
				}
			}
		}
		x1 = 0;
		y1 = 0;
		for (int x = 0; x < unitProduction.jobsAmount; x++)
		{
			if (bGUI.jobsGUI.Display(x1, y1, unitProduction.jobs[x].customName, unitProduction.jobs[x].customTexture, ratioX,
				ratioY))
			{
				unitProduction.CancelProduction(x);
			}
			if (bGUI.jobsGUI.contains)
			{
				manager.mouseOverGUI = true;
			}
			x1++;
			if (x1 >= bGUI.jobsGUI.buttonPerRow)
			{
				x1 = 0;
				y1++;
			}
		}
		for (int x = 0; x < techProduction.jobsAmount; x++)
		{
			if (bGUI.jobsGUI.Display(x1, y1, techProduction.jobs[x].customName, techProduction.jobs[x].customTexture, ratioX,
				ratioY))
			{
				techProduction.CancelProduction(x, g);
			}
			if (bGUI.jobsGUI.contains)
			{
				manager.mouseOverGUI = true;
			}
			x1++;
			if (x1 >= bGUI.jobsGUI.buttonPerRow)
			{
				x1 = 0;
				y1++;
			}
		}
	}

	void OnMouseDown()
	{
		selection.AddSelectedBuilding(gameObject);
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

	// Max Health
	public int GetMaxHealth()
	{
		return maxHealth;
	}

	public void SetMaxHealth(int nVal)
	{
		maxHealth = nVal;
	}

	public void AddMaxHealth(int nVal)
	{
		maxHealth += nVal;
	}

	public void SubMaxHealth(int nVal)
	{
		maxHealth -= nVal;
	}

	// Health

	public int GetHealth()
	{
		return health;
	}

	public void SetHealth(int nVal)
	{
		health = nVal;
	}

	public void AddHealth(int nVal)
	{
		health += nVal;
	}

	public void SubHealth(int nVal)
	{
		health -= nVal;
	}

	// Group

	public int GetGroup()
	{
		return group;
	}

	public void SetGroup(int nVal)
	{
		group = nVal;
	}

	// Unit Can Produce

	public bool GetUCanProduce()
	{
		return unitProduction.canProduce;
	}

	public void SetUCanProduce(bool nVal)
	{
		unitProduction.canProduce = nVal;
	}

	// Unit Can Produce [Interior]

	public bool GetUICanProduce()
	{
		return unitProduction.units[index].canProduce;
	}

	public void SetUICanProduce(bool nVal)
	{
		unitProduction.units[index].canProduce = nVal;
	}

	// Unit Cost

	public int GetUCost()
	{
		return unitProduction.units[index].cost[index1];
	}

	public void SetUCost(int nVal)
	{
		unitProduction.units[index].cost[index1] = nVal;
	}

	public void AddUCost(int nVal)
	{
		unitProduction.units[index].cost[index1] += nVal;
	}

	public void SubUCost(int nVal)
	{
		unitProduction.units[index].cost[index1] -= nVal;
	}

	// Unit Dur

	public float GetUDur()
	{
		return unitProduction.units[index].dur;
	}

	public void SetUDur(float nVal)
	{
		unitProduction.units[index].dur = nVal;
	}

	public void AddUDur(float nVal)
	{
		unitProduction.units[index].dur += nVal;
	}

	public void SubUDur(float nVal)
	{
		unitProduction.units[index].dur -= nVal;
	}

	// Unit Rate

	public float GetURate()
	{
		return unitProduction.units[index].rate;
	}

	public void SetURate(float nVal)
	{
		unitProduction.units[index].rate = nVal;
	}

	public void AddURate(float nVal)
	{
		unitProduction.units[index].rate += nVal;
	}

	public void SubURate(float nVal)
	{
		unitProduction.units[index].rate -= nVal;
	}

	// Unit JobsAmount

	public int GetUJobsAmount()
	{
		return unitProduction.jobsAmount;
	}

	// Unit CanBuildAtOnce

	public int GetUCanBuildAtOnce()
	{
		return unitProduction.canBuildAtOnce;
	}

	public void SetUCanBuildAtOnce(int nVal)
	{
		unitProduction.canBuildAtOnce = nVal;
	}

	public void AddUCanBuildAtOnce(int nVal)
	{
		unitProduction.canBuildAtOnce += nVal;
	}

	public void SubUCanBuildAtOnce(int nVal)
	{
		unitProduction.canBuildAtOnce -= nVal;
	}

	// Unit MaxAmount

	public int GetUMaxAmount()
	{
		return unitProduction.maxAmount;
	}

	public void SetUMaxAmount(int nVal)
	{
		unitProduction.maxAmount = nVal;
	}

	public void AddUMaxAmount(int nVal)
	{
		unitProduction.maxAmount += nVal;
	}

	public void SubUMaxAmount(int nVal)
	{
		unitProduction.maxAmount -= nVal;
	}

	// Tech Can Produce

	public bool GetTCanProduce()
	{
		return techProduction.canProduce;
	}

	public void SetTCanProduce(bool nVal)
	{
		techProduction.canProduce = nVal;
	}

	// Tech Can Produce [Interior]

	public bool GetTICanProduce()
	{
		return techProduction.techs[index].canProduce;
	}

	public void SetTICanProduce(bool nVal)
	{
		techProduction.techs[index].canProduce = nVal;
	}

	// Tech Cost

	public int GetTCost()
	{
		return techProduction.techs[index].cost[index1];
	}

	public void SetTCost(int nVal)
	{
		techProduction.techs[index].cost[index1] = nVal;
	}

	public void AddTCost(int nVal)
	{
		techProduction.techs[index].cost[index1] += nVal;
	}

	public void SubTCost(int nVal)
	{
		techProduction.techs[index].cost[index1] -= nVal;
	}

	// Tech Dur

	public float GetTDur()
	{
		return techProduction.techs[index].dur;
	}

	public void SetTDur(float nVal)
	{
		techProduction.techs[index].dur = nVal;
	}

	public void AddTDur(float nVal)
	{
		techProduction.techs[index].dur += nVal;
	}

	public void SubTDur(float nVal)
	{
		techProduction.techs[index].dur -= nVal;
	}

	// Tech Rate

	public float GetTRate()
	{
		return techProduction.techs[index].rate;
	}

	public void SetTRate(float nVal)
	{
		techProduction.techs[index].rate = nVal;
	}

	public void AddTRate(float nVal)
	{
		techProduction.techs[index].rate += nVal;
	}

	public void SubTRate(float nVal)
	{
		techProduction.techs[index].rate -= nVal;
	}

	// Tech Jobs Amount

	public int GetTJobsAmount()
	{
		return techProduction.jobsAmount;
	}

	// Tech MaxAmount

	public int GetTMaxAmount()
	{
		return techProduction.maxAmount;
	}

	public void SetTMaxAmount(int nVal)
	{
		techProduction.maxAmount = nVal;
	}

	public void AddMTaxAmount(int nVal)
	{
		techProduction.maxAmount += nVal;
	}

	public void SubTMaxAmount(int nVal)
	{
		techProduction.maxAmount -= nVal;
	}

	// Tech CanBuildAtOnce

	public int GetTCanBuildAtOnce()
	{
		return techProduction.canBuildAtOnce;
	}

	public void SetTCanBuildAtOnce(int nVal)
	{
		techProduction.canBuildAtOnce = nVal;
	}

	public void AddTCanBuildAtOnce(int nVal)
	{
		techProduction.canBuildAtOnce += nVal;
	}

	public void SubTCanBuildAtOnce(int nVal)
	{
		techProduction.canBuildAtOnce -= nVal;
	}

	public void SetIndex(int nVal)
	{
		index = nVal;
	}

	public void SetIndex1(int nVal)
	{
		index1 = nVal;
	}

	//}
}
