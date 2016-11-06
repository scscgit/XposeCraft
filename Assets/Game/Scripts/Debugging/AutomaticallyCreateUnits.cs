using UnityEngine;

public class AutomaticallyCreateUnits : MonoBehaviour
{
	public GameObject Unit;
	public GameObject[] SpawnPositions;
	public float CreationInterval = 20;

	Transform unitsParent;
	float timer = 0;

	void Start()
	{
		this.unitsParent = GameObject.Find("Units").transform;
	}

	void Update()
	{
		timer += Time.deltaTime;
		if (timer > CreationInterval)
		{
			timer -= CreationInterval;
			CreateNewUnit();
		}
	}

	bool CreateNewUnit()
	{
		if (SpawnPositions == null || SpawnPositions.Length == 0)
		{
			Debug.LogWarning(this + " does not have any position to spawn a unit");
			return false;
		}

		foreach (var spawnPosition in SpawnPositions)
		{
			if (Spawn(spawnPosition) != null)
			{
				Debug.Log(this + " spawned " + Unit + " on position " + spawnPosition);
				return true;
			}
		}
		Debug.LogWarning(this + " could not spawn a unit on any position");
		return false;
	}

	GameObject Spawn(GameObject spawnPosition)
	{
		RaycastHit hit;
		if (Physics.Raycast(spawnPosition.transform.position, Vector3.down, out hit))
		{
			return (GameObject)Instantiate(Unit, hit.point, Quaternion.FromToRotation(Unit.transform.up, hit.normal), unitsParent);
		}
		return null;
	}
}
