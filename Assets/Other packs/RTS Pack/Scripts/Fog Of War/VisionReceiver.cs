using UnityEngine;

public class VisionReceiver : MonoBehaviour
{
	// This code determines how a unit is displayed when in the In Vision, Discovered, and Undiscovered areas
	// The pastVisibleMat is displayed when the object has been seen at one time but not at this moment
	// The defaultMat is used for the unit if they are within vision at that time

	public MeshRenderer[] renderers;
	public Color[] defaultMat;
	public Color[] pastVisibleMat = {Color.gray, Color.gray};
	public bool hideObjectWhenNotSeen;
	public bool hideCursorIcon = true;
	public bool hideObject = true;
	public Vector2[] anchors;
	CursorObject cursorObj;
	public int curState;

	void Start()
	{
		GameObject.Find("Fog").GetComponent<Fog>().AddRenderer(gameObject, this);
		int matAmount = 0;
		for (int x = 0; x < renderers.Length; x++)
		{
			matAmount += renderers[x].materials.Length;
		}
		defaultMat = new Color[matAmount];
		int z = 0;
		for (int x = 0; x < renderers.Length; x++)
		{
			for (int y = 0; y < renderers[x].materials.Length; y++)
			{
				defaultMat[z] = renderers[x].materials[y].color;
				z++;
			}
		}
		if (hideCursorIcon)
		{
			cursorObj = gameObject.GetComponent<CursorObject>();
			if (cursorObj == null)
				hideCursorIcon = false;
		}
	}

	public void SetRenderer(int state)
	{
		curState = state;
		// In Vision
		if (state == 0)
		{
			for (int x = 0; x < renderers.Length; x++)
			{
				if (x == 0 && renderers[x].material.color == defaultMat[x] && renderers[x].enabled)
					break;
				else
				{
					renderers[x].material.color = defaultMat[x];
					renderers[x].enabled = true;
					if (hideCursorIcon)
						cursorObj.enabled = true;
				}
			}
		}
		// Discovered
		else if (state == 1)
		{
			for (int x = 0; x < renderers.Length; x++)
			{
				if (x == 0 && renderers[x].material.color == pastVisibleMat[x] && renderers[x].enabled)
					break;
				else
				{
					if (pastVisibleMat.Length > x)
						renderers[x].material.color = pastVisibleMat[x];
					if (hideObjectWhenNotSeen)
						renderers[x].enabled = false;
					else
						renderers[x].enabled = true;
					if (hideCursorIcon)
						cursorObj.enabled = true;
				}
			}
		}
		// Undiscovered
		else
		{
			for (int x = 0; x < renderers.Length; x++)
			{
				if (x == 0 && !renderers[x].enabled)
					break;
				else
				{
					if (hideObject)
					{
						renderers[x].enabled = false;
					}
					if (hideCursorIcon)
						cursorObj.enabled = false;
				}
			}
		}
	}
}
