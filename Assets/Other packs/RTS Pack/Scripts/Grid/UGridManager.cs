using UnityEngine;
using System.Collections;

public class UGridManager : MonoBehaviour {
	
	public UGrid grid;
	public bool displayGrid;
	public bool generate = false;
	public int size = 0;
	
	void OnDrawGizmos () {
		grid.grids[0].displayGrid = displayGrid;
		grid.grids[0].size = size;
		grid.generate = generate;
		generate = false;
	}
}
