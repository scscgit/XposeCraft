using UnityEngine;
using System.Collections;

public class AlterTime : MonoBehaviour
{
    public string key = "q";
    public int modifier = 2;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(key))
        {
            Time.timeScale = modifier;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
