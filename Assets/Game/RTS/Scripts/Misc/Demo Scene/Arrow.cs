using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour
{
    GameObject target;
    public int speed = 5;
    public bool landed = true;

    public void Attack(GameObject obj)
    {
        target = obj;
    }

    public void Start()
    {
        transform.parent = null;
    }

    public void Update()
    {
        transform.LookAt(target.transform);
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        float dist = Vector3.Distance(target.transform.position, transform.position);
        if (dist < 1)
        {
            //TimedDestruction destroy = GetComponent<TimedDestruction>();
            landed = true;
            gameObject.SetActive(false);
        }
    }
}
