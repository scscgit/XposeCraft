using UnityEngine;

public class TimedDestruction : MonoBehaviour
{
    public float time = 3;
    float startTime;
    bool set;

    void OnEnable()
    {
        startTime = Time.time;
        set = true;
    }

    void Update()
    {
        if (set && startTime + time <= Time.time)
        {
            gameObject.SetActive(false);
        }
    }
}
