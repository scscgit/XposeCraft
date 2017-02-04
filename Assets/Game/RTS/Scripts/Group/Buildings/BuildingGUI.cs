using UnityEngine;

public class BuildingGUI : MonoBehaviour
{
    public Faction groupManager;
    public Rect guiSize;
    public BuildingPlacement place;

    void OnGUI()
    {
        int y = 0;
        int z = 0;
        foreach (Building building in groupManager.BuildingList)
        {
            if (GUI.Button(
                new Rect(guiSize.x + z * guiSize.width, guiSize.y + y * guiSize.height, guiSize.width, guiSize.height),
                building.obj.GetComponent<BuildingController>().name))
            {
                place.BeginPlace(building);
            }
            z = z + 1;
            if (z == y)
            {
                y++;
                z = 0;
            }
        }
    }
}
