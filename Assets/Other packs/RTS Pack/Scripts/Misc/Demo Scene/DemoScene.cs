using UnityEngine;
using System.Collections;

public class DemoScene : MonoBehaviour {
	
	public GUISkin skin;
	public GameObject siloTemplate = null;
	
	string[] displayText = {"Hello, and welcome to the SRTS Pack Demo Scene! In this scene I am going to take you through the basics of the SRTS pack! First, please select a unit.",
							"Great, now lets make an outpost to spot any oncoming attacks! With the units selected, click the outpost option in the bottom left of the screen and place it on the map in a discovered area.",
							"Next lets go ahead and make a house to produce more worker units! Simply select the house option and place it in a discovered area.",
							"Now lets produce 5 more worker units! Simply select the house and in the top right click Worker 5 times.",
							"Alright, now we just need to make a silo for when the units are collecting resources. Simply place one here, since it is near the woods!",
							"",
							""};
	int state = 0;
	public UnitSelection select;
	
	void OnGUI () {
		GUI.skin = skin;
		GUI.Box(new Rect(Screen.width-300, 0, 300, 100), displayText[state]);
	}
	void FixedUpdate () {
		if(state == 0){
			if(select.curSelectedLength > 0){
				state = 1;
			}
		}
		else if(state == 1){
			if(GameObject.Find("Outpost") != null){
				state = 2;
			}
		}
		else if(state == 2){
			if(GameObject.Find("House") != null){
				state = 3;
			}
		}
		else if(state == 3){
			GameObject[] gameTag;
			gameTag = GameObject.FindGameObjectsWithTag("Unit");
			int amount = 0;
			for(int x = 0; x < gameTag.Length; x++){
				if(gameTag[x].name == "Worker Unit"){
					amount++;
				}
			}
			if(amount >= 10){
				state = 4;
			}
		}
		else if(state == 4){
			
		}
	}
}
