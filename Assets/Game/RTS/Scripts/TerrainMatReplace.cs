using UnityEngine;

public class TerrainMatReplace : MonoBehaviour
{
    public Material mat;

    void Start()
    {
        gameObject.GetComponent<Terrain>().materialTemplate = mat;
    }
}
