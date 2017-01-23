using UnityEngine;
using System.Collections;

public class MiniMapSignal : MonoBehaviour
{
    public string miniMapTag;
    public bool display;
    [HideInInspector] public int group = 0;
    public bool isStatic = false;
    VisionReceiver receiver;

    // Use this for initialization
    void Awake()
    {
        if (GameObject.Find("MiniMap").GetComponent<MiniMap>().AddElement(gameObject, miniMapTag, this, group))
        {
        }
        else
        {
            Debug.Log("MiniMap : Unit Tag : " + miniMapTag + " Not Found");
        }
        receiver = GetComponent<VisionReceiver>();
    }

    //If the renderer is disabled, the minimap icon will not show up
    void FixedUpdate()
    {
        if (receiver == null)
        {
            display = true;
        }
        else
        {
            display = (receiver.curState < 2);
        }
    }
}
