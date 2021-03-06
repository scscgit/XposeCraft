using System;
using UnityEngine;
using XposeCraft.Core.Faction.Buildings;
using XposeCraft.Core.Faction.Units;

namespace XposeCraft.Core.Faction
{
    public class Health : MonoBehaviour
    {
        UnitController unit;
        BuildingController build;
        int state;
        public float yIncrease = 5;
        public int scale = 50;
        public int yScale = 6;
        public Texture2D healthBar;
        public Texture2D backgroundBar;
        public HealthElement[] element = new HealthElement[0];
        Camera cam { get; set; }

        //bool displayed = false;

        void Start()
        {
            unit = GetComponent<UnitController>();
            if (unit == null)
            {
                state = 1;
                build = GetComponent<BuildingController>();
            }
        }

        public void Display()
        {
            if (!cam)
            {
                cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            }
            Vector2 point = cam.WorldToScreenPoint(new Vector3(
                transform.position.x,
                transform.position.y + yIncrease,
                transform.position.z));
            int maxHealth;
            int health;
            if (state == 0)
            {
                maxHealth = unit.maxHealth;
                health = unit.health;
            }
            else
            {
                maxHealth = build.maxHealth;
                health = build.health;
            }
            int widthSubtraction = scale / 2;
            DisplayGUI(point, maxHealth, health, widthSubtraction);
        }

        public void DisplayGUI(Vector2 point, int maxHealth, int health, int widthSubtraction)
        {
            if (backgroundBar != null)
            {
                GUI.DrawTexture(
                    new Rect(
                        point.x - widthSubtraction,
                        Screen.height - point.y,
                        scale * (float) 1,
                        yScale),
                    backgroundBar);
            }
            if (healthBar != null)
            {
                GUI.DrawTexture(
                    new Rect(
                        point.x - widthSubtraction,
                        Screen.height - point.y,
                        scale * ((float) health / maxHealth),
                        yScale),
                    healthBar);
            }
            for (int x = 0; x < element.Length; x++)
            {
                GUI.DrawTexture(
                    new Rect(
                        point.x + element[x].loc.x - widthSubtraction,
                        Screen.height - (point.y + element[x].loc.y),
                        element[x].loc.width,
                        element[x].loc.height),
                    element[x].image);
            }
        }
    }

    [Serializable]
    public class HealthElement
    {
        public Texture2D image = null;
        public Rect loc = new Rect();
    }
}
