using UnityEngine;
using System.Collections;

public class CursorObject : MonoBehaviour {
	
	CursorManager mang;
	public string cursorTag;
	
	void Start () {
		mang = GameObject.Find("Cursor Manager").GetComponent<CursorManager>();
	}

	void OnMouseOver () {
		if(this.enabled)
			mang.SendMessage("CursorSet", cursorTag);
	}
}
