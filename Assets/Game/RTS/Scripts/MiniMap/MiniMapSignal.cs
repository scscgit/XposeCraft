using UnityEngine;

public class MiniMapSignal : MonoBehaviour
{
    public string miniMapTag;
    public bool display;
    public int factionIndex { get; set; }
    public bool isStatic;
    VisionReceiver receiver;

    void Awake()
    {
        if (!GameObject.Find("MiniMap").GetComponent<MiniMap>().AddElement(gameObject, miniMapTag, this, factionIndex))
        {
            Debug.Log("MiniMap : Unit Tag : " + miniMapTag + " Not Found");
        }
        receiver = GetComponent<VisionReceiver>();
    }

    //If the renderer is disabled, the minimap icon will not show up
    void FixedUpdate()
    {
        display = receiver == null || receiver.curState < 2;
    }
}
