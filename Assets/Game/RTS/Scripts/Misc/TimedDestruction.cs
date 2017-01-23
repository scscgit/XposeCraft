using UnityEngine;
using System.Collections;

public class TimedDestruction : MonoBehaviour {
	
	public float time = 3;
	float startTime = 0;
	bool set = false;
	
	// Use this for initialization
	void OnEnable () {
		startTime = Time.time;
		set = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(set){
			if(startTime + time <= Time.time){
				gameObject.SetActive(false);
			}
		}
	}
}
