using UnityEngine;
using System.Collections;

public class BinaryTreeTest : MonoBehaviour {

	public bool remove = false;
	public bool recalculate = false;
	public int indexChanged = 0;
	public bool add = false;
	public int numberToAdd = 0;
	public int indexToAdd = 0;
	public int lowestNumber = 0;
	public BinaryHeap heap;
	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnDrawGizmos () {
		if(add){
			numberToAdd = Random.Range(0, 99);
			indexToAdd = numberToAdd;
			heap.Add(numberToAdd, indexToAdd);
			add = false;
		}
		if(remove){
			heap.Remove();
			remove = false;
		}
		if(recalculate){
			heap.Recalculate(indexChanged);
			recalculate = false;
		}
		int lw = 100;
		for(int x = 0; x < heap.numberOfItems; x++){
			if(heap.binaryHeap[x].cost < lw){
				lw = heap.binaryHeap[x].cost;
			}
		}
		lowestNumber = lw;
	}
}
