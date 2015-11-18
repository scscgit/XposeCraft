using UnityEngine;
using System.Collections;



public class DelayScript : MonoBehaviour {
	public int amountCur = 0;
	public float delayAmount = 0.01f;
	public int increment = 5;
	public int total = 0;
	public int clearDelayAmount;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public float GetDelay () {
		if(amountCur >= increment-1){
			total++;
			amountCur = 0;
			return delayAmount*total;
		}
		else{
			amountCur++;
			return delayAmount*total;
		}
	}
	
	public void Update () {
		if(clearDelayAmount == total*increment+amountCur){
			amountCur = 0;
			total = 0;
			clearDelayAmount = 0;
		}
	}
	
	public void ClearDelay () {
		clearDelayAmount++;
	}
}
