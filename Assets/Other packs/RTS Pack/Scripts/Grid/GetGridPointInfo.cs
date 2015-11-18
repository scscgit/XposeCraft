using UnityEngine;
using System.Collections;

public class GetGridPointInfo : MonoBehaviour {
	public UGrid myGrid;
	public int gridI = 0;
	public bool snap = false;
	public int gridLoc = 0;
	public GridPoint point;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnDrawGizmosSelected () {
		if(snap){
			int loc = ConvertLoc(transform.position);
			gridLoc = loc;
			transform.position = myGrid.grids[gridI].grid[loc].loc;
			point = myGrid.grids[gridI].grid[loc];
		}
	}
	
	public int ConvertLoc(Vector3 point){
		float xLoc = (point.x-myGrid.grids[gridI].startLoc.x);
		float zLoc = (point.z-myGrid.grids[gridI].startLoc.z);
		int x = Mathf.RoundToInt(xLoc/myGrid.grids[gridI].nodeDist);
		int z = Mathf.RoundToInt(zLoc/myGrid.grids[gridI].nodeDist);
		int loc = x+(z*myGrid.grids[gridI].size);
		return loc;
	}
}
