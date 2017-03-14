using UnityEngine;

namespace XposeCraft.UI.Cursor
{
    public class CursorObject : MonoBehaviour
    {
        CursorManager mang;
        public string cursorTag;

        void Start()
        {
            mang = GameObject.Find("Cursor Manager").GetComponent<CursorManager>();
        }

        void OnMouseOver()
        {
            if (enabled)
            {
                mang.SendMessage("CursorSet", cursorTag);
            }
        }
    }
}
