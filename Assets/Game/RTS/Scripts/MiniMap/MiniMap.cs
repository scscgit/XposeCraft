using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public MiniMapElement[] elements = new MiniMapElement[0];
    public MiniMapElement cameraElement;
    public GameObject cam;
    public GameObject cameraController;
    public LayerMask camZoomLayer;
    public Texture background;
    public Rect localBounds;
    public Rect realWorldBounds;
    [HideInInspector] public Texture fogTexture;
    Vector2 lastPoint1;
    Vector2 lastPoint2;
    public Color fogColor = Color.clear;
    Rect size;
    Rect cameraLoc = new Rect(0, 0, 0, 0);
    bool moveCamera;

    public void SetSize()
    {
        size = new Rect(
            localBounds.x / realWorldBounds.x,
            localBounds.y / realWorldBounds.y,
            localBounds.width / realWorldBounds.width,
            localBounds.height / realWorldBounds.height);
        for (var x = 0; x < elements.Length; x++)
        {
            MiniMapElement element = elements[x];
            for (int y = 0; y < element.objAmount; y++)
            {
                element.ModifyLoc(y, Determine2dLoc(element.elementTransform[y].position));
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (gameObject.name != "MiniMap")
        {
            gameObject.name = "MiniMap";
        }
        if (elements.Length <= 1)
        {
            return;
        }
        for (int x = 1; x < elements.Length; x++)
        {
            if (elements[x].moveUp)
            {
                elements[x].moveUp = false;
                var backupElement = elements[x - 1];
                elements[x - 1] = elements[x];
                elements[x] = backupElement;
            }
        }
    }

    public bool AddElement(GameObject obj, string tag, MiniMapSignal map, int group)
    {
        bool found = false;
        int index = 0;
        for (int x = 0; x < elements.Length; x++)
        {
            if (elements[x].tag == tag)
            {
                index = x;
                found = true;
                break;
            }
        }
        if (found)
        {
            elements[index].AddElement(obj, tag, map, group, Determine2dLoc(obj.transform.position));
        }
        return found;
    }

    void FixedUpdate()
    {
        for (var x = 0; x < elements.Length; x++)
        {
            MiniMapElement element = elements[x];
            for (int y = 0; y < element.objAmount; y++)
            {
                if (element.elementObj[y] == null)
                {
                    element.elementObj.RemoveAt(y);
                    element.elementMap.RemoveAt(y);
                    element.elementTransform.RemoveAt(y);
                    element.elementLoc.RemoveAt(y);
                    element.elementGroup.RemoveAt(y);
                    element.objAmount--;
                    y--;
                }
            }
        }

        for (var x = 0; x < elements.Length; x++)
        {
            MiniMapElement element = elements[x];
            for (int y = 0; y < element.objAmount; y++)
            {
                if (!element.elementMap[y].isStatic)
                {
                    element.elementLoc[y] = Determine2dLoc(element.elementTransform[y].position);
                }
            }
        }

        //Vector2 loc = Determine2dLoc(cam.transform.position);
        MiniMapElement details = cameraElement;
        if (!details.image)
        {
            return;
        }

        GUI.color = details.tints[0];
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(0, Screen.height));
        Physics.Raycast(ray, out hit, 1000, camZoomLayer);
        bool check = hit.collider != null;
        RaycastHit hit2;
        ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width, 0));
        Physics.Raycast(ray, out hit2, 1000, camZoomLayer);
        if (hit2.collider == null)
        {
            check = false;
        }
        if (check)
        {
            lastPoint1 = Determine2dLoc(hit.point);
            lastPoint2 = Determine2dLoc(hit2.point);
        }
        cameraLoc = new Rect(lastPoint1.x, lastPoint1.y, lastPoint2.x - lastPoint1.x, lastPoint2.y - lastPoint1.y);
        if (!moveCamera)
        {
            return;
        }

        Vector3 pos = Determine3dLoc(new Vector2(
            Input.mousePosition.x + cameraLoc.width / 4,
            Screen.height - Input.mousePosition.y + cameraLoc.height / 2));
        cameraController.transform.position = new Vector3(pos.x, cameraController.transform.position.y, pos.z);
    }

    void OnGUI()
    {
        // Saves a lot of processing
        useGUILayout = false;
        GUI.depth = 0;
        // For Moving around the Camera
        moveCamera = GUI.RepeatButton(localBounds, "") && Input.GetButton("LMB");

        // Draw the Map Background
        if (background != null)
        {
            GUI.DrawTexture(localBounds, background);
        }

        // Draw the Elements on the Map
        for (var x = 0; x < elements.Length; x++)
        {
            MiniMapElement element = elements[x];
            Vector2 halfSize = new Vector2(element.size.x / 2, element.size.y / 2);
            for (int y = 0; y < element.objAmount; y++)
            {
                Vector2 loc = element.elementLoc[y];
                if (element.elementMap[y].display)
                {
                    GUI.color = element.tints[element.elementGroup[y]];
                    GUI.DrawTexture(
                        new Rect(loc.x - halfSize.x, loc.y - halfSize.y, element.size.x, element.size.y),
                        element.image);
                }
            }
        }

        // Drawing the fog
        GUI.color = fogColor;
        GUI.DrawTexture(localBounds, fogTexture);
        if (cameraElement.image)
        {
            // Draw the Camera
            GUI.DrawTexture(cameraLoc, cameraElement.image, ScaleMode.StretchToFill);
        }
    }

    // Determines the Location on the MiniMap for the loc
    Vector2 Determine2dLoc(Vector3 loc)
    {
        return new Vector2(
            (loc.x - realWorldBounds.x) * size.width + localBounds.x,
            localBounds.y + localBounds.height - (loc.z - realWorldBounds.y) * size.height);
    }

    Vector3 Determine3dLoc(Vector2 loc)
    {
        Rect size = new Rect(
            realWorldBounds.x / localBounds.x,
            realWorldBounds.y / localBounds.y,
            realWorldBounds.width / localBounds.width,
            realWorldBounds.height / localBounds.height);
        return new Vector3(
            (loc.x - localBounds.x) * size.width,
            100,
            realWorldBounds.y + realWorldBounds.height - (loc.y - localBounds.y) * size.height);
    }
}
