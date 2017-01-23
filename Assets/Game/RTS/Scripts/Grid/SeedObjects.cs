using UnityEngine;

public class SeedObjects : MonoBehaviour
{
	public Seed[] obj;
	UGrid grid;
	public int gridI;
	public bool generate;

	void OnDrawGizmos()
	{
		if (!grid)
			grid = GameObject.Find("UGrid").GetComponent<UGrid>();

		if (generate)
		{
			for (int x = 0; x < obj.Length; x++)
			{
				GameObject folder = new GameObject();
				folder.name = "Folder";
				for (int z = 0; z < obj[x].amount; z++)
				{
					int loc = 0;
					bool viable = false;
					while (viable == false)
					{
						loc = Random.Range(0, grid.grids[gridI].grid.Length);
						Vector3 point = grid.grids[gridI].grid[loc].loc;
						if (point.x >= obj[x].area.x && point.x <= obj[x].area.width && point.z >= obj[x].area.y &&
						    point.z <= obj[x].area.height)
						{
							viable = true;
						}
						if (grid.grids[gridI].grid[loc].state == 2)
						{
							viable = false;
						}
					}
					GameObject clone = Instantiate(obj[x].obj, grid.grids[gridI].grid[loc].loc, Quaternion.identity) as GameObject;
					clone.transform.parent = folder.transform;
					clone.name = obj[x].obj.name;
					grid.grids[gridI].grid[loc].state = 2;
				}
			}
			generate = false;
		}
	}
}
