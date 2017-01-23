using UnityEngine;
using System.Collections;

public class TerrainMatReplace : MonoBehaviour
{
    public Material mat;

    // Use this for initialization
    void Start()
    {
        gameObject.GetComponent<Terrain>().materialTemplate = mat;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
