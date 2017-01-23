using UnityEngine;
using System.Collections;

public class ResourceSource : MonoBehaviour {

	public int resourceIndex = 0;
	public int amount = 0;
	public bool deleteWhenExhausted = false;
	public int closeSize = 1;
	public int gridI = 0;

	public void Start () {
		ClosePoints();
	}

	public int RequestResource (int rAmount) {
		if(amount >= rAmount){
			amount = amount - rAmount;
			return rAmount;
		}
		else{
			amount = 0;
			OpenPoints();
			Destroy(gameObject);
			return amount;
		}
	}

	public void OpenPoints () {
		UGrid grid = GameObject.Find("UGrid").GetComponent<UGrid>();
		int index = DetermineLoc(transform.position, grid);
		grid.grids[gridI].grid[index].state = 0;
		for(int x = -closeSize; x <= closeSize; x++){
			for(int y = -closeSize; y <= closeSize; y++){
				int i = x+y*grid.grids[gridI].size;
				grid.grids[gridI].grid[index+i].state = 0;
			}
		}
	}

	public void ClosePoints () {
		UGrid grid = GameObject.Find("UGrid").GetComponent<UGrid>();
		int index = DetermineLoc(transform.position, grid);
		grid.grids[gridI].grid[index].state = 2;
		for(int x = -closeSize; x <= closeSize; x++){
			for(int y = -closeSize; y <= closeSize; y++){
				int i = x+y*grid.grids[gridI].size;
				grid.grids[gridI].grid[index+i].state = 2;
			}
		}
	}

	int DetermineLoc (Vector3 loc, UGrid gridScript) {
		float xLoc = (loc.x-gridScript.grids[gridI].startLoc.x);
		float yLoc = (loc.z-gridScript.grids[gridI].startLoc.z);
		int x = Mathf.RoundToInt(xLoc/gridScript.grids[gridI].nodeDist);
		int y = Mathf.RoundToInt(yLoc/gridScript.grids[gridI].nodeDist);
		int nLoc = x+(y*gridScript.grids[gridI].size);
		return nLoc;
	}
}
