using UnityEngine;

public class AlterTime : MonoBehaviour
{
    public string key = "q";
    public int modifier = 2;

    void Update()
    {
        Time.timeScale = Input.GetKey(key) ? modifier : 1;
    }
}
