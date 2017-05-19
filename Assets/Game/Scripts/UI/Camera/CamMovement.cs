using UnityEngine;

namespace XposeCraft.UI.Camera
{
    public class CamMovement : MonoBehaviour
    {
        public Vector2 moveSpeed;
        Vector2 moveSpeedCur;
        public int modifier;
        public Vector2 scrollMinMax;
        public float scrollSpeed;
        public UnityEngine.Camera cam;
        public float scrollCur { get; set; }
        public Rect movementBorders;
        public Rect mouseBorders;

        void Update()
        {
            moveSpeedCur = new Vector2();
            MouseBorderMovement();
            KeyMovement();
            ScrollWheel();
            if (moveSpeedCur.x < 0 && moveSpeedCur.x + transform.position.x < movementBorders.x)
            {
                transform.position = new Vector3(movementBorders.x, transform.position.y, transform.position.z);
                moveSpeedCur = new Vector2(0, moveSpeedCur.y);
            }
            else if (moveSpeedCur.x > 0 && moveSpeedCur.x + transform.position.x > movementBorders.width)
            {
                transform.position = new Vector3(movementBorders.width, transform.position.y, transform.position.z);
                moveSpeedCur = new Vector2(0, moveSpeedCur.y);
            }
            if (moveSpeedCur.y < 0 && moveSpeedCur.y + transform.position.z < movementBorders.y)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, movementBorders.y);
                moveSpeedCur = new Vector2(moveSpeedCur.x, 0);
            }
            else if (moveSpeedCur.y > 0 && moveSpeedCur.y + transform.position.z > movementBorders.height)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, movementBorders.height);
                moveSpeedCur = new Vector2(moveSpeedCur.x, 0);
            }
            transform.Translate(moveSpeedCur.x, 0, moveSpeedCur.y);
        }

        // Movement For if the Mouse Goes to a certain area oncreen
        void MouseBorderMovement()
        {
            if (Input.mousePosition.x >= 0 && Input.mousePosition.x < mouseBorders.x)
            {
                moveSpeedCur += new Vector2(-moveSpeed.x, 0);
            }
            else if (Input.mousePosition.x <= Screen.width
                     && Input.mousePosition.x - Screen.width > -mouseBorders.width)
            {
                moveSpeedCur += new Vector2(moveSpeed.x, 0);
            }
            if (Input.mousePosition.y >= 0 && Input.mousePosition.y < mouseBorders.y)
            {
                moveSpeedCur += new Vector2(0, -moveSpeed.y);
            }
            else if (Input.mousePosition.y <= Screen.height
                     && Input.mousePosition.y - Screen.height > -mouseBorders.height)
            {
                moveSpeedCur += new Vector2(0, moveSpeed.y);
            }
        }

        // Keyboard based movement
        void KeyMovement()
        {
            moveSpeedCur += new Vector2(
                Input.GetAxis("Horizontal") * moveSpeed.x,
                Input.GetAxis("Vertical") * moveSpeed.y);
            if (Input.GetButton("IncreaseMoveSpeed"))
            {
                moveSpeedCur *= modifier;
            }
        }

        // Used for Scrollwheel functionality
        void ScrollWheel()
        {
            float amount;
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scrollCur < 0)
            {
                scroll *= 2;
            }
            if (scroll * scrollSpeed + scrollCur > scrollMinMax.y)
            {
                amount = scrollMinMax.y - scrollCur;
            }
            else if (scroll * scrollSpeed + scrollCur < scrollMinMax.x)
            {
                amount = scrollMinMax.x - scrollCur;
            }
            else
            {
                amount = scroll * scrollSpeed;
            }
            scrollCur += amount;
            cam.transform.position = cam.transform.position + cam.transform.forward * amount;
        }
    }
}
