using UnityEngine;
using System.Collections;

public class VisionSignal : MonoBehaviour {
    
    // This sets up an object with dynamic vision on the fog of war
    // That vision will be represented by a transparent circle with the
    // defined radius.
    
	public int radius = 5;
	public int upwardSightHeight = 1;
	public int downwardSightHeight = 1;
	
	// Use this for initialization
	void Start () {
		GameObject.Find("Fog").GetComponent<Fog>().AddAgent(gameObject, radius, upwardSightHeight, downwardSightHeight, this);
	}
}
