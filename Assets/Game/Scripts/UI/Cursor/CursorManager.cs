using UnityEngine;
using XposeCraft.Core.Required;

namespace XposeCraft.UI.Cursor
{
    public class CursorManager : MonoBehaviour
    {
        public CursorType[] cursor;
        string curState = "";
        Vector2 hotSpot = Vector2.zero;

        void OnDrawGizmos()
        {
            if (gameObject.name != "Cursor Manager")
            {
                gameObject.name = "Cursor Manager";
            }
        }

        void OnDrawGizmosSelected()
        {
            for (int x = 1; x < cursor.Length; x++)
            {
                if (cursor[x].moveUp)
                {
                    cursor[x].moveUp = false;
                    CursorType clone = cursor[x - 1];
                    cursor[x - 1] = cursor[x];
                    cursor[x] = clone;
                }
            }
        }

        void Update()
        {
            for (int x = 0; x < cursor.Length; x++)
            {
                if (curState == cursor[x].tag)
                {
                    UnityEngine.Cursor.SetCursor(cursor[x].texture, hotSpot, new CursorMode());
                }
            }
            curState = cursor[cursor.Length - 1].tag;
        }

        public void CursorSet(string state)
        {
            for (int x = 0; x < cursor.Length; x++)
            {
                if (cursor[x].tag == curState)
                {
                    break;
                }
                if (cursor[x].tag == state)
                {
                    curState = state;
                    break;
                }
            }
        }
    }
}
