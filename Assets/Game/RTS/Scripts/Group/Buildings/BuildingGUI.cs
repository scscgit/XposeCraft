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
        for (int x = 0; x < groupManager.BuildingList.Length; x++)
        {
            if (GUI.Button(
                new Rect(guiSize.x + z * guiSize.width, guiSize.y + y * guiSize.height, guiSize.width, guiSize.height),
                groupManager.BuildingList[x].obj.GetComponent<BuildingController>().name))
            {
                place.BeginPlace(groupManager.BuildingList[x]);
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
