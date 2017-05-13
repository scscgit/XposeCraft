using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XposeCraft.Core.Faction;
using XposeCraft.Core.Faction.Buildings;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Core.Fog_Of_War;
using XposeCraft.Core.Required;
using XposeCraft.Core.Resources;
using XposeCraft.GameInternal;
using XposeCraft.UI.MiniMap;

namespace XposeCraft.RTS
{
    public class FactionEditor : EditorWindow
    {
        FactionManager target;
        Faction nTarget;
        Vector2 factionPosition;
        Vector2 factionPosition2;
        int factionState;
        int factionId;
        string factionSearch = "Search";

        Vector2 unitPosition;
        string unitSearch = "Search";
        int unitState;
        int unitId;
        GameObject lastObj;
        GUISkin skin;
        int menuState;
        int subMenuState;
        int arraySelect;
        int arraySelect1;
        int arraySelect2;
        int arraySelect3;
        int miniMapState;
        int helpState;
        int techList;
        bool techOpen;

        //int maxY;

        // The Data to display in the Help box
        string[] helpOptions =
        {
            /*0*/
            "This is the help box, this is where you will find all the information for the individual options.",
            /*1*/
            "Please have at least 2 groups to work with their relations.",
            /*2*/
            "You cannot modify relations to yourself.",
            /*3*/
            "A Strength indicates an increased amount of damage to the target type, while a weakness indicates a reduced amount of damage to the target.",
            /*4*/
            "The Technology Tree determines what techs your faction can research and what the state of those techs is at startup.",
            /*5*/
            "The Global options define what stats your unit has.",
            /*6*/
            "If you would like your unit to be able to fight, click the check box then set up his fighting settings",
            /*7*/
            "If you would like your unit to be able to build, click the check box then set up his building settings per building type",
            /*8*/
            "If you would like your unit to be able to gather resources, click the check box then set up his gathering settings per resource",
            /*9*/
            "The GUI image is the image displayed when your object is selected, while the Objects field refers to objects to activate when the object is Selected.",
            /*10*/
            "Here you can set up the unit's MiniMap GUI settings, although you will still need to change the layer on the MiniMap GameObject.",
            /*11*/
            "Please add a new GameObject with the MiniMap component to the scene.",
            /*12*/
            "Here is the Fog Of War editor, so that you can change the area of your unit's sight.",
            /*13*/
            "The Close Width and Length refer to the gridpoints to close when the Building is placed. X = Close, W = Walkable, O = Open.",
            /*14*/
            "The Drop Off Point is a building where your units can return to in order to place the resources they are carrying.",
            /*15*/
            "",
            /*16*/
            "",
            /*17*/
            "",
            /*18*/
            ""
        };

        Texture selectionTexture;
        Vector2 scrollPos;
        Editor objEditor;


        [MenuItem("Window/Faction Editor")]
        static void Init()
        {
            GetWindow(typeof(FactionEditor));
        }

        public void Update()
        {
            Repaint();
        }

        void OnGUI()
        {
            // Root GUI Display
            Vector2 defaultRect = new Vector2(1500, 750);
            Vector2 realRect = new Vector2(position.width, position.height);
            Vector2 rect = new Vector2((int) realRect.x / defaultRect.x, (int) realRect.y / defaultRect.y);
            if (!target)
            {
                target = GameObject.Find("Faction Manager").GetComponent<FactionManager>();
                selectionTexture = target.SelectionTexture;
            }
            helpState = 0;
            switch (factionState)
            {
                case 0:
                    DrawFactionGui(rect);
                    break;
                case 1:
                    if (nTarget == null)
                    {
                        break;
                    }
                    if (unitState == 0)
                    {
                        DrawUnitGui(rect);
                    }
                    else
                    {
                        DrawBuildingGui(rect);
                    }
                    break;
            }
            DrawSetup(rect);

            // Saves any changes to the Faction Manager on a Scene Save
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkAllScenesDirty();
            }
        }

        void DrawSetup(Vector2 rect)
        {
            if (factionId < 0)
            {
                factionId = 0;
            }
            if (unitId < 0)
            {
                unitId = 0;
            }
            string[] options = {"Faction Data", "Faction Objects"};
            factionState = EditorGUI.Popup(new Rect(0, 0, 200 * rect.x, 25 * rect.y), factionState, options);
            factionPosition = GUI.BeginScrollView(
                new Rect(0, 45 * rect.y, 200 * rect.x, 655 * rect.y),
                factionPosition,
                target.FactionList.Length * 20 * rect.x < 655 * rect.x
                    ? new Rect(0, 0, 200 * rect.x, 655 * rect.y)
                    : new Rect(0, 0, 200 * rect.x, target.FactionList.Length * 20 * rect.y),
                false,
                true);
            if (factionId >= target.FactionList.Length)
            {
                factionId = target.FactionList.Length - 1;
            }
            for (int x = 0; x < target.FactionList.Length; x++)
            {
                if (target.FactionList[x])
                {
                    if (isWithin(new Rect(0, x * 20 * rect.y, 200 * rect.x, 20 * rect.y)))
                    {
                        factionId = x;
                    }
                    GUI.Label(new Rect(0, x * 20 * rect.y, 200 * rect.x, 20 * rect.y), target.FactionList[x].name);
                }
                else
                {
                    if (isWithin(new Rect(0, x * 20 * rect.y, 200 * rect.x, 20 * rect.y)))
                    {
                        factionId = x;
                    }
                    target.FactionList[x] = EditorGUI.ObjectField(
                        new Rect(0, x * 20 * rect.y, 200 * rect.x, 20 * rect.y),
                        target.FactionList[x],
                        typeof(GameObject),
                        true) as GameObject;
                }
                if (factionId == x)
                {
                    GUI.DrawTexture(
                        new Rect(0, x * 20 * rect.y, 200 * rect.x, 20 * rect.y),
                        selectionTexture,
                        ScaleMode.StretchToFill);
                }
            }
            GUI.EndScrollView();
            if (target.FactionList.Length > 0)
            {
                if (target.FactionList[factionId])
                {
                    factionSearch = EditorGUI.TextField(
                        new Rect(0, 25 * rect.y, 200 * rect.x, 20 * rect.y),
                        factionSearch);
                }
                else
                {
                    GUI.Box(new Rect(0, 25 * rect.y, 200 * rect.x, 20 * rect.y), "");
                }
            }
            if (GUI.Button(new Rect(0, 700 * rect.y, 100 * rect.x, 50 * rect.y), "+"))
            {
                ModifyFactions(target.FactionList.Length + 1, target.FactionList.Length, factionId);
            }
            if (GUI.Button(new Rect(100 * rect.x, 700 * rect.y, 100 * rect.x, 50 * rect.y), "-"))
            {
                ModifyFactions(target.FactionList.Length - 1, target.FactionList.Length, factionId);
            }
            if (target.FactionList.Length > 0)
            {
                if (factionId >= target.FactionList.Length)
                {
                    factionId = target.FactionList.Length - 1;
                }
                else if (target.FactionList[factionId])
                {
                    nTarget = target.FactionList[factionId].GetComponent<Faction>();
                    if (nTarget)
                    {
                        if (factionState == 1)
                        {
                            unitSearch = EditorGUI.TextField(
                                new Rect(200 * rect.x, 0, 200 * rect.x, 25 * rect.y),
                                unitSearch);

                            if (GUI.Button(new Rect(200 * rect.x, 25 * rect.y, 100 * rect.x, 20 * rect.y), "U"))
                            {
                                unitState = 0;
                            }
                            if (GUI.Button(new Rect(300 * rect.x, 25 * rect.y, 100 * rect.x, 20 * rect.y), "B"))
                            {
                                unitState = 1;
                            }
                            switch (unitState)
                            {
                                case 0:
                                    GUI.DrawTexture(
                                        new Rect(200 * rect.x, 25 * rect.y, 100 * rect.x, 20 * rect.y),
                                        selectionTexture,
                                        ScaleMode.StretchToFill);
                                    unitPosition = GUI.BeginScrollView(
                                        new Rect(200 * rect.x, 45 * rect.y, 200 * rect.x, 655 * rect.y),
                                        unitPosition,
                                        nTarget.UnitList.Length * 20 * rect.x < 655 * rect.x
                                            ? new Rect(0, 0, 200 * rect.x, 655 * rect.y)
                                            : new Rect(0, 0, 200 * rect.x, nTarget.UnitList.Length * 20 * rect.y),
                                        false,
                                        true);
                                    if (unitId >= nTarget.UnitList.Length)
                                    {
                                        unitId = nTarget.UnitList.Length - 1;
                                    }
                                    for (int x = 0; x < nTarget.UnitList.Length; x++)
                                    {
                                        if (nTarget.UnitList[x].obj != null)
                                        {
                                            if (isWithin(
                                                new Rect(0 * rect.x, x * 20 * rect.y, 200 * rect.x, 20 * rect.y)))
                                            {
                                                unitId = x;
                                            }
                                            UnitController cont =
                                                nTarget.UnitList[x].obj.GetComponent<UnitController>();
                                            GUI.Label(
                                                new Rect(0 * rect.x, x * 20 * rect.y, 200 * rect.x, 20 * rect.y),
                                                cont ? cont.name : "New Unit");
                                        }
                                        else
                                        {
                                            if (isWithin(
                                                new Rect(0 * rect.x, x * 20 * rect.y, 200 * rect.x, 20 * rect.y)))
                                            {
                                                unitId = x;
                                            }
                                            nTarget.UnitList[x].obj = EditorGUI.ObjectField(
                                                new Rect(0 * rect.x, x * 20 * rect.y, 200 * rect.x, 20 * rect.y),
                                                nTarget.UnitList[x].obj,
                                                typeof(GameObject),
                                                true) as GameObject;
                                        }
                                        if (unitId == x)
                                        {
                                            GUI.DrawTexture(
                                                new Rect(0 * rect.x, x * 20 * rect.y, 200 * rect.x, 20 * rect.y),
                                                selectionTexture,
                                                ScaleMode.StretchToFill);
                                        }
                                    }
                                    break;
                                case 1:
                                    GUI.DrawTexture(
                                        new Rect(300 * rect.x, 25 * rect.y, 100 * rect.x, 20 * rect.y),
                                        selectionTexture,
                                        ScaleMode.StretchToFill);
                                    unitPosition = GUI.BeginScrollView(
                                        new Rect(200 * rect.x, 45 * rect.y, 200 * rect.x, 655 * rect.y),
                                        unitPosition,
                                        nTarget.BuildingList.Length * 20 * rect.x < 655 * rect.x
                                            ? new Rect(0, 0, 200 * rect.x, 655 * rect.y)
                                            : new Rect(0, 0, 200 * rect.x, nTarget.BuildingList.Length * 20 * rect.y),
                                        false,
                                        true);
                                    if (unitId >= nTarget.BuildingList.Length)
                                    {
                                        unitId = nTarget.BuildingList.Length - 1;
                                    }
                                    for (int x = 0; x < nTarget.BuildingList.Length; x++)
                                    {
                                        if (nTarget.BuildingList[x].obj != null)
                                        {
                                            if (isWithin(new Rect(0, x * 20 * rect.y, 200 * rect.x, 20 * rect.y)))
                                            {
                                                unitId = x;
                                            }
                                            BuildingController cont =
                                                nTarget.BuildingList[x].obj.GetComponent<BuildingController>();
                                            GUI.Label(
                                                new Rect(0, x * 20 * rect.y, 200 * rect.x, 20 * rect.y),
                                                cont ? cont.name : "New Building");
                                        }
                                        else
                                        {
                                            if (isWithin(new Rect(0, x * 20 * rect.y, 200 * rect.x, 20 * rect.y)))
                                            {
                                                unitId = x;
                                            }
                                            nTarget.BuildingList[x].obj = EditorGUI.ObjectField(
                                                new Rect(0, x * 20 * rect.y, 200 * rect.x, 20 * rect.y),
                                                nTarget.BuildingList[x].obj,
                                                typeof(GameObject),
                                                true) as GameObject;
                                        }
                                        if (unitId == x)
                                        {
                                            GUI.DrawTexture(
                                                new Rect(0, x * 20 * rect.y, 200 * rect.x, 20 * rect.y),
                                                selectionTexture,
                                                ScaleMode.StretchToFill);
                                        }
                                    }
                                    break;
                            }
                            GUI.EndScrollView();
                            if (GUI.Button(new Rect(200 * rect.x, 700 * rect.y, 100 * rect.x, 50 * rect.y), "+"))
                            {
                                switch (unitState)
                                {
                                    case 0:
                                        ModifyFactionUnits(
                                            nTarget.UnitList.Length + 1,
                                            nTarget.UnitList.Length,
                                            unitId);
                                        break;
                                    case 1:
                                        ModifyFactionBuildings(
                                            nTarget.BuildingList.Length + 1,
                                            nTarget.BuildingList.Length,
                                            unitId);
                                        break;
                                }
                            }
                            if (GUI.Button(new Rect(300 * rect.x, 700 * rect.y, 100 * rect.x, 50 * rect.y), "-"))
                            {
                                switch (unitState)
                                {
                                    case 0:
                                        ModifyFactionUnits(
                                            nTarget.UnitList.Length - 1,
                                            nTarget.UnitList.Length,
                                            unitId);
                                        unitId--;
                                        break;
                                    case 1:
                                        ModifyFactionBuildings(
                                            nTarget.BuildingList.Length - 1,
                                            nTarget.BuildingList.Length,
                                            unitId);
                                        unitId--;
                                        break;
                                }
                            }
                        }
                    }
                    else if (GUI.Button(new Rect(200 * rect.x, 0 * rect.y, 1300 * rect.x, 750 * rect.y), "+"))
                    {
                        target.FactionList[factionId].AddComponent<Faction>();
                        //InitializeFactionRelations();
                    }
                }
            }
            GUI.Box(
                factionState == 0
                    ? new Rect(200 * rect.x, 700 * rect.y, 1300 * rect.x, 50 * rect.y)
                    : new Rect(400 * rect.x, 700 * rect.y, 1100 * rect.x, 50 * rect.y),
                helpOptions[helpState]);
        }

        void DrawFactionGui(Vector2 rect)
        {
            GUI.Box(new Rect(200 * rect.x, 25 * rect.y, 1300 * rect.x, 50 * rect.y), "");
            if (GUI.Button(new Rect(200 * rect.x, 0, 325 * rect.x, 25 * rect.y), "Global"))
            {
                menuState = 0;
                subMenuState = 0;
            }
            if (GUI.Button(new Rect(525 * rect.x, 0, 325 * rect.x, 25 * rect.y), "Relations"))
            {
                menuState = 1;
                subMenuState = 0;
            }
            if (GUI.Button(new Rect(850 * rect.x, 0, 325 * rect.x, 25 * rect.y), "Techs"))
            {
                menuState = 2;
                subMenuState = 0;
            }
            if (GUI.Button(new Rect(1175 * rect.x, 0, 325 * rect.x, 25 * rect.y), "Unit Types"))
            {
                menuState = 3;
                subMenuState = 0;
            }

            switch (menuState)
            {
                // Global
                case 0:
                    DrawFactionGuiGlobal(rect);
                    break;
                // Relations
                case 1:
                    DrawFactionGuiRelations(rect);
                    break;
                // Techs
                case 2:
                    DrawFactionGuiTechs(rect);
                    break;
                // Unit Types
                case 3:
                    DrawFactionGuiUnitTypes(rect);
                    break;
            }
        }

        private void DrawFactionGuiGlobal(Vector2 rect)
        {
            GUI.DrawTexture(
                new Rect(200 * rect.x, 0, 325 * rect.x, 25 * rect.y),
                selectionTexture,
                ScaleMode.StretchToFill);
            if (factionId < 0)
            {
                factionId = 0;
            }
            // Prevent value getting out of sync when Factions get externally modified
            if (factionId >= target.FactionList.Length)
            {
                factionId = target.FactionList.Length - 1;
            }
            if (target.FactionList.Length <= 0)
            {
                factionId = 0;
                return;
            }
            if (target.FactionList[factionId] != null)
            {
                target.FactionList[factionId].name = EditorGUI.TextField(
                    new Rect(200 * rect.x, 95 * rect.y, 1300 * rect.x, 25 * rect.y),
                    "Name : ",
                    target.FactionList[factionId].name);
            }
        }

        private void DrawFactionGuiRelations(Vector2 rect)
        {
            if (nTarget == null)
            {
                return;
            }
            GUI.DrawTexture(
                new Rect(525 * rect.x, 0, 325 * rect.x, 25 * rect.y),
                selectionTexture,
                ScaleMode.StretchToFill);
            InitializeFactionRelations();
            if (target.FactionList.Length == 1)
            {
                helpState = 1;
                return;
            }
            string[] factionNames = new string[target.FactionList.Length];
            int selfLocation = factionId;
            for (int x = 0; x < target.FactionList.Length; x++)
            {
                if (target.FactionList[x] != null)
                {
                    factionNames[x] = (x + 1) + ". " + target.FactionList[x].name;
                }
            }
            arraySelect = EditorGUI.Popup(
                new Rect(200 * rect.x, 95 * rect.y, 1300 * rect.x, 25 * rect.y),
                "Faction : ",
                arraySelect,
                factionNames);
            // Preventing occassional illegal selection that occurred when Factions got removed
            if (arraySelect >= nTarget.Relations.Length)
            {
                arraySelect = nTarget.Relations.Length - 1;
            }
            if (arraySelect != selfLocation)
            {
                string[] options = {"Ally", "Neutral", "Enemy"};
                nTarget.Relations[arraySelect].state = EditorGUI.Popup(
                    new Rect(200 * rect.x, 120 * rect.y, 1300 * rect.x, 25 * rect.y),
                    "Relation : ",
                    nTarget.Relations[arraySelect].state,
                    options);
            }
            else
            {
                helpState = 2;
            }
        }

        /// <summary>
        /// Adds or removes Relations of Factions when their number changes.
        /// Instead of updating only a current Faction, iterates through them all to prevent arrays getting out of bounds.
        /// </summary>
        private void InitializeFactionRelations()
        {
            var previous = nTarget;
            foreach (GameObject factionObject in target.FactionList)
            {
                if (factionObject == null)
                {
                    continue;
                }
                nTarget = factionObject.GetComponent<Faction>();
                if (nTarget == null)
                {
                    continue;
                }
                if (nTarget.Relations.Length != target.FactionList.Length)
                {
                    ModifyFactionRelations(target.FactionList.Length, nTarget.Relations.Length);
                }
                // Hotfix for Relations not getting initialized soon enough
                for (var index = 0; index < nTarget.Relations.Length; index++)
                {
                    if (nTarget.Relations[index] == null)
                    {
                        nTarget.Relations[index] = new Relation();
                    }
                }
            }
            nTarget = previous;
        }

        private void DrawFactionGuiTechs(Vector2 rect)
        {
            if (nTarget == null)
            {
                return;
            }
            GUI.DrawTexture(
                new Rect(850 * rect.x, 0, 325 * rect.x, 25 * rect.y),
                selectionTexture,
                ScaleMode.StretchToFill);
            helpState = 4;

            if (nTarget.Tech.Length > 0)
            {
                //int size = nTarget.tech.Length;
                Technology[] techs = nTarget.Tech;
                //if (maxY <= 620) {
                factionPosition2 = GUI.BeginScrollView(
                    new Rect(200 * rect.x, 80 * rect.y, 1300 * rect.x, 620 * rect.y),
                    factionPosition2,
                    new Rect(0, 0, 200 * rect.x, 655 * rect.y),
                    false,
                    true);
                //} else {
                //    factionPosition2 = GUI.BeginScrollView(new Rect(200 * rect.x, 80 * rect.y, 1300 * rect.x, 620 * rect.y),
                //        factionPosition2, new Rect(0, 0, 200 * rect.x, maxY * rect.y), false, true);
                //}
                int curY = 0;
                if (GUI.Button(new Rect(0 * rect.x, curY * rect.y, 650 * rect.x, 20 * rect.y), "Add"))
                {
                    nTarget.Tech = ModifyTechs(nTarget.Tech.Length + 1, nTarget.Tech.Length, nTarget.Tech);
                }
                if (GUI.Button(new Rect(650 * rect.x, curY * rect.y, 650 * rect.x, 20 * rect.y), "Remove"))
                {
                    nTarget.Tech = ModifyTechs(nTarget.Tech.Length - 1, nTarget.Tech.Length, nTarget.Tech);
                }
                curY += 25;
                string[] techNames = new string[techs.Length];
                for (int z = 0; z < techs.Length; z++)
                {
                    techNames[z] = (z + 1) + ". " + techs[z].name;
                }
                if (techList >= techs.Length)
                {
                    techList = 0;
                }
                techList = EditorGUI.Popup(
                    new Rect(0 * rect.x, curY * rect.y, 1300 * rect.x, 20 * rect.y),
                    "Tech : ",
                    techList,
                    techNames);
                curY += 25;
                techOpen = EditorGUI.Foldout(
                    new Rect(0 * rect.x, curY * rect.y, 1300 * rect.x, 20 * rect.y),
                    techOpen,
                    "Info : ");
                curY += 25;
                if (techOpen)
                {
                    techs[techList].name = EditorGUI.TextField(
                        new Rect(0 * rect.x, curY * rect.y, 1300 * rect.x, 20 * rect.y),
                        "Name : ",
                        techs[techList].name);
                    curY += 25;
                    EditorGUI.LabelField(
                        new Rect(0 * rect.x, curY * rect.y, 1300 * rect.x, 20 * rect.y),
                        "Texture : ");
                    curY += 20;
                    techs[techList].texture = EditorGUI.ObjectField(
                        new Rect(0 * rect.x, curY * rect.y, 100 * rect.x, 100 * rect.y),
                        "",
                        techs[techList].texture,
                        typeof(Texture2D),
                        true) as Texture2D;
                    curY += 80;
                    techs[techList].active = EditorGUI.Toggle(
                        new Rect(0 * rect.x, curY * rect.y, 1300 * rect.x, 20 * rect.y),
                        "Active : ",
                        techs[techList].active);
                    //curY += 25;
                }
                GUI.EndScrollView();
            }
            else
            {
                if (GUI.Button(new Rect(200 * rect.x, 80 * rect.y, 650 * rect.x, 20 * rect.y), "Add"))
                {
                    nTarget.Tech = ModifyTechs(nTarget.Tech.Length + 1, nTarget.Tech.Length, nTarget.Tech);
                }
                if (GUI.Button(new Rect(850 * rect.x, 80 * rect.y, 650 * rect.x, 20 * rect.y), "Remove"))
                {
                    nTarget.Tech = ModifyTechs(nTarget.Tech.Length - 1, nTarget.Tech.Length, nTarget.Tech);
                }
            }
        }

        private void DrawFactionGuiUnitTypes(Vector2 rect)
        {
            helpState = 3;
            GUI.DrawTexture(
                new Rect(1175 * rect.x, 0, 325 * rect.x, 25 * rect.y),
                selectionTexture,
                ScaleMode.StretchToFill);
            int curY = 95;
            if (target.UnitTypes.Length > 0)
            {
                string[] typeNames = new string[target.UnitTypes.Length];
                for (int x = 0; x < target.UnitTypes.Length; x++)
                {
                    typeNames[x] = (x + 1) + ". " + target.UnitTypes[x].name;
                }
                if (GUI.Button(new Rect(200 * rect.x, curY * rect.y, 650 * rect.x, 15 * rect.y), "Add Type"))
                {
                    ModifyFactionTypes(target.UnitTypes.Length + 1, target.UnitTypes.Length, arraySelect);
                }
                if (GUI.Button(new Rect(850 * rect.x, curY * rect.y, 650 * rect.x, 15 * rect.y), "Remove Type"))
                {
                    ModifyFactionTypes(target.UnitTypes.Length - 1, target.UnitTypes.Length, arraySelect);
                }
                curY += 20;
                arraySelect = EditorGUI.Popup(
                    new Rect(200 * rect.x, curY * rect.y, 1300 * rect.x, 25 * rect.y),
                    "Type : ",
                    arraySelect,
                    typeNames);
                curY += 15;
                if (arraySelect >= target.UnitTypes.Length || arraySelect < 0)
                {
                    arraySelect = 0;
                }
                target.UnitTypes[arraySelect].name = EditorGUI.TextField(
                    new Rect(200 * rect.x, curY * rect.y, 1300 * rect.x, 25 * rect.y),
                    "Type Name : ",
                    target.UnitTypes[arraySelect].name);
                curY += 40;
                if (target.UnitTypes[arraySelect].strengths.Length > 0)
                {
                    string[] strengthName = new string[target.UnitTypes[arraySelect].strengths.Length];
                    if (arraySelect1 >= target.UnitTypes[arraySelect].strengths.Length)
                    {
                        arraySelect1 = target.UnitTypes[arraySelect].strengths.Length - 1;
                    }
                    for (int x = 0; x < target.UnitTypes[arraySelect].strengths.Length; x++)
                    {
                        strengthName[x] = (x + 1) + ". " + target.UnitTypes[arraySelect].strengths[x].name;
                    }
                    GUI.Box(new Rect(200 * rect.x, curY * rect.y - 3, 1300 * rect.x, 130 * rect.y), "");
                    if (GUI.Button(new Rect(200 * rect.x, curY * rect.y, 650 * rect.x, 15 * rect.y), "Add Strength"))
                    {
                        ModifyFactionTypesStrengths(
                            target.UnitTypes[arraySelect].strengths.Length + 1,
                            target.UnitTypes[arraySelect].strengths.Length,
                            arraySelect1,
                            arraySelect);
                    }
                    if (GUI.Button(new Rect(850 * rect.x, curY * rect.y, 650 * rect.x, 15 * rect.y), "Remove Strength"))
                    {
                        ModifyFactionTypesStrengths(
                            target.UnitTypes[arraySelect].strengths.Length - 1,
                            target.UnitTypes[arraySelect].strengths.Length,
                            arraySelect1,
                            arraySelect);
                    }
                    curY += 20;
                    arraySelect1 = EditorGUI.Popup(
                        new Rect(200 * rect.x, curY * rect.y, 1300 * rect.x, 25 * rect.y),
                        "Strength : ",
                        arraySelect1,
                        strengthName);
                    curY += 15;
                    if (arraySelect1 >= target.UnitTypes[arraySelect].strengths.Length || arraySelect1 < 0)
                    {
                        arraySelect1 = 0;
                    }
                    if (target.UnitTypes[arraySelect].strengths.Length > 0)
                    {
                        target.UnitTypes[arraySelect].strengths[arraySelect1].name = EditorGUI.TextField(
                            new Rect(200 * rect.x, curY * rect.y, 1300 * rect.x, 25 * rect.y),
                            "Name : ",
                            target.UnitTypes[arraySelect].strengths[arraySelect1].name);
                        curY += 30;
                        target.UnitTypes[arraySelect].strengths[arraySelect1].target = EditorGUI.Popup(
                            new Rect(200 * rect.x, curY * rect.y, 1300 * rect.x, 25 * rect.y),
                            "Target : ",
                            target.UnitTypes[arraySelect].strengths[arraySelect1].target,
                            typeNames);
                        target.UnitTypes[arraySelect].strengths[arraySelect1].targetName =
                            typeNames[target.UnitTypes[arraySelect].strengths[arraySelect1].target];
                        curY += 30;
                        target.UnitTypes[arraySelect].strengths[arraySelect1].amount = EditorGUI.FloatField(
                            new Rect(200 * rect.x, curY * rect.y, 1300 * rect.x, 25 * rect.y),
                            "Ratio : ",
                            target.UnitTypes[arraySelect].strengths[arraySelect1].amount);
                        curY += 30;
                    }
                }
                else
                {
                    GUI.Box(new Rect(200 * rect.x, curY * rect.y - 3, 1300 * rect.x, 26 * rect.y), "");
                    if (GUI.Button(new Rect(200 * rect.x, curY * rect.y, 1300 * rect.x, 15 * rect.y), "Add Strength"))
                    {
                        ModifyFactionTypesStrengths(
                            target.UnitTypes[arraySelect].strengths.Length + 1,
                            target.UnitTypes[arraySelect].strengths.Length,
                            arraySelect1,
                            arraySelect);
                    }
                    curY += 20;
                }
                curY += 10;
                if (target.UnitTypes[arraySelect].weaknesses.Length > 0)
                {
                    string[] weaknessName = new string[target.UnitTypes[arraySelect].weaknesses.Length];
                    if (arraySelect2 >= target.UnitTypes[arraySelect].weaknesses.Length || arraySelect2 < 0)
                    {
                        arraySelect2 = 0;
                    }
                    for (int x = 0; x < target.UnitTypes[arraySelect].weaknesses.Length; x++)
                    {
                        weaknessName[x] = (x + 1) + ". " + target.UnitTypes[arraySelect].weaknesses[x].name;
                    }
                    GUI.Box(new Rect(200 * rect.x, curY * rect.y - 3, 1300 * rect.x, 130 * rect.y), "");
                    if (GUI.Button(new Rect(200 * rect.x, curY * rect.y, 650 * rect.x, 15 * rect.y), "Add Weakness"))
                    {
                        ModifyFactionTypesWeaknesses(
                            target.UnitTypes[arraySelect].weaknesses.Length + 1,
                            target.UnitTypes[arraySelect].weaknesses.Length,
                            arraySelect2,
                            arraySelect);
                    }
                    if (GUI.Button(new Rect(850 * rect.x, curY * rect.y, 650 * rect.x, 15 * rect.y), "Remove Weakness"))
                    {
                        ModifyFactionTypesWeaknesses(
                            target.UnitTypes[arraySelect].weaknesses.Length - 1,
                            target.UnitTypes[arraySelect].weaknesses.Length,
                            arraySelect2,
                            arraySelect);
                    }
                    curY += 20;
                    arraySelect2 = EditorGUI.Popup(
                        new Rect(200 * rect.x, curY * rect.y, 1300 * rect.x, 25 * rect.y),
                        "Weakness : ",
                        arraySelect2,
                        weaknessName);
                    curY += 15;
                    target.UnitTypes[arraySelect].weaknesses[arraySelect2].name = EditorGUI.TextField(
                        new Rect(200 * rect.x, curY * rect.y, 1300 * rect.x, 25 * rect.y),
                        "Name : ",
                        target.UnitTypes[arraySelect].weaknesses[arraySelect2].name);
                    curY += 30;
                    target.UnitTypes[arraySelect].weaknesses[arraySelect2].target = EditorGUI.Popup(
                        new Rect(200 * rect.x, curY * rect.y, 1300 * rect.x, 25 * rect.y),
                        "Target : ",
                        target.UnitTypes[arraySelect].weaknesses[arraySelect2].target,
                        typeNames);
                    curY += 30;
                    target.UnitTypes[arraySelect].weaknesses[arraySelect2].amount = EditorGUI.FloatField(
                        new Rect(200 * rect.x, curY * rect.y, 1300 * rect.x, 25 * rect.y),
                        "Ratio : ",
                        target.UnitTypes[arraySelect].weaknesses[arraySelect2].amount);
                    //curY += 30;
                }
                else
                {
                    GUI.Box(new Rect(200 * rect.x, curY * rect.y - 3, 1300 * rect.x, 26 * rect.y), "");
                    if (GUI.Button(new Rect(200 * rect.x, curY * rect.y, 1300 * rect.x, 15 * rect.y), "Add Weakness"))
                    {
                        ModifyFactionTypesWeaknesses(
                            target.UnitTypes[arraySelect].weaknesses.Length + 1,
                            target.UnitTypes[arraySelect].weaknesses.Length,
                            arraySelect2,
                            arraySelect);
                    }
                    //curY += 20;
                }
            }
            else if (GUI.Button(new Rect(200 * rect.x, curY * rect.y, 1300 * rect.x, 25 * rect.y), "+"))
            {
                ModifyFactionTypes(target.UnitTypes.Length + 1, target.UnitTypes.Length, arraySelect);
            }
        }

        void DrawUnitGui(Vector2 rect)
        {
            if (unitId >= nTarget.UnitList.Length || unitId < 0)
            {
                unitId = 0;
                return;
            }
            if (nTarget.UnitList.Length <= 0 || !nTarget.UnitList[unitId].obj)
            {
                return;
            }
            Unit targetUnit = nTarget.UnitList[unitId];
            // Saves any changes on a Scene Save
            EditorUtility.SetDirty(targetUnit.obj);

            MiniMapSignal myTargetMap = targetUnit.obj.GetComponent<MiniMapSignal>();
            VisionSignal myTargetVision = targetUnit.obj.GetComponent<VisionSignal>();
            UnitController myTarget = targetUnit.obj.GetComponent<UnitController>();
            UnitMovement moveTarget = targetUnit.obj.GetComponent<UnitMovement>();
            // If the Unit does not have the base unit scripts, give the user the option to add them
            if (!myTarget)
            {
                if (GUI.Button(
                    new Rect(400 * rect.x, 0, 1100 * rect.x, 750 * rect.y),
                    "Add Unit Controller Components"))
                {
                    myTarget = targetUnit.obj.AddComponent<UnitController>();
                    myTarget.gui.SetType("Unit");
                    // Add More Components
                }
                return;
            }
            if (targetUnit.obj != lastObj)
            {
                objEditor = Editor.CreateEditor(targetUnit.obj);
                lastObj = targetUnit.obj;
            }
            objEditor.OnInteractivePreviewGUI(
                new Rect(945 * rect.x, 50 * rect.y, 555 * rect.x, 650 * rect.y),
                EditorStyles.toolbarButton);

            // Button Display Area

            if (GUI.Button(new Rect(400 * rect.x, 0, 275 * rect.x, 25 * rect.y), "Stats"))
            {
                menuState = 0;
                subMenuState = 0;
            }
            if (GUI.Button(new Rect(675 * rect.x, 0, 275 * rect.x, 25 * rect.y), "Visuals"))
            {
                menuState = 1;
                subMenuState = 0;
            }
            if (GUI.Button(new Rect(950 * rect.x, 0, 275 * rect.x, 25 * rect.y), "Techs"))
            {
                menuState = 2;
                subMenuState = 0;
            }
            if (GUI.Button(new Rect(1225 * rect.x, 0, 275 * rect.x, 25 * rect.y), "Anim/Sounds"))
            {
                menuState = 3;
                subMenuState = 0;
            }

            // Menu Area

            // The Stats Area
            if (menuState == 0)
            {
                GUI.DrawTexture(
                    new Rect(400 * rect.x, 0, 275 * rect.x, 25 * rect.y),
                    selectionTexture,
                    ScaleMode.StretchToFill);
                if (GUI.Button(new Rect(400 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y), "Global"))
                {
                    subMenuState = 0;
                    arraySelect = 0;
                }
                if (GUI.Button(new Rect(620 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y), "Weapon"))
                {
                    subMenuState = 1;
                    arraySelect = 0;
                }
                if (GUI.Button(new Rect(840 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y), "Build"))
                {
                    subMenuState = 2;
                    arraySelect = 0;
                }
                if (GUI.Button(new Rect(1060 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y), "Gather"))
                {
                    subMenuState = 3;
                    arraySelect = 0;
                }
                if (GUI.Button(new Rect(1280 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y), "Ratios"))
                {
                    subMenuState = 4;
                    arraySelect = 0;
                }
                // Global
                if (subMenuState == 0)
                {
                    helpState = 5;
                    GUI.DrawTexture(
                        new Rect(400 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y),
                        selectionTexture,
                        ScaleMode.StretchToFill);
                    myTarget.name = EditorGUI.TextField(
                        new Rect(400 * rect.x, 70 * rect.y, 540 * rect.x, 25 * rect.y),
                        "Name : ",
                        myTarget.name);
                    myTarget.maxHealth = EditorGUI.IntField(
                        new Rect(400 * rect.x, 100 * rect.y, 540 * rect.x, 25 * rect.y),
                        "Max Health : ",
                        myTarget.maxHealth);
                    myTarget.health = EditorGUI.IntField(
                        new Rect(400 * rect.x, 130 * rect.y, 540 * rect.x, 25 * rect.y),
                        "Health : ",
                        myTarget.health);
                    moveTarget.speed = EditorGUI.IntField(
                        new Rect(400 * rect.x, 160 * rect.y, 540 * rect.x, 25 * rect.y),
                        "Speed : ",
                        moveTarget.speed);
                    moveTarget.rotateSpeed = EditorGUI.IntField(
                        new Rect(400 * rect.x, 190 * rect.y, 540 * rect.x, 25 * rect.y),
                        "Rotate Speed : ",
                        moveTarget.rotateSpeed);
                    if (target.UnitTypes.Length > 0)
                    {
                        string[] typeNames = new string[target.UnitTypes.Length];
                        for (int x = 0; x < target.UnitTypes.Length; x++)
                        {
                            typeNames[x] = (x + 1) + ". " + target.UnitTypes[x].name;
                            if (target.UnitTypes[x] == myTarget.type)
                            {
                                arraySelect = x;
                            }
                        }
                        arraySelect = EditorGUI.Popup(
                            new Rect(400 * rect.x, 220 * rect.y, 545 * rect.x, 25 * rect.y),
                            "Type : ",
                            arraySelect,
                            typeNames);
                        myTarget.type = target.UnitTypes[arraySelect];
                    }
                    targetUnit.obj = EditorGUI.ObjectField(
                        new Rect(400 * rect.x, 250 * rect.y, 540 * rect.x, 25 * rect.y),
                        "Object : ",
                        targetUnit.obj,
                        typeof(GameObject),
                        true) as GameObject;

                    //Add In Unit Movement Stats
                }
                // Weapon
                else if (subMenuState == 1)
                {
                    helpState = 6;
                    GUI.DrawTexture(
                        new Rect(620 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y),
                        selectionTexture,
                        ScaleMode.StretchToFill);
                    myTarget.weapon.fighterUnit = EditorGUI.Toggle(
                        new Rect(400 * rect.x, 70 * rect.y, 540 * rect.x, 25 * rect.y),
                        "Fighter : ",
                        myTarget.weapon.fighterUnit);
                    if (myTarget.weapon.fighterUnit)
                    {
                        myTarget.weapon.attackRate = EditorGUI.FloatField(
                            new Rect(400 * rect.x, 100 * rect.y, 540 * rect.x, 25 * rect.y),
                            "Attack Rate : ",
                            myTarget.weapon.attackRate);
                        myTarget.weapon.attackRange = EditorGUI.FloatField(
                            new Rect(400 * rect.x, 130 * rect.y, 540 * rect.x, 25 * rect.y),
                            "Attack Range : ",
                            myTarget.weapon.attackRange);
                        myTarget.weapon.attackDamage = EditorGUI.IntField(
                            new Rect(400 * rect.x, 160 * rect.y, 540 * rect.x, 25 * rect.y),
                            "Attack Damage : ",
                            myTarget.weapon.attackDamage);
                        myTarget.weapon.lookRange = EditorGUI.FloatField(
                            new Rect(400 * rect.x, 190 * rect.y, 540 * rect.x, 25 * rect.y),
                            "Look Range : ",
                            myTarget.weapon.lookRange);
                        myTarget.weapon.attackObj = EditorGUI.ObjectField(
                            new Rect(400 * rect.x, 220 * rect.y, 540 * rect.x, 25 * rect.y),
                            "Attack Object : ",
                            myTarget.weapon.attackObj,
                            typeof(GameObject),
                            true) as GameObject;
                    }
                }
                // Build
                else if (subMenuState == 2)
                {
                    helpState = 7;
                    GUI.DrawTexture(
                        new Rect(840 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y),
                        selectionTexture,
                        ScaleMode.StretchToFill);
                    myTarget.build.builderUnit = EditorGUI.Toggle(
                        new Rect(400 * rect.x, 70 * rect.y, 540 * rect.x, 25 * rect.y),
                        "Builder : ",
                        myTarget.build.builderUnit);
                    if (!myTarget.build.builderUnit)
                    {
                        return;
                    }
                    if (myTarget.build.build.Length != nTarget.BuildingList.Length)
                    {
                        ModifyUnitBuildBehaviour(nTarget.BuildingList.Length, myTarget.build.build.Length, myTarget);
                    }
                    else
                    {
                        string[] buildNames = new string[nTarget.BuildingList.Length];
                        for (int x = 0; x < buildNames.Length; x++)
                        {
                            if (nTarget.BuildingList[x].obj != null)
                            {
                                BuildingController cont =
                                    nTarget.BuildingList[x].obj.GetComponent<BuildingController>();
                                if (cont != null)
                                {
                                    buildNames[x] = cont.name;
                                }
                            }
                        }
                        if (arraySelect >= buildNames.Length)
                        {
                            arraySelect = 0;
                        }
                        else
                        {
                            arraySelect = EditorGUI.Popup(
                                new Rect(400 * rect.x, 100 * rect.y, 540 * rect.x, 25 * rect.y),
                                arraySelect,
                                buildNames);
                            if (nTarget.BuildingList.Length > 0)
                            {
                                GUI.Box(
                                    myTarget.build.build[arraySelect].canBuild
                                        ? new Rect(400 * rect.x, 130 * rect.y, 540 * rect.x, 90 * rect.y)
                                        : new Rect(400 * rect.x, 130 * rect.y, 540 * rect.x, 25 * rect.y),
                                    "");
                                myTarget.build.build[arraySelect].canBuild = EditorGUI.Toggle(
                                    new Rect(400 * rect.x, 130 * rect.y, 540 * rect.x, 25 * rect.y),
                                    "Can Build : ",
                                    myTarget.build.build[arraySelect].canBuild);
                                if (myTarget.build.build[arraySelect].canBuild)
                                {
                                    myTarget.build.build[arraySelect].amount = EditorGUI.IntField(
                                        new Rect(400 * rect.x, 160 * rect.y, 540 * rect.x, 25 * rect.y),
                                        "Build Amount : ",
                                        myTarget.build.build[arraySelect].amount);
                                    myTarget.build.build[arraySelect].rate = EditorGUI.FloatField(
                                        new Rect(400 * rect.x, 190 * rect.y, 540 * rect.x, 25 * rect.y),
                                        "Build Rate : ",
                                        myTarget.build.build[arraySelect].rate);
                                }
                            }
                        }
                    }
                }
                // Gather
                else if (subMenuState == 3)
                {
                    helpState = 8;
                    GUI.DrawTexture(
                        new Rect(1060 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y),
                        selectionTexture,
                        ScaleMode.StretchToFill);
                    myTarget.resource.resourceUnit = EditorGUI.Toggle(
                        new Rect(400 * rect.x, 70 * rect.y, 540 * rect.x, 25 * rect.y),
                        "Gatherer : ",
                        myTarget.resource.resourceUnit);
                    if (!myTarget.resource.resourceUnit)
                    {
                        return;
                    }
                    // Current Resource Manager will be used for read-only purposes
                    ResourceManager resourceManager = GameManager.Instance.CurrentPlayerResourceManager;
                    if (myTarget.resource.behaviour.Length != resourceManager.resourceTypes.Length)
                    {
                        ModifyUnitGathering(
                            resourceManager.resourceTypes.Length,
                            myTarget.resource.behaviour.Length,
                            myTarget);
                    }
                    else
                    {
                        string[] resourceNames = new string[resourceManager.resourceTypes.Length];
                        for (int x = 0; x < resourceNames.Length; x++)
                        {
                            resourceNames[x] = resourceManager.resourceTypes[x].name;
                        }
                        if (arraySelect >= resourceNames.Length)
                        {
                            arraySelect = 0;
                        }
                        else
                        {
                            var resourceBehaviour = myTarget.resource.behaviour[arraySelect];
                            arraySelect = EditorGUI.Popup(
                                new Rect(400 * rect.x, 100 * rect.y, 540 * rect.x, 25 * rect.y),
                                arraySelect,
                                resourceNames);
                            GUI.Box(
                                resourceBehaviour.canGather
                                    ? new Rect(400 * rect.x, 130 * rect.y, 540 * rect.x, 150 * rect.y)
                                    : new Rect(400 * rect.x, 130 * rect.y, 540 * rect.x, 25 * rect.y),
                                "");
                            resourceBehaviour.canGather = EditorGUI.Toggle(
                                new Rect(400 * rect.x, 130 * rect.y, 540 * rect.x, 25 * rect.y),
                                "Can Gather : ",
                                resourceBehaviour.canGather);
                            if (resourceBehaviour.canGather)
                            {
                                resourceBehaviour.amount = EditorGUI.IntField(
                                    new Rect(400 * rect.x, 160 * rect.y, 540 * rect.x, 25 * rect.y),
                                    "Gather Amount : ",
                                    resourceBehaviour.amount);
                                resourceBehaviour.rate = EditorGUI.FloatField(
                                    new Rect(400 * rect.x, 190 * rect.y, 540 * rect.x, 25 * rect.y),
                                    "Gather Rate : ",
                                    resourceBehaviour.rate);
                                resourceBehaviour.returnWhenFull = EditorGUI.Toggle(
                                    new Rect(400 * rect.x, 220 * rect.y, 540 * rect.x, 25 * rect.y),
                                    "Drop Off Resource : ",
                                    resourceBehaviour.returnWhenFull);
                            }
                            resourceBehaviour.carryCapacity = EditorGUI.IntField(
                                new Rect(400 * rect.x, 250 * rect.y, 540 * rect.x, 25 * rect.y),
                                "Carry Capacity : ",
                                resourceBehaviour.carryCapacity);
                        }
                    }
                }
                // Ratios
                else if (subMenuState == 4)
                {
                    GUI.DrawTexture(
                        new Rect(1280 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y),
                        selectionTexture,
                        ScaleMode.StretchToFill);
                    if (GUI.Button(new Rect(400 * rect.x, 50 * rect.y, 273 * rect.x, 20 * rect.y), "Add"))
                    {
                        ModifyUnitRatios(myTarget.ratio.Length + 1, myTarget.ratio.Length, myTarget);
                    }
                    if (GUI.Button(new Rect(673 * rect.x, 50 * rect.y, 272 * rect.x, 20 * rect.y), "Remove"))
                    {
                        ModifyUnitRatios(myTarget.ratio.Length - 1, myTarget.ratio.Length, myTarget);
                    }
                    if (myTarget.ratio.Length > 0)
                    {
                        string[] names = new string[myTarget.ratio.Length];
                        for (int x = 0; x < names.Length; x++)
                        {
                            names[x] = "" + (x + 1) + ". " + myTarget.ratio[x].name;
                        }
                        if (arraySelect < names.Length)
                        {
                            arraySelect = EditorGUI.Popup(
                                new Rect(400 * rect.x, 75 * rect.y, 545 * rect.x, 20 * rect.y),
                                "Ratio : ",
                                arraySelect,
                                names);
                            myTarget.ratio[arraySelect].name = EditorGUI.TextField(
                                new Rect(400 * rect.x, 100 * rect.y, 545 * rect.x, 20 * rect.y),
                                "Enemy Name : ",
                                myTarget.ratio[arraySelect].name);
                            myTarget.ratio[arraySelect].amount = EditorGUI.FloatField(
                                new Rect(400 * rect.x, 125 * rect.y, 545 * rect.x, 20 * rect.y),
                                "Amount : ",
                                myTarget.ratio[arraySelect].amount);
                        }
                        else
                        {
                            arraySelect = 0;
                        }
                    }
                }
            }
            // The Visuals Area
            else if (menuState == 1)
            {
                GUI.DrawTexture(
                    new Rect(675 * rect.x, 0, 275 * rect.x, 25 * rect.y),
                    selectionTexture,
                    ScaleMode.StretchToFill);
                if (GUI.Button(new Rect(400 * rect.x, 25 * rect.y, 367 * rect.x, 20 * rect.y), "Selected"))
                {
                    subMenuState = 0;
                    arraySelect = 0;
                }
                if (GUI.Button(new Rect(767 * rect.x, 25 * rect.y, 367 * rect.x, 20 * rect.y), "MiniMap"))
                {
                    subMenuState = 1;
                    arraySelect = 0;
                }
                if (GUI.Button(new Rect(1134 * rect.x, 25 * rect.y, 366 * rect.x, 20 * rect.y), "Vision"))
                {
                    subMenuState = 2;
                    arraySelect = 0;
                }
                // Selected
                if (subMenuState == 0)
                {
                    GUI.DrawTexture(
                        new Rect(400 * rect.x, 25 * rect.y, 367 * rect.x, 20 * rect.y),
                        selectionTexture,
                        ScaleMode.StretchToFill);
                    myTarget.gui.SetType("Unit");
                    EditorGUI.LabelField(
                        new Rect(400 * rect.x, 50 * rect.y, 545 * rect.x, 100 * rect.y),
                        "GUI Image : ");
                    myTarget.gui.image = EditorGUI.ObjectField(
                        new Rect(400 * rect.x, 80 * rect.y, 100 * rect.x, 100 * rect.y),
                        "",
                        myTarget.gui.image,
                        typeof(Texture2D),
                        true) as Texture2D;
                    //Modify Selected Objects
                    if (GUI.Button(new Rect(400 * rect.x, 180 * rect.y, 273 * rect.x, 25 * rect.y), "Add"))
                    {
                        ModifyUnitSelectedObjects(
                            myTarget.gui.selectObjs.Length + 1,
                            myTarget.gui.selectObjs.Length,
                            myTarget);
                    }
                    if (GUI.Button(new Rect(673 * rect.x, 180 * rect.y, 272 * rect.x, 25 * rect.y), "Remove"))
                    {
                        ModifyUnitSelectedObjects(
                            myTarget.gui.selectObjs.Length - 1,
                            myTarget.gui.selectObjs.Length,
                            myTarget);
                    }
                    string[] list = new string[myTarget.gui.selectObjs.Length];
                    for (int x = 0; x < list.Length; x++)
                    {
                        if (myTarget.gui.selectObjs[x])
                        {
                            list[x] = (x + 1) + ". " + myTarget.gui.selectObjs[x].name;
                        }
                        else
                        {
                            list[x] = (x + 1) + ". Empty";
                        }
                    }
                    arraySelect = EditorGUI.Popup(
                        new Rect(400 * rect.x, 210 * rect.y, 545 * rect.x, 25 * rect.y),
                        "Objects : ",
                        arraySelect,
                        list);
                    if (arraySelect >= myTarget.gui.selectObjs.Length)
                    {
                        arraySelect = 0;
                    }
                    else
                    {
                        myTarget.gui.selectObjs[arraySelect] = EditorGUI.ObjectField(
                            new Rect(400 * rect.x, 240 * rect.y, 545 * rect.x, 25 * rect.y),
                            "",
                            myTarget.gui.selectObjs[arraySelect],
                            typeof(GameObject),
                            false) as GameObject;
                    }
                    Health healthObj = targetUnit.obj.GetComponent<Health>();
                    if (healthObj == null)
                    {
                        if (GUI.Button(
                            new Rect(400 * rect.x, 280 * rect.y, 545 * rect.x, 420 * rect.y),
                            "Add Health Indicator"))
                        {
                            targetUnit.obj.AddComponent<Health>();
                        }
                    }
                    else
                    {
                        healthObj.backgroundBar = EditorGUI.ObjectField(
                            new Rect(400 * rect.x, 280 * rect.y, 100 * rect.x, 50 * rect.y),
                            healthObj.backgroundBar,
                            typeof(Texture2D),
                            true) as Texture2D;
                        healthObj.healthBar = EditorGUI.ObjectField(
                            new Rect(400 * rect.x, 330 * rect.y, 100 * rect.x, 50 * rect.y),
                            healthObj.healthBar,
                            typeof(Texture2D),
                            true) as Texture2D;
                        healthObj.yIncrease = EditorGUI.FloatField(
                            new Rect(400 * rect.x, 390 * rect.y, 545 * rect.x, 30 * rect.y),
                            "World-Y Increase : ",
                            healthObj.yIncrease);
                        healthObj.scale = EditorGUI.IntField(
                            new Rect(400 * rect.x, 420 * rect.y, 545 * rect.x, 30 * rect.y),
                            "UI-X Scale : ",
                            healthObj.scale);
                        healthObj.yScale = EditorGUI.IntField(
                            new Rect(400 * rect.x, 450 * rect.y, 545 * rect.x, 30 * rect.y),
                            "UI-Y Scale : ",
                            healthObj.yScale);
                        int healthLength = healthObj.element.Length;
                        healthLength = EditorGUI.IntField(
                            new Rect(400 * rect.x, 485 * rect.y, 545 * rect.x, 30 * rect.y),
                            "Health Elements : ",
                            healthLength);
                        if (healthLength != healthObj.element.Length)
                        {
                            ModifyHealthElements(healthLength, healthObj.element.Length, healthObj);
                        }
                        if (healthLength > 0)
                        {
                            string[] elementName = new string[healthLength];
                            for (int x = 0; x < elementName.Length; x++)
                            {
                                elementName[x] = "Element " + (x + 1);
                            }
                            arraySelect1 = EditorGUI.Popup(
                                new Rect(400 * rect.x, 520 * rect.y, 545 * rect.x, 25 * rect.y),
                                "Element : ",
                                arraySelect1,
                                elementName);
                            if (arraySelect1 >= healthObj.element.Length)
                            {
                                arraySelect1 = 0;
                            }
                            healthObj.element[arraySelect1].image = EditorGUI.ObjectField(
                                new Rect(400 * rect.x, 540 * rect.y, 100 * rect.x, 50 * rect.y),
                                healthObj.element[arraySelect1].image,
                                typeof(Texture2D),
                                true) as Texture2D;
                            healthObj.element[arraySelect1].loc = EditorGUI.RectField(
                                new Rect(400 * rect.x, 600 * rect.y, 545 * rect.x, 50 * rect.y),
                                healthObj.element[arraySelect1].loc);
                        }
                        Vector2 point = new Vector2(620 * rect.x, 650 * rect.y);
                        if (healthObj.backgroundBar != null)
                        {
                            GUI.DrawTexture(
                                new Rect(point.x, point.y, healthObj.scale * ((float) 1), healthObj.yScale),
                                healthObj.backgroundBar);
                        }
                        if (healthObj.healthBar != null)
                        {
                            GUI.DrawTexture(
                                new Rect(point.x, point.y, healthObj.scale * ((float) 50 / 100), healthObj.yScale),
                                healthObj.healthBar);
                        }
                        foreach (HealthElement healthElement in healthObj.element)
                        {
                            if (healthElement.image == null)
                            {
                                continue;
                            }
                            GUI.DrawTexture(
                                new Rect(
                                    point.x + healthElement.loc.x,
                                    point.y - healthElement.loc.y,
                                    healthElement.loc.width,
                                    healthElement.loc.height),
                                healthElement.image);
                        }
                    }
                    helpState = 9;
                }
                // MiniMap
                else if (subMenuState == 1)
                {
                    GUI.DrawTexture(
                        new Rect(767 * rect.x, 25 * rect.y, 367 * rect.x, 20 * rect.y),
                        selectionTexture,
                        ScaleMode.StretchToFill);
                    if (myTargetMap == null)
                    {
                        if (GUI.Button(
                            new Rect(400 * rect.x, 45 * rect.y, 1100 * rect.x, 705 * rect.y),
                            "Add MiniMap Components"))
                        {
                            targetUnit.obj.AddComponent<MiniMapSignal>();
                        }
                    }
                    else
                    {
                        myTargetMap.enabled = EditorGUI.Toggle(
                            new Rect(400 * rect.x, 70 * rect.y, 540 * rect.x, 25 * rect.y),
                            "MiniMap Signal : ",
                            myTargetMap.enabled);
                        if (!myTargetMap.enabled)
                        {
                            return;
                        }
                        myTargetMap.miniMapTag = EditorGUI.TextField(
                            new Rect(400 * rect.x, 100 * rect.y, 540 * rect.x, 25 * rect.y),
                            "Tag : ",
                            "" + myTargetMap.miniMapTag);
                        GameObject mapObj = GameObject.Find("MiniMap");
                        Color defaultColor = GUI.color;
                        if (mapObj == null)
                        {
                            return;
                        }
                        MiniMap map = GameObject.Find("MiniMap").GetComponent<MiniMap>();
                        if (map != null)
                        {
                            helpState = 10;
                            foreach (MiniMapElement mapElement in map.elements)
                            {
                                if (mapElement.tag == myTargetMap.miniMapTag)
                                {
                                    GUI.color = mapElement.tints.Length > miniMapState
                                        ? mapElement.tints[miniMapState]
                                        : mapElement.tints[0];
                                    int size = (int) (50 * rect.x);
                                    GUI.DrawTexture(
                                        new Rect(400 * rect.x, 130 * rect.y, size, size),
                                        mapElement.image);
                                    GUI.color = defaultColor;
                                    mapElement.image = EditorGUI.ObjectField(
                                        new Rect(400 * rect.x, 185 * rect.y, 100, 100),
                                        mapElement.image,
                                        typeof(Texture2D),
                                        true) as Texture2D;
                                    miniMapState = EditorGUI.IntField(
                                        new Rect(400 * rect.x, 320 * rect.y, 540 * rect.x, 25 * rect.y),
                                        "Faction : ",
                                        miniMapState);
                                }
                            }
                        }
                        else
                        {
                            helpState = 11;
                        }
                    }
                }
                // Fog Of War
                else if (subMenuState == 2)
                {
                    GUI.DrawTexture(
                        new Rect(1134 * rect.x, 25 * rect.y, 366 * rect.x, 20 * rect.y),
                        selectionTexture,
                        ScaleMode.StretchToFill);
                    if (myTargetVision == null)
                    {
                        if (GUI.Button(
                            new Rect(400 * rect.x, 45 * rect.y, 1100 * rect.x, 705 * rect.y),
                            "Add Vision Components"))
                        {
                            targetUnit.obj.AddComponent<VisionSignal>();
                            // Add More Components
                        }
                    }
                    else
                    {
                        myTargetVision.enabled = EditorGUI.Toggle(
                            new Rect(400 * rect.x, 70 * rect.y, 540 * rect.x, 25 * rect.y),
                            "Vision Signal : ",
                            myTargetVision.enabled);
                        if (myTargetVision.enabled)
                        {
                            myTargetVision.radius = EditorGUI.IntField(
                                new Rect(400 * rect.x, 100 * rect.y, 540 * rect.x, 25 * rect.y),
                                "Radius : ",
                                myTargetVision.radius);
                            myTargetVision.upwardSightHeight = EditorGUI.IntField(
                                new Rect(400 * rect.x, 130 * rect.y, 540 * rect.x, 25 * rect.y),
                                "Upward Sight Height : ",
                                myTargetVision.upwardSightHeight);
                            myTargetVision.downwardSightHeight = EditorGUI.IntField(
                                new Rect(400 * rect.x, 160 * rect.y, 540 * rect.x, 25 * rect.y),
                                "Downward Sight Height : ",
                                myTargetVision.downwardSightHeight);
                        }
                        helpState = 12;
                    }
                }
            }
            // Technology
            else if (menuState == 2)
            {
                GUI.DrawTexture(
                    new Rect(950 * rect.x, 0, 275 * rect.x, 25 * rect.y),
                    selectionTexture,
                    ScaleMode.StretchToFill);
                GUI.Box(new Rect(400 * rect.x, 25 * rect.y, 1100 * rect.x, 20 * rect.y), "");
                if (GUI.Button(new Rect(400 * rect.x, 50 * rect.y, 273 * rect.x, 20 * rect.y), "Add"))
                {
                    ModifyUnitTechEffects(myTarget.techEffect.Length + 1, myTarget.techEffect.Length, myTarget);
                }
                if (GUI.Button(new Rect(673 * rect.x, 50 * rect.y, 272 * rect.x, 20 * rect.y), "Remove"))
                {
                    ModifyUnitTechEffects(myTarget.techEffect.Length - 1, myTarget.techEffect.Length, myTarget);
                }
                if (nTarget.Tech.Length <= 0 || myTarget.techEffect.Length <= 0)
                {
                    return;
                }
                int size = nTarget.Tech.Length;
                string[] names = new string[size];
                string[] nameArray = new string[size];
                for (int x = 0; x < nameArray.Length; x++)
                {
                    nameArray[x] = (x + 1) + ". " + nTarget.Tech[x].name;
                    names[x] = nTarget.Tech[x].name;
                }
                string[] unitTechs = new string[myTarget.techEffect.Length];
                for (int x = 0; x < unitTechs.Length; x++)
                {
                    unitTechs[x] = (x + 1) + ". " + myTarget.techEffect[x].name;
                }
                arraySelect = EditorGUI.Popup(
                    new Rect(400 * rect.x, 75 * rect.y, 545 * rect.x, 20 * rect.y),
                    "Techs : ",
                    arraySelect,
                    unitTechs);
                if (arraySelect >= myTarget.techEffect.Length)
                {
                    arraySelect = 0;
                }
                else
                {
                    int curY = 100;
                    arraySelect2 = myTarget.techEffect[arraySelect].index;
                    arraySelect2 = EditorGUI.Popup(
                        new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 20 * rect.y),
                        "Tech Options : ",
                        arraySelect2,
                        nameArray);
                    curY += 25;
                    myTarget.techEffect[arraySelect].index = arraySelect2;
                    if (arraySelect2 > nameArray.Length)
                    {
                        arraySelect2 = 0;
                    }
                    myTarget.techEffect[arraySelect].name = names[arraySelect2];
                    myTarget.techEffect[arraySelect].replacementObject = EditorGUI.ObjectField(
                        new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 20 * rect.y),
                        "Replacement Object : ",
                        myTarget.techEffect[arraySelect].replacementObject,
                        typeof(GameObject),
                        false) as GameObject;
                    curY += 25;
                    string[,] functions = new string[24, 4]
                    {
                        /* 0 */ {"GetName", "SetName", "", ""},
                        /* 1 */ {"GetMaxHealth", "SetMaxHealth", "AddMaxHealth", "SubMaxHealth"},
                        /* 2 */ {"GetHealth", "SetHealth", "AddHealth", "SubHealth"},
                        /* 3 */ {"GetFaction", "SetFaction", "", ""},
                        /* 4 */ {"GetFighterUnit", "SetFighterUnit", "", ""},
                        /* 5 */ {"GetAttackRate", "SetAttackRate", "AddAttackRate", "SubAttackRate"},
                        /* 6 */ {"GetAttackRange", "SetAttackRange", "AddAttackRange", "SubAttackRange"},
                        /* 7 */ {"GetAttackDamage", "SetAttackDamage", "AddAttackDamage", "SubAttackDamage"},
                        /* 8 */ {"GetLookRange", "SetLookRange", "AddLookRange", "SubLookRange"},
                        /* 9 */ {"GetSize", "SetSize", "AddSize", "SubSize"},
                        /* 10 */ {"GetRadius", "SetRadius", "AddRadius", "SubRadius"},
                        /* 11 */ {"GetResourceUnit", "SetResourceUnit", "", ""},
                        /* 12 */ {"GetCanGather", "SetCanGather", "", ""},
                        /* 13 */ {"GetResourceAmount", "SetResourceAmount", "AddResourceAmount", "SubResourceAmount"},
                        /* 14 */ {"GetResourceRate", "SetResourceRate", "AddResourceRate", "SubResourceRate"},
                        /* 15 */ {"GetBuilderUnit", "SetBuilderUnit", "AddBuilderUnit", "SubBuilderUnit"},
                        /* 16 */ {"GetCanBuild", "SetCanBuild", "", ""},
                        /* 17 */ {"GetBuilderAmount", "SetBuilderAmount", "AddBuilderAmount", "SubBuilderAmount"},
                        /* 18 */ {"GetBuilderRate", "SetBuilderRate", "AddBuilderRate", "SubBuilderRate"},
                        /* 19 */ {"GetSpeed", "SetSpeed", "AddSpeed", "SubSpeed"},
                        /* 20 */ {"GetRotateSpeed", "SetRotateSpeed", "AddRotateSpeed", "SubRotateSpeed"},
                        /* 21 */ {"GetMiniMapTag", "SetMiniMapTag", "AddMiniMapTag", "SubMiniMapTag"},
                        /* 22 */ {"GetVisionRadius", "SetVisionRadius", "AddVisionRadius", "SubVisionRadius"},
                        /* 23 */ {"GetCarryCapacity", "SetCarryCapacity", "AddCarryCapacity", "SubCarryCapacity"}
                    };
                    string[] variableName =
                    {
                        "Name", "Max Health", "Health", "Group",
                        "Fighter Unit", "Attack Rate", "Attack Range", "Attack Damage", "Look Range",
                        "Size", "Vision Radius",
                        "Resource Unit", "Can Gather", "Gather Amount", "Gather Rate",
                        "Builder Unit", "Can Build", "Rate", "Amount",
                        "Speed", "Rotate Speed", "Mini Map Tag", "Vision Radius", "CarryCapacity"
                    };
                    for (var x = 0; x < myTarget.techEffect[arraySelect].effects.Length; x++)
                    {
                        Effects effects = myTarget.techEffect[arraySelect].effects[x];
                        effects.effectName = EditorGUI.Popup(
                            new Rect(400 * rect.x, curY * rect.y, 200 * rect.x, 20 * rect.y),
                            "",
                            effects.effectName,
                            variableName);
                        int index = effects.effectName;
                        string[] funcTypes;
                        if (index == 0 || index == 3 || index == 4 || index == 11 || index == 12 || index == 15 ||
                            index == 16 || index == 21)
                        {
                            funcTypes = new string[2];
                            funcTypes[0] = "Get";
                            funcTypes[1] = "Set";
                        }
                        else
                        {
                            funcTypes = new string[4];
                            funcTypes[0] = "Get";
                            funcTypes[1] = "Set";
                            funcTypes[2] = "Add";
                            funcTypes[3] = "Sub";
                        }
                        effects.funcType = EditorGUI.Popup(
                            new Rect(600 * rect.x, curY * rect.y, 100 * rect.x, 20 * rect.y),
                            "",
                            effects.funcType,
                            funcTypes);
                        effects.funcName = functions[index, effects.funcType];
                        if (effects.funcType == 0)
                        {
                            EditorGUI.LabelField(
                                new Rect(800 * rect.x, curY * rect.y, 145 * rect.x, 20 * rect.y),
                                "Getter");
                        }
                        else
                        {
                            ResourceManager resourceManager;
                            switch (index)
                            {
                                case 0:
                                case 21:
                                    effects.text = EditorGUI.TextField(
                                        new Rect(800 * rect.x, curY * rect.y, 145 * rect.x, 20 * rect.y),
                                        "",
                                        effects.text);
                                    break;
                                case 4:
                                case 11:
                                case 15:
                                    effects.toggle = EditorGUI.Toggle(
                                        new Rect(800 * rect.x, curY * rect.y, 145 * rect.x, 20 * rect.y),
                                        "",
                                        effects.toggle);
                                    break;
                                case 12:
                                    // Resource List
                                    // Current Resource Manager will be used for read-only purposes
                                    resourceManager = GameManager.Instance.CurrentPlayerResourceManager;
                                    if (myTarget.resource.behaviour.Length != resourceManager.resourceTypes.Length)
                                    {
                                        ModifyUnitGathering(
                                            resourceManager.resourceTypes.Length,
                                            myTarget.resource.behaviour.Length,
                                            myTarget);
                                    }
                                    else
                                    {
                                        string[] resourceNames = new string[resourceManager.resourceTypes.Length];
                                        for (int y = 0; y < resourceNames.Length; y++)
                                        {
                                            resourceNames[y] = resourceManager.resourceTypes[y].name;
                                        }
                                        if (arraySelect >= resourceNames.Length)
                                        {
                                            arraySelect = 0;
                                        }
                                        else
                                        {
                                            if (effects.index >= resourceNames.Length)
                                            {
                                                effects.index = 0;
                                            }
                                            effects.index = EditorGUI.Popup(
                                                new Rect(700 * rect.x, curY * rect.y, 100 * rect.x, 20 * rect.y),
                                                "",
                                                effects.index,
                                                resourceNames);
                                            effects.toggle = EditorGUI.Toggle(
                                                new Rect(800 * rect.x, curY * rect.y, 145 * rect.x, 20 * rect.y),
                                                "",
                                                effects.toggle);
                                        }
                                    }
                                    break;
                                case 16:
                                    // Building List
                                    if (myTarget.build.build.Length != nTarget.BuildingList.Length)
                                    {
                                        ModifyUnitBuildBehaviour(
                                            nTarget.BuildingList.Length,
                                            myTarget.build.build.Length,
                                            myTarget);
                                    }
                                    else
                                    {
                                        string[] buildNames = new string[nTarget.BuildingList.Length];
                                        for (int y = 0; y < buildNames.Length; y++)
                                        {
                                            if (nTarget.BuildingList[y].obj != null)
                                            {
                                                BuildingController cont = nTarget.BuildingList[y]
                                                    .obj.GetComponent<BuildingController>();
                                                if (cont != null)
                                                {
                                                    buildNames[y] = cont.name;
                                                }
                                            }
                                        }
                                        if (arraySelect >= buildNames.Length)
                                        {
                                            arraySelect = 0;
                                        }
                                        else
                                        {
                                            if (effects.index >= buildNames.Length)
                                            {
                                                effects.index = 0;
                                            }
                                            effects.index = EditorGUI.Popup(
                                                new Rect(700 * rect.x, curY * rect.y, 100 * rect.x, 20 * rect.y),
                                                "",
                                                effects.index,
                                                buildNames);
                                            effects.toggle = EditorGUI.Toggle(
                                                new Rect(800 * rect.x, curY * rect.y, 145 * rect.x, 20 * rect.y),
                                                "",
                                                effects.toggle);
                                        }
                                    }
                                    break;
                                case 13:
                                case 14:
                                case 17:
                                case 18:
                                    if (index == 13 || index == 14)
                                    {
                                        // Resource List
                                        // Current Resource Manager will be used for read-only purposes
                                        resourceManager = GameManager.Instance.CurrentPlayerResourceManager;
                                        if (myTarget.resource.behaviour.Length != resourceManager.resourceTypes.Length)
                                        {
                                            ModifyUnitGathering(
                                                resourceManager.resourceTypes.Length,
                                                myTarget.resource.behaviour.Length,
                                                myTarget);
                                        }
                                        else
                                        {
                                            string[] resourceNames = new string[resourceManager.resourceTypes.Length];
                                            for (int y = 0; y < resourceNames.Length; y++)
                                            {
                                                resourceNames[y] = resourceManager.resourceTypes[y].name;
                                            }
                                            if (arraySelect >= resourceNames.Length)
                                            {
                                                arraySelect = 0;
                                            }
                                            else
                                            {
                                                if (effects.index >= resourceNames.Length)
                                                {
                                                    effects.index = 0;
                                                }
                                                effects.index = EditorGUI.Popup(
                                                    new Rect(700 * rect.x, curY * rect.y, 100 * rect.x, 20 * rect.y),
                                                    "",
                                                    effects.index,
                                                    resourceNames);
                                                effects.amount = EditorGUI.FloatField(
                                                    new Rect(800 * rect.x, curY * rect.y, 145 * rect.x, 20 * rect.y),
                                                    "",
                                                    effects.amount);
                                            }
                                        }
                                    }
                                    else if (index == 17 || index == 18)
                                    {
                                        // Building List
                                        if (myTarget.build.build.Length != nTarget.BuildingList.Length)
                                        {
                                            ModifyUnitBuildBehaviour(
                                                nTarget.BuildingList.Length,
                                                myTarget.build.build.Length,
                                                myTarget);
                                        }
                                        else
                                        {
                                            string[] buildNames = new string[nTarget.BuildingList.Length];
                                            for (int y = 0; y < buildNames.Length; y++)
                                            {
                                                if (nTarget.BuildingList[y].obj != null)
                                                {
                                                    BuildingController cont =
                                                        nTarget.BuildingList[y].obj.GetComponent<BuildingController>();
                                                    if (cont != null)
                                                    {
                                                        buildNames[y] = cont.name;
                                                    }
                                                }
                                            }
                                            if (arraySelect >= buildNames.Length)
                                            {
                                                arraySelect = 0;
                                            }
                                            else
                                            {
                                                if (effects.index >= buildNames.Length)
                                                {
                                                    effects.index = 0;
                                                }
                                                effects.index = EditorGUI.Popup(
                                                    new Rect(700 * rect.x, curY * rect.y, 100 * rect.x, 20 * rect.y),
                                                    "",
                                                    effects.index,
                                                    buildNames);
                                                effects.amount = EditorGUI.FloatField(
                                                    new Rect(800 * rect.x, curY * rect.y, 145 * rect.x, 20 * rect.y),
                                                    "",
                                                    effects.amount);
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    effects.amount = EditorGUI.FloatField(
                                        new Rect(800 * rect.x, curY * rect.y, 145 * rect.x, 20 * rect.y),
                                        "",
                                        effects.amount);
                                    break;
                            }
                        }
                        curY += 25;
                    }
                    if (GUI.Button(new Rect(400 * rect.x, curY * rect.y, 273 * rect.x, 20 * rect.y), "Add"))
                    {
                        ModifyUnitTechEffectEffects(
                            myTarget.techEffect[arraySelect].effects.Length + 1,
                            myTarget.techEffect[arraySelect].effects.Length,
                            myTarget,
                            arraySelect);
                    }
                    if (GUI.Button(new Rect(673 * rect.x, curY * rect.y, 272 * rect.x, 20 * rect.y), "Remove"))
                    {
                        ModifyUnitTechEffectEffects(
                            myTarget.techEffect[arraySelect].effects.Length - 1,
                            myTarget.techEffect[arraySelect].effects.Length,
                            myTarget,
                            arraySelect);
                    }
                }
            }
            // Animation and Sounds
            else if (menuState == 3)
            {
                GUI.DrawTexture(
                    new Rect(1225 * rect.x, 0, 275 * rect.x, 25 * rect.y),
                    selectionTexture,
                    ScaleMode.StretchToFill);
                GUI.Box(new Rect(400 * rect.x, 25 * rect.y, 1100 * rect.x, 20 * rect.y), "");
                Animator comp = targetUnit.obj.GetComponent<Animator>();
                if (myTarget.anim.manager)
                {
                    myTarget.anim.manager.runtimeAnimatorController = EditorGUI.ObjectField(
                        new Rect(400 * rect.x, 70 * rect.y, 540 * rect.x, 25 * rect.y),
                        "Controller : ",
                        myTarget.anim.manager.runtimeAnimatorController,
                        typeof(RuntimeAnimatorController),
                        false) as RuntimeAnimatorController;
                }
                else if (comp)
                {
                    myTarget.anim.manager = comp;
                }
                else if (GUI.Button(new Rect(400 * rect.x, 70 * rect.y, 540 * rect.x, 25 * rect.y), "Add Animator"))
                {
                    targetUnit.obj.AddComponent<Animator>();
                }
                myTarget.anim.idleAudio = EditorGUI.ObjectField(
                    new Rect(400 * rect.x, 100 * rect.y, 540 * rect.x, 25 * rect.y),
                    "Idle Audio : ",
                    myTarget.anim.idleAudio,
                    typeof(AudioClip),
                    false) as AudioClip;
                myTarget.anim.moveAudio = EditorGUI.ObjectField(
                    new Rect(400 * rect.x, 130 * rect.y, 540 * rect.x, 25 * rect.y),
                    "Move Audio : ",
                    myTarget.anim.moveAudio,
                    typeof(AudioClip),
                    false) as AudioClip;
                myTarget.anim.gatherAudio = EditorGUI.ObjectField(
                    new Rect(400 * rect.x, 160 * rect.y, 540 * rect.x, 25 * rect.y),
                    "Gather Audio : ",
                    myTarget.anim.gatherAudio,
                    typeof(AudioClip),
                    false) as AudioClip;
                myTarget.anim.buildAudio = EditorGUI.ObjectField(
                    new Rect(400 * rect.x, 190 * rect.y, 540 * rect.x, 25 * rect.y),
                    "Build Audio : ",
                    myTarget.anim.buildAudio,
                    typeof(AudioClip),
                    false) as AudioClip;
                myTarget.anim.attackAudio = EditorGUI.ObjectField(
                    new Rect(400 * rect.x, 220 * rect.y, 540 * rect.x, 25 * rect.y),
                    "Attack Audio : ",
                    myTarget.anim.attackAudio,
                    typeof(AudioClip),
                    false) as AudioClip;
                myTarget.anim.deathObject = EditorGUI.ObjectField(
                    new Rect(400 * rect.x, 250 * rect.y, 540 * rect.x, 25 * rect.y),
                    "Death Object : ",
                    myTarget.anim.deathObject,
                    typeof(GameObject),
                    false) as GameObject;
            }
        }

        void DrawBuildingGui(Vector2 rect)
        {
            if (unitId >= nTarget.BuildingList.Length || unitId < 0)
            {
                unitId = 0;
                return;
            }
            if (nTarget.BuildingList.Length <= 0 || !nTarget.BuildingList[unitId].obj)
            {
                return;
            }
            Building targetBuilding = nTarget.BuildingList[unitId];
            // Saves any changes on a Scene Save
            EditorUtility.SetDirty(targetBuilding.obj);

            BuildingController myTarget = targetBuilding.obj.GetComponent<BuildingController>();
            MiniMapSignal myTargetMap = targetBuilding.obj.GetComponent<MiniMapSignal>();
            VisionSignal myTargetVision = targetBuilding.obj.GetComponent<VisionSignal>();
            if (!myTarget)
            {
                if (GUI.Button(new Rect(400 * rect.x, 0, 1100 * rect.x, 750 * rect.y), "+"))
                {
                    targetBuilding.obj.AddComponent<BuildingController>();
                }
                return;
            }
            if (GUI.Button(new Rect(400 * rect.x, 0, 275 * rect.x, 25 * rect.y), "Stats"))
            {
                menuState = 0;
                subMenuState = 0;
            }
            if (GUI.Button(new Rect(675 * rect.x, 0, 275 * rect.x, 25 * rect.y), "GUI"))
            {
                menuState = 1;
                subMenuState = 0;
            }
            if (GUI.Button(new Rect(950 * rect.x, 0, 275 * rect.x, 25 * rect.y), "Techs"))
            {
                menuState = 2;
                subMenuState = 0;
            }
            if (GUI.Button(new Rect(1225 * rect.x, 0, 275 * rect.x, 25 * rect.y), "Anim/Sounds"))
            {
                menuState = 3;
                subMenuState = 0;
            }
            // Stats
            if (menuState == 0)
            {
                if (targetBuilding.obj != lastObj)
                {
                    objEditor = Editor.CreateEditor(targetBuilding.obj);
                    lastObj = targetBuilding.obj;
                }
                if (menuState == 0 && subMenuState != 3)
                {
                    objEditor.OnInteractivePreviewGUI(
                        new Rect(945 * rect.x, 50 * rect.y, 555 * rect.x, 650 * rect.y),
                        EditorStyles.toolbarButton);
                }
                GUI.DrawTexture(
                    new Rect(400 * rect.x, 0, 275 * rect.x, 25 * rect.y),
                    selectionTexture,
                    ScaleMode.StretchToFill);
                if (GUI.Button(new Rect(400 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y), "Global"))
                {
                    subMenuState = 0;
                    arraySelect = 0;
                }
                if (GUI.Button(new Rect(620 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y), "Production"))
                {
                    subMenuState = 2;
                    arraySelect = 0;
                }
                if (GUI.Button(new Rect(840 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y), "Size"))
                {
                    subMenuState = 3;
                    arraySelect = 0;
                }
                if (GUI.Button(new Rect(1060 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y), "Generate"))
                {
                    subMenuState = 4;
                    arraySelect = 0;
                }
                if (GUI.Button(new Rect(1280 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y), "Drop Off"))
                {
                    subMenuState = 5;
                    arraySelect = 0;
                }
                // Global
                if (subMenuState == 0)
                {
                    GUI.DrawTexture(
                        new Rect(400 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y),
                        selectionTexture,
                        ScaleMode.StretchToFill);
                    targetBuilding.autoBuild = EditorGUI.Toggle(
                        new Rect(400 * rect.x, 70 * rect.y, 540 * rect.x, 25 * rect.y),
                        "Auto Build : ",
                        targetBuilding.autoBuild);
                    targetBuilding.tempObj = EditorGUI.ObjectField(
                        new Rect(400 * rect.x, 100 * rect.y, 540 * rect.x, 25 * rect.y),
                        "Temporary : ",
                        targetBuilding.tempObj,
                        typeof(GameObject),
                        true) as GameObject;
                    int curY = 130;
                    if (!targetBuilding.autoBuild)
                    {
                        targetBuilding.progressObj = EditorGUI.ObjectField(
                            new Rect(400 * rect.x, curY * rect.y, 540 * rect.x, 25 * rect.y),
                            "Progress : ",
                            targetBuilding.progressObj,
                            typeof(GameObject),
                            true) as GameObject;
                        curY += 30;
                    }
                    targetBuilding.obj = EditorGUI.ObjectField(
                        new Rect(400 * rect.x, curY * rect.y, 540 * rect.x, 25 * rect.y),
                        "Final : ",
                        targetBuilding.obj,
                        typeof(GameObject),
                        true) as GameObject;
                    curY += 30;
                    myTarget.name = EditorGUI.TextField(
                        new Rect(400 * rect.x, curY * rect.y, 540 * rect.x, 25 * rect.y),
                        "Name : ",
                        myTarget.name);
                    curY += 30;
                    myTarget.maxHealth = EditorGUI.IntField(
                        new Rect(400 * rect.x, curY * rect.y, 540 * rect.x, 25 * rect.y),
                        "Max Health : ",
                        myTarget.maxHealth);
                    curY += 30;
                    myTarget.health = EditorGUI.IntField(
                        new Rect(400 * rect.x, curY * rect.y, 540 * rect.x, 25 * rect.y),
                        "Health : ",
                        myTarget.health);
                    curY += 30;
                    if (target.UnitTypes.Length > 0)
                    {
                        string[] typeNames = new string[target.UnitTypes.Length];
                        for (int x = 0; x < target.UnitTypes.Length; x++)
                        {
                            typeNames[x] = (x + 1) + ". " + target.UnitTypes[x].name;
                            if (target.UnitTypes[x] == myTarget.type)
                            {
                                arraySelect = x;
                            }
                        }
                        arraySelect = EditorGUI.Popup(
                            new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 25 * rect.y),
                            "Type : ",
                            arraySelect,
                            typeNames);
                        myTarget.type = target.UnitTypes[arraySelect];
                        curY += 30;
                    }
                    if (targetBuilding.progressObj)
                    {
                        BuildingController progressObj = targetBuilding
                            .progressObj.GetComponent<BuildingController>();
                        BuildingController objScript = targetBuilding
                            .obj.GetComponent<BuildingController>();
                        if (progressObj != null)
                        {
                            progressObj.progressReq = EditorGUI.FloatField(
                                new Rect(400 * rect.x, curY * rect.y, 540 * rect.x, 25 * rect.y),
                                "Required Build Progress : ",
                                progressObj.progressReq);
                            curY += 30;
                            progressObj.progressCur = EditorGUI.FloatField(
                                new Rect(400 * rect.x, curY * rect.y, 540 * rect.x, 25 * rect.y),
                                "Starting Progress : ",
                                progressObj.progressCur);
                            curY += 30;
                            progressObj.progressRate = EditorGUI.FloatField(
                                new Rect(400 * rect.x, curY * rect.y, 540 * rect.x, 25 * rect.y),
                                "Progress Rate : ",
                                progressObj.progressRate);
                            curY += 30;
                            progressObj.progressPerRate = EditorGUI.FloatField(
                                new Rect(400 * rect.x, curY * rect.y, 540 * rect.x, 25 * rect.y),
                                "Progress Per Rate : ",
                                progressObj.progressPerRate);
                            curY += 30;
                            progressObj.nextBuild = targetBuilding.obj;
                            progressObj.buildingType = BuildingType.ProgressBuilding;
                            objScript.buildingType = BuildingType.CompleteBuilding;
                        }
                        else
                        {
                            progressObj = targetBuilding.progressObj.AddComponent<BuildingController>();
                            progressObj.buildingType = BuildingType.ProgressBuilding;
                        }
                    }
                    // Current Resource Manager will be used for read-only purposes
                    ResourceManager mg = GameManager.Instance.CurrentPlayerResourceManager;
                    if (targetBuilding.cost.Length != mg.resourceTypes.Length)
                    {
                        targetBuilding.cost = new int[mg.resourceTypes.Length];
                    }
                    string[] costs = new string[mg.resourceTypes.Length];
                    for (int x = 0; x < costs.Length; x++)
                    {
                        costs[x] = mg.resourceTypes[x].name;
                    }
                    arraySelect3 = EditorGUI.Popup(
                        new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 25 * rect.y),
                        "Resource Type : ",
                        arraySelect3,
                        costs);
                    curY += 25;
                    if (arraySelect3 >= costs.Length)
                    {
                        arraySelect3 = 0;
                    }
                    else
                    {
                        targetBuilding.cost[arraySelect3] = EditorGUI.IntField(
                            new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 25 * rect.y),
                            "Cost : ",
                            targetBuilding.cost[arraySelect3]);
                        curY += 30;
                    }
                    GUI.skin.textArea.wordWrap = true;
                    targetBuilding.description = EditorGUI.TextArea(
                        new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 100 * rect.y),
                        targetBuilding.description);
                }
                // Production
                else if (subMenuState == 2)
                {
                    GUI.DrawTexture(
                        new Rect(620 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y),
                        selectionTexture,
                        ScaleMode.StretchToFill);
                    string[] options = {"Units", "Techs"};
                    arraySelect = EditorGUI.Popup(
                        new Rect(400 * rect.x, 70 * rect.y, 540 * rect.x, 25 * rect.y),
                        "Production Type : ",
                        arraySelect,
                        options);
                    if (arraySelect == 0)
                    {
                        myTarget.unitProduction.canProduce = EditorGUI.Toggle(
                            new Rect(400 * rect.x, 100 * rect.y, 540 * rect.x, 25 * rect.y),
                            "Can Produce : ",
                            myTarget.unitProduction.canProduce);
                        if (!myTarget.unitProduction.canProduce)
                        {
                            return;
                        }
                        int curY = 120;
                        if (myTarget.unitProduction.units.Length > 0)
                        {
                            myTarget.unitProduction.canBuildAtOnce = EditorGUI.IntField(
                                new Rect(400 * rect.x, curY * rect.y, 540 * rect.x, 25 * rect.y),
                                "Can Produce At Once : ",
                                myTarget.unitProduction.canBuildAtOnce);
                            curY += 30;
                            myTarget.unitProduction.maxAmount = EditorGUI.IntField(
                                new Rect(400 * rect.x, curY * rect.y, 540 * rect.x, 25 * rect.y),
                                "Max Amount : ",
                                myTarget.unitProduction.maxAmount);
                            curY += 30;
                            myTarget.unitProduction.buildLoc = EditorGUI.ObjectField(
                                new Rect(400 * rect.x, curY * rect.y, 540 * rect.x, 25 * rect.y),
                                "Build Loc : ",
                                myTarget.unitProduction.buildLoc,
                                typeof(GameObject),
                                false) as GameObject;
                            curY += 30;
                            string[] produceNames = new string[myTarget.unitProduction.units.Length];
                            string[] unitNames = new string[nTarget.UnitList.Length];
                            for (int x = 0; x < nTarget.UnitList.Length; x++)
                            {
                                if (nTarget.UnitList[x].obj)
                                {
                                    UnitController cont = nTarget.UnitList[x].obj.GetComponent<UnitController>();
                                    if (cont != null)
                                    {
                                        unitNames[x] = (x + 1) + ". " + cont.name;
                                    }
                                }
                            }
                            for (int x = 0; x < myTarget.unitProduction.units.Length; x++)
                            {
                                if (myTarget.unitProduction.units[x] != null)
                                {
                                    produceNames[x] = (x + 1) + ". " + myTarget.unitProduction.units[x].customName;
                                }
                            }
                            if (GUI.Button(
                                new Rect(400 * rect.x, curY * rect.y, 270 * rect.x, 15 * rect.y),
                                "Add Unit"))
                            {
                                ModifyFactionUnitProduction(
                                    myTarget.unitProduction.units.Length + 1,
                                    myTarget.unitProduction.units.Length,
                                    arraySelect);
                            }
                            if (GUI.Button(
                                new Rect(670 * rect.x, curY * rect.y, 270 * rect.x, 15 * rect.y),
                                "Remove Unit"))
                            {
                                ModifyFactionUnitProduction(
                                    myTarget.unitProduction.units.Length - 1,
                                    myTarget.unitProduction.units.Length,
                                    arraySelect);
                            }
                            curY += 20;
                            arraySelect2 = EditorGUI.Popup(
                                new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 25 * rect.y),
                                "Production : ",
                                arraySelect2,
                                produceNames);
                            curY += 20;
                            if (arraySelect2 >= myTarget.unitProduction.units.Length || arraySelect2 < 0)
                            {
                                arraySelect2 = myTarget.unitProduction.units.Length - 1;
                            }
                            else
                            {
                                myTarget.unitProduction.units[arraySelect2].customName = EditorGUI.TextField(
                                    new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 25 * rect.y),
                                    "Custom Name : ",
                                    myTarget.unitProduction.units[arraySelect2].customName);
                                curY += 30;
                                EditorGUI.LabelField(
                                    new Rect(400 * rect.x, curY * rect.y, 150 * rect.x, 100 * rect.y),
                                    "Custom Texture : ");
                                myTarget.unitProduction.units[arraySelect2].customTexture = EditorGUI.ObjectField(
                                    new Rect(600 * rect.x, curY * rect.y, 150 * rect.x, 100 * rect.y),
                                    myTarget.unitProduction.units[arraySelect2].customTexture,
                                    typeof(Texture2D),
                                    true) as Texture2D;
                                curY += 110;
                                myTarget.unitProduction.units[arraySelect2].UnitIndex = EditorGUI.Popup(
                                    new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 25 * rect.y),
                                    "Unit To Produce : ",
                                    myTarget.unitProduction.units[arraySelect2].UnitIndex,
                                    unitNames);
                                curY += 20;
                                myTarget.unitProduction.units[arraySelect2].canProduce = EditorGUI.Toggle(
                                    new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 25 * rect.y),
                                    "Can Produce : ",
                                    myTarget.unitProduction.units[arraySelect2].canProduce);
                                curY += 20;
                                myTarget.unitProduction.units[arraySelect2].dur = EditorGUI.FloatField(
                                    new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 25 * rect.y),
                                    "Duration : ",
                                    myTarget.unitProduction.units[arraySelect2].dur);
                                curY += 25;
                                myTarget.unitProduction.units[arraySelect2].amount = EditorGUI.FloatField(
                                    new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 25 * rect.y),
                                    "Amount : ",
                                    myTarget.unitProduction.units[arraySelect2].amount);
                                curY += 25;
                                myTarget.unitProduction.units[arraySelect2].rate = EditorGUI.FloatField(
                                    new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 25 * rect.y),
                                    "Rate : ",
                                    myTarget.unitProduction.units[arraySelect2].rate);
                                curY += 30;
                                // Current Resource Manager will be used for read-only purposes
                                ResourceManager mg = GameManager.Instance.CurrentPlayerResourceManager;
                                if (myTarget.unitProduction.units[arraySelect2].cost.Length != mg.resourceTypes.Length)
                                {
                                    myTarget.unitProduction.units[arraySelect2].cost = new int[mg.resourceTypes.Length];
                                }
                                string[] costs = new string[mg.resourceTypes.Length];
                                for (int x = 0; x < costs.Length; x++)
                                {
                                    costs[x] = mg.resourceTypes[x].name;
                                }
                                arraySelect3 = EditorGUI.Popup(
                                    new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 25 * rect.y),
                                    "Resource Type : ",
                                    arraySelect3,
                                    costs);
                                curY += 25;
                                if (arraySelect3 >= costs.Length)
                                {
                                    arraySelect3 = 0;
                                }
                                else
                                {
                                    myTarget.unitProduction.units[arraySelect2].cost[arraySelect3] = EditorGUI.IntField(
                                        new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 25 * rect.y),
                                        "Cost to Produce : ",
                                        myTarget.unitProduction.units[arraySelect2].cost[arraySelect3]);
                                    curY += 30;
                                }
                                GUI.skin.textArea.wordWrap = true;
                                myTarget.unitProduction.units[arraySelect2].description = EditorGUI.TextArea(
                                    new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 100 * rect.y),
                                    myTarget.unitProduction.units[arraySelect2].description);
                            }
                        }
                        else
                        {
                            if (GUI.Button(
                                new Rect(400 * rect.x, curY * rect.y, 270 * rect.x, 15 * rect.y),
                                "Add Unit"))
                            {
                                ModifyFactionUnitProduction(
                                    myTarget.unitProduction.units.Length + 1,
                                    myTarget.unitProduction.units.Length,
                                    arraySelect);
                            }
                            if (GUI.Button(
                                new Rect(670 * rect.x, curY * rect.y, 270 * rect.x, 15 * rect.y),
                                "Remove Unit"))
                            {
                                ModifyFactionUnitProduction(
                                    myTarget.unitProduction.units.Length - 1,
                                    myTarget.unitProduction.units.Length,
                                    arraySelect);
                            }
                        }
                    }
                    else if (arraySelect == 1)
                    {
                        myTarget.techProduction.canProduce = EditorGUI.Toggle(
                            new Rect(400 * rect.x, 100 * rect.y, 540 * rect.x, 25 * rect.y),
                            "Can Produce : ",
                            myTarget.techProduction.canProduce);
                        if (!myTarget.techProduction.canProduce)
                        {
                            return;
                        }
                        int curY = 120;
                        if (myTarget.techProduction.techs.Length > 0)
                        {
                            myTarget.techProduction.canBuildAtOnce = EditorGUI.IntField(
                                new Rect(400 * rect.x, curY * rect.y, 540 * rect.x, 25 * rect.y),
                                "Can Produce At Once : ",
                                myTarget.techProduction.canBuildAtOnce);
                            curY += 30;
                            myTarget.techProduction.maxAmount = EditorGUI.IntField(
                                new Rect(400 * rect.x, curY * rect.y, 540 * rect.x, 25 * rect.y),
                                "Max Amount : ",
                                myTarget.techProduction.maxAmount);
                            curY += 30;
                            string[] produceNames = new string[myTarget.techProduction.techs.Length];
                            for (int x = 0; x < myTarget.techProduction.techs.Length; x++)
                            {
                                if (myTarget.techProduction.techs[x] == null)
                                {
                                    myTarget.techProduction.techs[x] = new ProduceTech();
                                }
                                if (myTarget.techProduction.techs[x] != null)
                                {
                                    produceNames[x] =
                                        (x + 1) + ". " + myTarget.techProduction.techs[x].customName;
                                }
                            }
                            int size = nTarget.Tech.Length;
                            string[] names = new string[size];
                            string[] nameArray = new string[size];
                            for (int x = 0; x < nameArray.Length; x++)
                            {
                                nameArray[x] = (x + 1) + ". " + nTarget.Tech[x].name;
                                names[x] = nTarget.Tech[x].name;
                            }
                            string[] unitTechs = new string[myTarget.techEffect.Length];
                            for (int x = 0; x < unitTechs.Length; x++)
                            {
                                unitTechs[x] = (x + 1) + ". " + myTarget.techEffect[x].name;
                            }
                            if (GUI.Button(
                                new Rect(400 * rect.x, curY * rect.y, 270 * rect.x, 15 * rect.y),
                                "Add Tech"))
                            {
                                ModifyFactionTechnologyProduction(
                                    myTarget.techProduction.techs.Length + 1,
                                    myTarget.techProduction.techs.Length,
                                    arraySelect);
                            }
                            if (GUI.Button(
                                new Rect(670 * rect.x, curY * rect.y, 270 * rect.x, 15 * rect.y),
                                "Remove Tech"))
                            {
                                ModifyFactionTechnologyProduction(
                                    myTarget.techProduction.techs.Length - 1,
                                    myTarget.techProduction.techs.Length,
                                    arraySelect);
                            }
                            curY += 20;
                            arraySelect2 = EditorGUI.Popup(
                                new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 25 * rect.y),
                                "Production : ",
                                arraySelect2,
                                produceNames);
                            curY += 20;
                            if (arraySelect2 >= myTarget.techProduction.techs.Length &&
                                myTarget.techProduction.techs.Length > 0)
                            {
                                arraySelect2 = myTarget.techProduction.techs.Length - 1;
                            }
                            else
                            {
                                var techSelection = myTarget.techProduction.techs[arraySelect2] ?? new ProduceTech();
                                if (techSelection.customName == null)
                                {
                                    techSelection.customName = "";
                                }

                                if (arraySelect2 > myTarget.techProduction.techs.Length)
                                {
                                    arraySelect2 = 0;
                                }
                                techSelection.customName = EditorGUI.TextField(
                                    new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 25 * rect.y),
                                    "Custom Name : ",
                                    techSelection.customName);
                                curY += 30;
                                EditorGUI.LabelField(
                                    new Rect(400 * rect.x, curY * rect.y, 150 * rect.x, 100 * rect.y),
                                    "Custom Texture : ");
                                techSelection.customTexture = EditorGUI.ObjectField(
                                    new Rect(600 * rect.x, curY * rect.y, 150 * rect.x, 100 * rect.y),
                                    techSelection.customTexture,
                                    typeof(Texture2D),
                                    true) as Texture2D;
                                curY += 110;

                                techSelection.index = EditorGUI.Popup(
                                    new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 25 * rect.y),
                                    "Tech To Produce : ",
                                    techSelection.index,
                                    nameArray);
                                if (techSelection.index >= size || techSelection.index < 0)
                                {
                                    techSelection.index = 0;
                                }
                                else
                                {
                                    techSelection.techName = names[techSelection.index];
                                }
                                curY += 20;
                                techSelection.canProduce = EditorGUI.Toggle(
                                    new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 25 * rect.y),
                                    "Can Produce : ",
                                    techSelection.canProduce);
                                curY += 20;
                                techSelection.dur = EditorGUI.FloatField(
                                    new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 25 * rect.y),
                                    "Duration : ",
                                    techSelection.dur);
                                curY += 25;
                                techSelection.rate = EditorGUI.FloatField(
                                    new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 25 * rect.y),
                                    "Rate : ",
                                    techSelection.rate);
                                curY += 30;
                                techSelection.amount = EditorGUI.FloatField(
                                    new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 25 * rect.y),
                                    "Amount : ",
                                    techSelection.amount);
                                curY += 30;
                                // Current Resource Manager will be used for read-only purposes
                                ResourceManager mg = GameManager.Instance.CurrentPlayerResourceManager;
                                if (techSelection.cost.Length != mg.resourceTypes.Length)
                                {
                                    techSelection.cost = new int[mg.resourceTypes.Length];
                                }
                                string[] costs = new string[mg.resourceTypes.Length];
                                for (int x = 0; x < costs.Length; x++)
                                {
                                    costs[x] = mg.resourceTypes[x].name;
                                }
                                arraySelect3 = EditorGUI.Popup(
                                    new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 25 * rect.y),
                                    "Resource Type : ",
                                    arraySelect3,
                                    costs);
                                curY += 25;
                                if (arraySelect3 >= costs.Length)
                                {
                                    arraySelect3 = 0;
                                }
                                else
                                {
                                    techSelection.cost[arraySelect3] = EditorGUI.IntField(
                                        new Rect(400 * rect.x, curY * rect.y, 545 * rect.x, 25 * rect.y),
                                        "Cost to Produce : ",
                                        techSelection.cost[arraySelect3]);
                                    //curY += 20;
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(
                                new Rect(400 * rect.x, curY * rect.y, 270 * rect.x, 15 * rect.y),
                                "Add Tech"))
                            {
                                ModifyFactionTechnologyProduction(
                                    myTarget.techProduction.techs.Length + 1,
                                    myTarget.techProduction.techs.Length,
                                    arraySelect);
                            }
                            if (GUI.Button(
                                new Rect(670 * rect.x, curY * rect.y, 270 * rect.x, 15 * rect.y),
                                "Remove Tech"))
                            {
                                ModifyFactionTechnologyProduction(
                                    myTarget.techProduction.techs.Length - 1,
                                    myTarget.techProduction.techs.Length,
                                    arraySelect);
                            }
                        }
                    }
                }
                // Size
                else if (subMenuState == 3)
                {
                    helpState = 13;
                    GUI.DrawTexture(
                        new Rect(840 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y),
                        selectionTexture,
                        ScaleMode.StretchToFill);
                    targetBuilding.closeWidth = EditorGUI.IntField(
                        new Rect(400 * rect.x, 70 * rect.y, 1100 * rect.x, 25 * rect.y),
                        "Close Width : ",
                        targetBuilding.closeWidth);
                    targetBuilding.closeLength = EditorGUI.IntField(
                        new Rect(400 * rect.x, 100 * rect.y, 1100 * rect.x, 25 * rect.y),
                        "Close Length : ",
                        targetBuilding.closeLength);
                    int arraySize = (targetBuilding.closeWidth * 2 + 1) *
                                    (targetBuilding.closeLength * 2 + 1);
                    if (targetBuilding.closePoints.Length != arraySize)
                    {
                        targetBuilding.closePoints = new int[arraySize];
                        for (int x = 0; x < arraySize; x++)
                        {
                            targetBuilding.closePoints[x] = 2;
                        }
                    }
                    factionPosition2 = GUI.BeginScrollView(
                        new Rect(400 * rect.x, 130 * rect.y, 1100 * rect.x, 570 * rect.y),
                        factionPosition2,
                        new Rect(
                            0,
                            0,
                            30 * (targetBuilding.closeWidth * 2 + 1) + 60,
                            30 * (targetBuilding.closeLength * 2 + 1) + 60));
                    for (int x = 0; x < targetBuilding.closeWidth * 2 + 1; x++)
                    {
                        GUI.Label(new Rect(30 + 30 * x, 0, 30, 30), "" + x);
                    }
                    for (int y = 0; y < targetBuilding.closeLength * 2 + 1; y++)
                    {
                        GUI.Label(new Rect(0, 30 + 30 * y, 30, 30), "" + y);
                    }
                    for (int x = 0; x < targetBuilding.closeWidth * 2 + 1; x++)
                    {
                        for (int y = 0; y < targetBuilding.closeLength * 2 + 1; y++)
                        {
                            int i = x * (targetBuilding.closeLength * 2 + 1) + y;
                            if (targetBuilding.closePoints[i] == 0)
                            {
                                if (GUI.Button(new Rect(30 + 30 * x, 30 + 30 * y, 30, 30), "O"))
                                {
                                    targetBuilding.closePoints[i] = 1;
                                }
                            }
                            else if (targetBuilding.closePoints[i] == 1)
                            {
                                if (GUI.Button(new Rect(30 + 30 * x, 30 + 30 * y, 30, 30), "W"))
                                {
                                    targetBuilding.closePoints[i] = 2;
                                }
                            }
                            else if (targetBuilding.closePoints[i] == 2)
                            {
                                if (GUI.Button(new Rect(30 + 30 * x, 30 + 30 * y, 30, 30), "X"))
                                {
                                    targetBuilding.closePoints[i] = 0;
                                }
                            }
                        }
                    }
                    for (int x = 0; x < targetBuilding.closeWidth * 2 + 1; x++)
                    {
                        GUI.Label(
                            new Rect(30 + 30 * x, 30 + 30 * (targetBuilding.closeLength * 2 + 1), 30, 30),
                            "" + x);
                    }
                    for (int y = 0; y < targetBuilding.closeLength * 2 + 1; y++)
                    {
                        GUI.Label(
                            new Rect(30 + 30 * (targetBuilding.closeWidth * 2 + 1), 30 + 30 * y, 30, 30),
                            "" + y);
                    }
                    GUI.EndScrollView();
                }
                // Generate
                else if (subMenuState == 4)
                {
                    GUI.DrawTexture(
                        new Rect(1060 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y),
                        selectionTexture,
                        ScaleMode.StretchToFill);
                    // Put Button Here for Resource Generate
                    ResourceGenerate gen = targetBuilding.obj.GetComponent<ResourceGenerate>();
                    if (gen != null)
                    {
                        // Current Resource Manager will be used for read-only purposes
                        ResourceManager rm = GameManager.Instance.CurrentPlayerResourceManager;
                        if (rm != null)
                        {
                            if (gen.resource.Length != rm.resourceTypes.Length)
                            {
                                gen.resource = new ResourceG[rm.resourceTypes.Length];
                            }
                            else
                            {
                                string[] types = new string[rm.resourceTypes.Length];
                                for (int x = 0; x < types.Length; x++)
                                {
                                    types[x] = rm.resourceTypes[x].name;
                                }
                                arraySelect = EditorGUI.Popup(
                                    new Rect(400 * rect.x, 70 * rect.y, 540 * rect.x, 25 * rect.y),
                                    "Resource Type : ",
                                    arraySelect,
                                    types);
                                if (arraySelect >= types.Length)
                                {
                                    arraySelect = 0;
                                }
                                else
                                {
                                    gen.resource[arraySelect].amount = EditorGUI.IntField(
                                        new Rect(400 * rect.x, 100 * rect.y, 540 * rect.x, 25 * rect.y),
                                        "Amount : ",
                                        gen.resource[arraySelect].amount);
                                    gen.resource[arraySelect].rate = EditorGUI.FloatField(
                                        new Rect(400 * rect.x, 130 * rect.y, 540 * rect.x, 25 * rect.y),
                                        "Rate : ",
                                        gen.resource[arraySelect].rate);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (GUI.Button(
                            new Rect(400 * rect.x, 50 * rect.y, 540 * rect.x, 650 * rect.y),
                            "Add Resource Generator"))
                        {
                            targetBuilding.obj.AddComponent<ResourceGenerate>();
                        }
                    }
                }
                else if (subMenuState == 5)
                {
                    GUI.DrawTexture(
                        new Rect(1280 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y),
                        selectionTexture,
                        ScaleMode.StretchToFill);
                    ResourceDropOff dropOff = targetBuilding.obj.GetComponent<ResourceDropOff>();
                    if (dropOff != null)
                    {
                        // Current Resource Manager will be used for read-only purposes
                        ResourceManager rm = GameManager.Instance.CurrentPlayerResourceManager;
                        if (rm != null)
                        {
                            if (dropOff.type.Length != rm.resourceTypes.Length)
                            {
                                dropOff.type = new bool[rm.resourceTypes.Length];
                            }
                            else
                            {
                                string[] types = new string[rm.resourceTypes.Length];
                                for (int x = 0; x < types.Length; x++)
                                {
                                    types[x] = rm.resourceTypes[x].name;
                                }
                                arraySelect = EditorGUI.Popup(
                                    new Rect(400 * rect.x, 70 * rect.y, 540 * rect.x, 25 * rect.y),
                                    "Resource Type : ",
                                    arraySelect,
                                    types);
                                if (arraySelect >= types.Length)
                                {
                                    arraySelect = 0;
                                }
                                else
                                {
                                    dropOff.type[arraySelect] = EditorGUI.Toggle(
                                        new Rect(400 * rect.x, 100 * rect.y, 540 * rect.x, 25 * rect.y),
                                        "Drop Off : ",
                                        dropOff.type[arraySelect]);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (GUI.Button(
                            new Rect(400 * rect.x, 50 * rect.y, 540 * rect.x, 650 * rect.y),
                            "Add Drop Off"))
                        {
                            targetBuilding.obj.AddComponent<ResourceDropOff>();
                        }
                    }
                }
            }
            // GUI
            else if (menuState == 1)
            {
                if (targetBuilding.obj != lastObj)
                {
                    objEditor = Editor.CreateEditor(targetBuilding.obj);
                    lastObj = targetBuilding.obj;
                }
                objEditor.OnInteractivePreviewGUI(
                    new Rect(945 * rect.x, 50 * rect.y, 555 * rect.x, 650 * rect.y),
                    EditorStyles.toolbarButton);
                GUI.DrawTexture(
                    new Rect(675 * rect.x, 0, 275 * rect.x, 25 * rect.y),
                    selectionTexture,
                    ScaleMode.StretchToFill);
                if (GUI.Button(new Rect(400 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y), "Selected"))
                {
                    subMenuState = 0;
                    arraySelect = 0;
                }
                if (GUI.Button(new Rect(620 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y), "Production GUI"))
                {
                    subMenuState = 1;
                    arraySelect = 0;
                }
                if (GUI.Button(new Rect(840 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y), "MiniMap"))
                {
                    subMenuState = 2;
                    arraySelect = 0;
                }
                if (GUI.Button(new Rect(1060 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y), "Vision"))
                {
                    subMenuState = 3;
                    arraySelect = 0;
                }
                if (GUI.Button(new Rect(1280 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y), "Progress"))
                {
                    subMenuState = 4;
                    arraySelect = 0;
                }
                // Selected
                if (subMenuState == 0)
                {
                    GUI.DrawTexture(
                        new Rect(400 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y),
                        selectionTexture,
                        ScaleMode.StretchToFill);
                    myTarget.gui.SetType("Unit");
                    EditorGUI.LabelField(
                        new Rect(400 * rect.x, 50 * rect.y, 545 * rect.x, 100 * rect.y),
                        "GUI Image : ");
                    myTarget.gui.image =
                        EditorGUI.ObjectField(
                            new Rect(400 * rect.x, 80 * rect.y, 100 * rect.x, 100 * rect.y),
                            "",
                            myTarget.gui.image,
                            typeof(Texture2D),
                            true) as Texture2D;
                    //Modify Selected Objects
                    if (GUI.Button(new Rect(400 * rect.x, 180 * rect.y, 273 * rect.x, 25 * rect.y), "Add"))
                    {
                        ModifyBuildingSelectedObjects(
                            myTarget.gui.selectObjs.Length + 1,
                            myTarget.gui.selectObjs.Length,
                            myTarget);
                    }
                    if (GUI.Button(new Rect(673 * rect.x, 180 * rect.y, 272 * rect.x, 25 * rect.y), "Remove"))
                    {
                        ModifyBuildingSelectedObjects(
                            myTarget.gui.selectObjs.Length - 1,
                            myTarget.gui.selectObjs.Length,
                            myTarget);
                    }
                    string[] list = new string[myTarget.gui.selectObjs.Length];
                    for (int x = 0; x < list.Length; x++)
                    {
                        if (myTarget.gui.selectObjs[x])
                        {
                            list[x] = (x + 1) + ". " + myTarget.gui.selectObjs[x].name;
                        }
                        else
                        {
                            list[x] = (x + 1) + ". Empty";
                        }
                    }
                    arraySelect = EditorGUI.Popup(
                        new Rect(400 * rect.x, 210 * rect.y, 545 * rect.x, 25 * rect.y),
                        "Objects : ",
                        arraySelect,
                        list);
                    if (arraySelect >= myTarget.gui.selectObjs.Length)
                    {
                        arraySelect = 0;
                    }
                    else
                    {
                        myTarget.gui.selectObjs[arraySelect] = EditorGUI.ObjectField(
                            new Rect(400 * rect.x, 240 * rect.y, 545 * rect.x, 25 * rect.y),
                            "",
                            myTarget.gui.selectObjs[arraySelect],
                            typeof(GameObject),
                            false) as GameObject;
                    }
                    Health healthObj = targetBuilding.obj.GetComponent<Health>();
                    if (healthObj == null)
                    {
                        if (GUI.Button(
                            new Rect(400 * rect.x, 280 * rect.y, 545 * rect.x, 420 * rect.y),
                            "Add Health Indicator"))
                        {
                            targetBuilding.obj.AddComponent<Health>();
                        }
                    }
                    else
                    {
                        healthObj.backgroundBar = EditorGUI.ObjectField(
                            new Rect(400 * rect.x, 280 * rect.y, 100 * rect.x, 50 * rect.y),
                            healthObj.backgroundBar,
                            typeof(Texture2D),
                            true) as Texture2D;
                        healthObj.healthBar = EditorGUI.ObjectField(
                            new Rect(400 * rect.x, 330 * rect.y, 100 * rect.x, 50 * rect.y),
                            healthObj.healthBar,
                            typeof(Texture2D),
                            true) as Texture2D;
                        healthObj.yIncrease = EditorGUI.FloatField(
                            new Rect(400 * rect.x, 390 * rect.y, 545 * rect.x, 30 * rect.y),
                            "World-Y Increase : ",
                            healthObj.yIncrease);
                        healthObj.scale = EditorGUI.IntField(
                            new Rect(400 * rect.x, 420 * rect.y, 545 * rect.x, 30 * rect.y),
                            "UI-X Scale : ",
                            healthObj.scale);
                        healthObj.yScale = EditorGUI.IntField(
                            new Rect(400 * rect.x, 450 * rect.y, 545 * rect.x, 30 * rect.y),
                            "UI-Y Scale : ",
                            healthObj.yScale);
                        int healthLength = healthObj.element.Length;
                        healthLength = EditorGUI.IntField(
                            new Rect(400 * rect.x, 485 * rect.y, 545 * rect.x, 30 * rect.y),
                            "Health Elements : ",
                            healthLength);
                        if (healthLength != healthObj.element.Length)
                        {
                            ModifyHealthElements(healthLength, healthObj.element.Length, healthObj);
                        }
                        if (healthLength > 0)
                        {
                            string[] elementName = new string[healthLength];
                            for (int x = 0; x < elementName.Length; x++)
                            {
                                elementName[x] = "Element " + (x + 1);
                            }
                            arraySelect1 = EditorGUI.Popup(
                                new Rect(400 * rect.x, 520 * rect.y, 545 * rect.x, 25 * rect.y),
                                "Element : ",
                                arraySelect1,
                                elementName);
                            healthObj.element[arraySelect1].image = EditorGUI.ObjectField(
                                new Rect(400 * rect.x, 540 * rect.y, 100 * rect.x, 50 * rect.y),
                                healthObj.element[arraySelect1].image,
                                typeof(Texture2D),
                                true) as Texture2D;
                            healthObj.element[arraySelect1].loc = EditorGUI.RectField(
                                new Rect(400 * rect.x, 600 * rect.y, 545 * rect.x, 50 * rect.y),
                                healthObj.element[arraySelect1].loc);
                        }
                        Vector2 point = new Vector2(620 * rect.x, 650 * rect.y);
                        if (healthObj.backgroundBar != null)
                        {
                            GUI.DrawTexture(
                                new Rect(point.x, point.y, healthObj.scale * ((float) 1), healthObj.yScale),
                                healthObj.backgroundBar);
                        }
                        if (healthObj.healthBar != null)
                        {
                            GUI.DrawTexture(
                                new Rect(point.x, point.y, healthObj.scale * ((float) 50 / 100), healthObj.yScale),
                                healthObj.healthBar);
                        }
                        for (int x = 0; x < healthObj.element.Length; x++)
                        {
                            var element = healthObj.element[x];
                            if (element.image == null)
                            {
                                continue;
                            }
                            GUI.DrawTexture(
                                new Rect(
                                    point.x + element.loc.x,
                                    point.y - element.loc.y,
                                    element.loc.width,
                                    element.loc.height),
                                element.image);
                        }
                    }
                    helpState = 9;
                }
                // Production GUI
                else if (subMenuState == 1)
                {
                    GUI.DrawTexture(
                        new Rect(620 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y),
                        selectionTexture,
                        ScaleMode.StretchToFill);
                    string[] names = {"Units", "Techs", "Jobs"};
                    arraySelect =
                        EditorGUI.Popup(
                            new Rect(400 * rect.x, 50 * rect.y, 550 * rect.x, 20 * rect.y),
                            "Type : ",
                            arraySelect,
                            names);
                    if (arraySelect == 0)
                    {
                        myTarget.bGUI.unitGUI.startPos = EditorGUI.Vector2Field(
                            new Rect(400 * rect.x, 75 * rect.y, 545 * rect.x, 25 * rect.y),
                            "Start Pos : ",
                            myTarget.bGUI.unitGUI.startPos);
                        myTarget.bGUI.unitGUI.buttonSize = EditorGUI.Vector2Field(
                            new Rect(400 * rect.x, 120 * rect.y, 545 * rect.x, 25 * rect.y),
                            "Button Size : ",
                            myTarget.bGUI.unitGUI.buttonSize);
                        myTarget.bGUI.unitGUI.buttonPerRow = EditorGUI.IntField(
                            new Rect(400 * rect.x, 165 * rect.y, 545 * rect.x, 25 * rect.y),
                            "Button Per Row : ",
                            myTarget.bGUI.unitGUI.buttonPerRow);
                        myTarget.bGUI.unitGUI.displacement = EditorGUI.Vector2Field(
                            new Rect(400 * rect.x, 210 * rect.y, 545 * rect.x, 25 * rect.y),
                            "Displacement : ",
                            myTarget.bGUI.unitGUI.displacement);
                    }
                    else if (arraySelect == 1)
                    {
                        myTarget.bGUI.technologyGUI.startPos = EditorGUI.Vector2Field(
                            new Rect(400 * rect.x, 75 * rect.y, 545 * rect.x, 25 * rect.y),
                            "Start Pos : ",
                            myTarget.bGUI.technologyGUI.startPos);
                        myTarget.bGUI.technologyGUI.buttonSize = EditorGUI.Vector2Field(
                            new Rect(400 * rect.x, 120 * rect.y, 545 * rect.x, 25 * rect.y),
                            "Button Size : ",
                            myTarget.bGUI.technologyGUI.buttonSize);
                        myTarget.bGUI.technologyGUI.buttonPerRow = EditorGUI.IntField(
                            new Rect(400 * rect.x, 165 * rect.y, 545 * rect.x, 25 * rect.y),
                            "Button Per Row : ",
                            myTarget.bGUI.technologyGUI.buttonPerRow);
                        myTarget.bGUI.technologyGUI.displacement = EditorGUI.Vector2Field(
                            new Rect(400 * rect.x, 210 * rect.y, 545 * rect.x, 25 * rect.y),
                            "Displacement : ",
                            myTarget.bGUI.technologyGUI.displacement);
                    }
                    else if (arraySelect == 2)
                    {
                        myTarget.bGUI.jobsGUI.startPos = EditorGUI.Vector2Field(
                            new Rect(400 * rect.x, 75 * rect.y, 545 * rect.x, 25 * rect.y),
                            "Start Pos : ",
                            myTarget.bGUI.jobsGUI.startPos);
                        myTarget.bGUI.jobsGUI.buttonSize = EditorGUI.Vector2Field(
                            new Rect(400 * rect.x, 120 * rect.y, 545 * rect.x, 25 * rect.y),
                            "Button Size : ",
                            myTarget.bGUI.jobsGUI.buttonSize);
                        myTarget.bGUI.jobsGUI.buttonPerRow = EditorGUI.IntField(
                            new Rect(400 * rect.x, 165 * rect.y, 545 * rect.x, 25 * rect.y),
                            "Button Per Row : ",
                            myTarget.bGUI.jobsGUI.buttonPerRow);
                        myTarget.bGUI.jobsGUI.displacement = EditorGUI.Vector2Field(
                            new Rect(400 * rect.x, 210 * rect.y, 545 * rect.x, 25 * rect.y),
                            "Displacement : ",
                            myTarget.bGUI.jobsGUI.displacement);
                    }
                }
                // MiniMap
                else if (subMenuState == 2)
                {
                    GUI.DrawTexture(
                        new Rect(840 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y),
                        selectionTexture,
                        ScaleMode.StretchToFill);
                    if (myTargetMap == null)
                    {
                        if (GUI.Button(
                            new Rect(400 * rect.x, 45 * rect.y, 1100 * rect.x, 705 * rect.y),
                            "Add MiniMap Components"))
                        {
                            targetBuilding.obj.AddComponent<MiniMapSignal>();
                        }
                    }
                    else
                    {
                        myTargetMap.enabled = EditorGUI.Toggle(
                            new Rect(400 * rect.x, 70 * rect.y, 540 * rect.x, 25 * rect.y),
                            "MiniMap Signal : ",
                            myTargetMap.enabled);
                        if (!myTargetMap.enabled)
                        {
                            return;
                        }
                        myTargetMap.miniMapTag = EditorGUI.TextField(
                            new Rect(400 * rect.x, 100 * rect.y, 540 * rect.x, 25 * rect.y),
                            "Tag : ",
                            myTargetMap.miniMapTag);
                        GameObject mapObj = GameObject.Find("MiniMap");
                        Color defaultColor = GUI.color;
                        if (mapObj == null)
                        {
                            return;
                        }
                        MiniMap map = GameObject.Find("MiniMap").GetComponent<MiniMap>();
                        if (map == null)
                        {
                            helpState = 11;
                            return;
                        }
                        helpState = 10;
                        foreach (MiniMapElement mapElement in map.elements)
                        {
                            if (mapElement.tag == myTargetMap.miniMapTag)
                            {
                                GUI.color = mapElement.tints.Length > miniMapState
                                    ? mapElement.tints[miniMapState]
                                    : mapElement.tints[0];
                                int size = (int) (50 * rect.x);
                                GUI.DrawTexture(new Rect(400 * rect.x, 130 * rect.y, size, size), mapElement.image);
                                GUI.color = defaultColor;
                                mapElement.image = EditorGUI.ObjectField(
                                    new Rect(400 * rect.x, 185 * rect.y, 100, 100),
                                    mapElement.image,
                                    typeof(Texture2D),
                                    true) as Texture2D;
                                miniMapState = EditorGUI.IntField(
                                    new Rect(400 * rect.x, 320 * rect.y, 540 * rect.x, 25 * rect.y),
                                    "Faction : ",
                                    miniMapState);
                            }
                        }
                    }
                }
                // Fog Of War
                else if (subMenuState == 3)
                {
                    GUI.DrawTexture(
                        new Rect(1060 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y),
                        selectionTexture,
                        ScaleMode.StretchToFill);
                    if (myTargetVision == null)
                    {
                        if (GUI.Button(
                            new Rect(400 * rect.x, 45 * rect.y, 1100 * rect.x, 705 * rect.y),
                            "Add Vision Components"))
                        {
                            targetBuilding.obj.AddComponent<VisionSignal>();
                            // Add More Components
                        }
                    }
                    else
                    {
                        myTargetVision.enabled = EditorGUI.Toggle(
                            new Rect(400 * rect.x, 70 * rect.y, 540 * rect.x, 25 * rect.y),
                            "Vision Signal : ",
                            myTargetVision.enabled);
                        if (myTargetVision.enabled)
                        {
                            myTargetVision.radius = EditorGUI.IntField(
                                new Rect(400 * rect.x, 100 * rect.y, 540 * rect.x, 25 * rect.y),
                                "Radius : ",
                                myTargetVision.radius);
                            myTargetVision.upwardSightHeight = EditorGUI.IntField(
                                new Rect(400 * rect.x, 130 * rect.y, 540 * rect.x, 25 * rect.y),
                                "Upward Sight Height : ",
                                myTargetVision.upwardSightHeight);
                            myTargetVision.downwardSightHeight = EditorGUI.IntField(
                                new Rect(400 * rect.x, 160 * rect.y, 540 * rect.x, 25 * rect.y),
                                "Downward Sight Height : ",
                                myTargetVision.downwardSightHeight);
                        }
                        helpState = 12;
                    }
                }
                // Progress Indicator
                else if (subMenuState == 4)
                {
                    GUI.DrawTexture(
                        new Rect(1280 * rect.x, 25 * rect.y, 220 * rect.x, 20 * rect.y),
                        selectionTexture,
                        ScaleMode.StretchToFill);
                    if (targetBuilding.progressObj == null)
                    {
                        return;
                    }
                    Progress progressObj = targetBuilding.progressObj.GetComponent<Progress>();
                    if (progressObj == null)
                    {
                        if (GUI.Button(
                            new Rect(400 * rect.x, 50 * rect.y, 545 * rect.x, 750 * rect.y),
                            "Add Progress Indicator"))
                        {
                            targetBuilding.progressObj.AddComponent<Progress>();
                        }
                    }
                    else
                    {
                        progressObj.yIncrease = EditorGUI.FloatField(
                            new Rect(400 * rect.x, 70 * rect.y, 545 * rect.x, 30 * rect.y),
                            "World-Y Increase : ",
                            progressObj.yIncrease);
                        progressObj.scale = EditorGUI.IntField(
                            new Rect(400 * rect.x, 100 * rect.y, 545 * rect.x, 30 * rect.y),
                            "UI-X Scale : ",
                            progressObj.scale);
                        progressObj.yScale = EditorGUI.IntField(
                            new Rect(400 * rect.x, 130 * rect.y, 545 * rect.x, 30 * rect.y),
                            "UI-Y Scale : ",
                            progressObj.yScale);
                        progressObj.color = EditorGUI.ColorField(
                            new Rect(400 * rect.x, 160 * rect.y, 545 * rect.x, 30 * rect.y),
                            "Color : ",
                            progressObj.color);
                        int nl = progressObj.texture.Length;
                        nl = EditorGUI.IntField(
                            new Rect(400 * rect.x, 190 * rect.y, 545 * rect.x, 30 * rect.y),
                            "Textures : ",
                            nl);
                        if (nl != progressObj.texture.Length)
                        {
                            ModifyProgressTextures(nl, progressObj.texture.Length, progressObj);
                        }
                        if (nl > 0)
                        {
                            string[] textureName = new string[nl];
                            for (int x = 0; x < nl; x++)
                            {
                                textureName[x] = "Texture " + (x + 1);
                            }
                            if (arraySelect1 > nl)
                            {
                                arraySelect1 = nl - 1;
                            }
                            arraySelect1 = EditorGUI.Popup(
                                new Rect(400 * rect.x, 220 * rect.y, 545 * rect.x, 25 * rect.y),
                                "Element : ",
                                arraySelect1,
                                textureName);
                            progressObj.texture[arraySelect1] = EditorGUI.ObjectField(
                                new Rect(400 * rect.x, 250 * rect.y, 100 * rect.x, 50 * rect.y),
                                progressObj.texture[arraySelect1],
                                typeof(Texture2D),
                                true) as Texture2D;
                        }
                    }
                }
            }
            // Techs
            else if (menuState == 2)
            {
                GUI.DrawTexture(
                    new Rect(950 * rect.x, 0, 275 * rect.x, 25 * rect.y),
                    selectionTexture,
                    ScaleMode.StretchToFill);
                GUI.Box(new Rect(400 * rect.x, 25 * rect.y, 1100 * rect.x, 20 * rect.y), "");
                if (GUI.Button(new Rect(400 * rect.x, 50 * rect.y, 550 * rect.x, 20 * rect.y), "Add"))
                {
                    ModifyBuildingTechEffects(myTarget.techEffect.Length + 1, myTarget.techEffect.Length, myTarget);
                }
                if (GUI.Button(new Rect(950 * rect.x, 50 * rect.y, 550 * rect.x, 20 * rect.y), "Remove"))
                {
                    ModifyBuildingTechEffects(myTarget.techEffect.Length - 1, myTarget.techEffect.Length, myTarget);
                }
                if (nTarget.Tech.Length > 0 && myTarget.techEffect.Length > 0)
                {
                    int size = nTarget.Tech.Length;
                    string[] nameArray = new string[size];
                    string[] names = new string[size];
                    for (int x = 0; x < nameArray.Length; x++)
                    {
                        nameArray[x] = (x + 1) + ". " + nTarget.Tech[x].name;
                        names[x] = nTarget.Tech[x].name;
                    }
                    string[] unitTechs = new string[myTarget.techEffect.Length];
                    for (int x = 0; x < unitTechs.Length; x++)
                    {
                        unitTechs[x] = (x + 1) + ". " + myTarget.techEffect[x].name;
                    }
                    arraySelect = EditorGUI.Popup(
                        new Rect(400 * rect.x, 75 * rect.y, 1100 * rect.x, 20 * rect.y),
                        "Techs : ",
                        arraySelect,
                        unitTechs);
                    if (arraySelect >= myTarget.techEffect.Length)
                    {
                        arraySelect = 0;
                    }
                    int curY = 100;
                    arraySelect2 = myTarget.techEffect[arraySelect].index;
                    arraySelect2 =
                        EditorGUI.Popup(
                            new Rect(400 * rect.x, curY * rect.y, 1100 * rect.x, 20 * rect.y),
                            "Tech Options : ",
                            arraySelect2,
                            nameArray);
                    curY += 25;
                    myTarget.techEffect[arraySelect].index = arraySelect2;
                    if (arraySelect2 > nameArray.Length)
                    {
                        arraySelect2 = 0;
                    }
                    myTarget.techEffect[arraySelect].name = names[arraySelect2];
                    myTarget.techEffect[arraySelect].replacementObject = EditorGUI.ObjectField(
                        new Rect(400 * rect.x, curY * rect.y, 1100 * rect.x, 20 * rect.y),
                        "Replacement Object : ",
                        myTarget.techEffect[arraySelect].replacementObject,
                        typeof(GameObject),
                        false) as GameObject;
                    curY += 25;
                    string[,] functions = new string[18, 4]
                    {
                        /* 0 */ {"GetName", "SetName", "", ""},
                        /* 1 */ {"GetMaxHealth", "SetMaxHealth", "AddMaxHealth", "SubMaxHealth"},
                        /* 2 */ {"GetHealth", "SetHealth", "AddHealth", "SubHealth"},
                        /* 3 */ {"GetFaction", "SetFaction", "", ""},
                        /* 4 */ {"GetUCanProduce", "SetUCanProduce", "", ""},
                        /* 5 */ {"GetUICanProduce", "SetUICanProduce", "", ""},
                        /* 6 */ {"GetUCost", "SetUCost", "AddUCost", "SubUCost"},
                        /* 7 */ {"GetUDur", "SetUDur", "AddUDur", "SubUDur"},
                        /* 8 */ {"GetURate", "SetURate", "AddURate", "SubURate"},
                        /* 9 */
                        {"GetUCanBuildAtOnce", "SetUCanBuildAtOnce", "AddUCanBuildAtOnce", "SubUCanBuildAtOnce"},
                        /* 10 */ {"GetUMaxAmount", "SetUMaxAmount", "AddUMaxAmount", "SubUMaxAmount"},
                        /* 11 */ {"GetTCanProduce", "SetTCanProduce", "", ""},
                        /* 12 */ {"GetTCost", "SetTCost", "AddTCost", "SubTCost"},
                        /* 13 */ {"GetTICanProduce", "SetTICanProduce", "", ""},
                        /* 14 */ {"GetTDur", "SetTDur", "AddTDur", "SubTDur"},
                        /* 15 */ {"GetTRate", "SetTRate", "AddTRate", "SubTRate"},
                        /* 16 */ {"GetTMaxAmount", "SetTMaxAmount", "AddTMaxAmount", "SubTMaxAmount"},
                        /* 17 */
                        {"GetTCanBuildAtOnce", "SetTCanBuildAtOnce", "AddTCanBuildAtOnce", "SubTCanBuildAtOnce"},
                    };
                    string[] variableName =
                    {
                        "Name", "Max Health", "Health", "Group",
                        "UCanProduce", "UICanProduce", "UCost", "UDur", "URate",
                        "UCanBuildAtOnce", "UMaxAmount",
                        "TCanProduce", "TCost", "TICanProduce", "TDur",
                        "TRate", "TMaxAmount", "TCanBuildAtOnce"
                    };
                    for (int x = 0; x < myTarget.techEffect[arraySelect].effects.Length; x++)
                    {
                        if (myTarget.techEffect[arraySelect].effects[x] == null)
                        {
                            myTarget.techEffect[arraySelect].effects[x] = new Effects();
                        }
                        myTarget.techEffect[arraySelect].effects[x].effectName = EditorGUI.Popup(
                            new Rect(400 * rect.x, curY * rect.y, 400 * rect.x, 20 * rect.y),
                            "",
                            myTarget.techEffect[arraySelect].effects[x].effectName,
                            variableName);
                        int index = myTarget.techEffect[arraySelect].effects[x].effectName;
                        //{
                        string[] funcTypes;
                        if (index == 0 || index == 3 || index == 4 || index == 5 || index == 11 || index == 13)
                        {
                            funcTypes = new string[2];
                            funcTypes[0] = "Get";
                            funcTypes[1] = "Set";
                        }
                        else
                        {
                            funcTypes = new string[4];
                            funcTypes[0] = "Get";
                            funcTypes[1] = "Set";
                            funcTypes[2] = "Add";
                            funcTypes[3] = "Sub";
                        }
                        myTarget.techEffect[arraySelect].effects[x].funcType = EditorGUI.Popup(
                            new Rect(800 * rect.x, curY * rect.y, 200 * rect.x, 20 * rect.y),
                            "",
                            myTarget.techEffect[arraySelect].effects[x].funcType,
                            funcTypes);
                        myTarget.techEffect[arraySelect].effects[x].funcName = functions[
                            index,
                            myTarget.techEffect[arraySelect].effects[x].funcType];
                        //}
                        if (myTarget.techEffect[arraySelect].effects[x].funcType == 0)
                        {
                            EditorGUI.LabelField(
                                new Rect(1000 * rect.x, curY * rect.y, 290 * rect.x, 20 * rect.y),
                                "Getter");
                        }
                        else
                        {
                            string[] produceNames = new string[myTarget.unitProduction.units.Length];
                            for (int y = 0; y < myTarget.unitProduction.units.Length; y++)
                            {
                                if (myTarget.unitProduction.units[y] != null)
                                {
                                    produceNames[y] = (y + 1) + ". " + myTarget.unitProduction.units[y].customName;
                                }
                            }
                            string[] produceTNames = new string[myTarget.techProduction.techs.Length];
                            for (int y = 0; y < myTarget.techProduction.techs.Length; y++)
                            {
                                if (myTarget.techProduction.techs[y] != null)
                                {
                                    produceTNames[y] = (y + 1) + ". " + myTarget.techProduction.techs[y].customName;
                                }
                            }
                            switch (index)
                            {
                                case 0:
                                    myTarget.techEffect[arraySelect].effects[x].text = EditorGUI.TextField(
                                        new Rect(1000 * rect.x, curY * rect.y, 290 * rect.x, 20 * rect.y),
                                        "",
                                        myTarget.techEffect[arraySelect].effects[x].text);
                                    break;
                                case 4:
                                case 11:
                                    myTarget.techEffect[arraySelect].effects[x].toggle = EditorGUI.Toggle(
                                        new Rect(1000 * rect.x, curY * rect.y, 290 * rect.x, 20 * rect.y),
                                        "",
                                        myTarget.techEffect[arraySelect].effects[x].toggle);
                                    break;
                                case 5:
                                    myTarget.techEffect[arraySelect].effects[x].index = EditorGUI.Popup(
                                        new Rect(1290 * rect.x, curY * rect.y, 210 * rect.x, 20 * rect.y),
                                        "",
                                        myTarget.techEffect[arraySelect].effects[x].index,
                                        produceNames);
                                    myTarget.techEffect[arraySelect].effects[x].toggle = EditorGUI.Toggle(
                                        new Rect(1000 * rect.x, curY * rect.y, 290 * rect.x, 20 * rect.y),
                                        "",
                                        myTarget.techEffect[arraySelect].effects[x].toggle);
                                    break;
                                case 13:
                                    myTarget.techEffect[arraySelect].effects[x].index = EditorGUI.Popup(
                                        new Rect(1290 * rect.x, curY * rect.y, 210 * rect.x, 20 * rect.y),
                                        "",
                                        myTarget.techEffect[arraySelect].effects[x].index,
                                        produceTNames);
                                    myTarget.techEffect[arraySelect].effects[x].toggle = EditorGUI.Toggle(
                                        new Rect(1000 * rect.x, curY * rect.y, 290 * rect.x, 20 * rect.y),
                                        "",
                                        myTarget.techEffect[arraySelect].effects[x].toggle);
                                    break;
                                case 7:
                                case 8:
                                    myTarget.techEffect[arraySelect].effects[x].index = EditorGUI.Popup(
                                        new Rect(1290 * rect.x, curY * rect.y, 210 * rect.x, 20 * rect.y),
                                        "",
                                        myTarget.techEffect[arraySelect].effects[x].index,
                                        produceNames);
                                    myTarget.techEffect[arraySelect].effects[x].amount = EditorGUI.FloatField(
                                        new Rect(1000 * rect.x, curY * rect.y, 290 * rect.x, 20 * rect.y),
                                        "",
                                        myTarget.techEffect[arraySelect].effects[x].amount);
                                    break;
                                case 14:
                                case 15:
                                    myTarget.techEffect[arraySelect].effects[x].index = EditorGUI.Popup(
                                        new Rect(1290 * rect.x, curY * rect.y, 210 * rect.x, 20 * rect.y),
                                        "",
                                        myTarget.techEffect[arraySelect].effects[x].index,
                                        produceTNames);
                                    myTarget.techEffect[arraySelect].effects[x].amount = EditorGUI.FloatField(
                                        new Rect(1000 * rect.x, curY * rect.y, 290 * rect.x, 20 * rect.y),
                                        "",
                                        myTarget.techEffect[arraySelect].effects[x].amount);
                                    break;
                                case 6:
                                case 12:
                                    // Resource List
                                    // Current Resource Manager will be used for read-only purposes
                                    ResourceManager resourceManager = GameManager.Instance.CurrentPlayerResourceManager;
                                    string[] resourceNames = new string[resourceManager.resourceTypes.Length];
                                    for (int y = 0; y < resourceNames.Length; y++)
                                    {
                                        resourceNames[y] = resourceManager.resourceTypes[y].name;
                                    }
                                    if (arraySelect >= resourceNames.Length)
                                    {
                                        arraySelect = 0;
                                    }
                                    else
                                    {
                                        if (myTarget.techEffect[arraySelect].effects[x].index >= resourceNames.Length)
                                        {
                                            myTarget.techEffect[arraySelect].effects[x].index = 0;
                                        }
                                        if (index == 6)
                                        {
                                            myTarget.techEffect[arraySelect].effects[x].index = EditorGUI.Popup(
                                                new Rect(1290 * rect.x, curY * rect.y, 100 * rect.x, 20 * rect.y),
                                                "",
                                                myTarget.techEffect[arraySelect].effects[x].index,
                                                produceNames);
                                            myTarget.techEffect[arraySelect].effects[x].index1 = EditorGUI.Popup(
                                                new Rect(1390 * rect.x, curY * rect.y, 110 * rect.x, 20 * rect.y),
                                                "",
                                                myTarget.techEffect[arraySelect].effects[x].index1,
                                                resourceNames);
                                            myTarget.techEffect[arraySelect].effects[x].amount = EditorGUI.FloatField(
                                                new Rect(1000 * rect.x, curY * rect.y, 290 * rect.x, 20 * rect.y),
                                                "",
                                                myTarget.techEffect[arraySelect].effects[x].amount);
                                        }
                                        else
                                        {
                                            myTarget.techEffect[arraySelect].effects[x].index = EditorGUI.Popup(
                                                new Rect(1290 * rect.x, curY * rect.y, 100 * rect.x, 20 * rect.y),
                                                "",
                                                myTarget.techEffect[arraySelect].effects[x].index,
                                                produceTNames);
                                            myTarget.techEffect[arraySelect].effects[x].index1 = EditorGUI.Popup(
                                                new Rect(1390 * rect.x, curY * rect.y, 110 * rect.x, 20 * rect.y),
                                                "",
                                                myTarget.techEffect[arraySelect].effects[x].index1,
                                                resourceNames);
                                            myTarget.techEffect[arraySelect].effects[x].amount = EditorGUI.FloatField(
                                                new Rect(1000 * rect.x, curY * rect.y, 290 * rect.x, 20 * rect.y),
                                                "",
                                                myTarget.techEffect[arraySelect].effects[x].amount);
                                        }
                                    }
                                    break;
                                default:
                                    myTarget.techEffect[arraySelect].effects[x].amount = EditorGUI.FloatField(
                                        new Rect(1000 * rect.x, curY * rect.y, 290 * rect.x, 20 * rect.y),
                                        "",
                                        myTarget.techEffect[arraySelect].effects[x].amount);
                                    break;
                            }
                        }
                        curY += 25;
                    }
                    if (GUI.Button(new Rect(400 * rect.x, curY * rect.y, 273 * rect.x, 20 * rect.y), "Add"))
                    {
                        ModifyBuildingTechEffectEffects(
                            myTarget.techEffect[arraySelect].effects.Length + 1,
                            myTarget.techEffect[arraySelect].effects.Length,
                            myTarget,
                            arraySelect);
                    }
                    if (GUI.Button(new Rect(673 * rect.x, curY * rect.y, 272 * rect.x, 20 * rect.y), "Remove"))
                    {
                        ModifyBuildingTechEffectEffects(
                            myTarget.techEffect[arraySelect].effects.Length - 1,
                            myTarget.techEffect[arraySelect].effects.Length,
                            myTarget,
                            arraySelect);
                    }
                }
            }
            // Anim/Sounds
            else if (menuState == 3)
            {
                if (targetBuilding.obj != lastObj)
                {
                    objEditor = Editor.CreateEditor(targetBuilding.obj);
                    lastObj = targetBuilding.obj;
                }
                objEditor.OnInteractivePreviewGUI(
                    new Rect(945 * rect.x, 50 * rect.y, 555 * rect.x, 650 * rect.y),
                    EditorStyles.toolbarButton);
                GUI.DrawTexture(
                    new Rect(1225 * rect.x, 0, 275 * rect.x, 25 * rect.y),
                    selectionTexture,
                    ScaleMode.StretchToFill);
                GUI.Box(new Rect(400 * rect.x, 25 * rect.y, 1100 * rect.x, 20 * rect.y), "");
                Animator comp = targetBuilding.obj.GetComponent<Animator>();
                if (myTarget.anim.manager)
                {
                    myTarget.anim.manager.runtimeAnimatorController = EditorGUI.ObjectField(
                        new Rect(400 * rect.x, 70 * rect.y, 540 * rect.x, 25 * rect.y),
                        "Controller : ",
                        myTarget.anim.manager.runtimeAnimatorController,
                        typeof(RuntimeAnimatorController),
                        false) as RuntimeAnimatorController;
                }
                else if (comp)
                {
                    myTarget.anim.manager = comp;
                }
                else
                {
                    if (GUI.Button(new Rect(400 * rect.x, 70 * rect.y, 540 * rect.x, 25 * rect.y), "Add Animator"))
                    {
                        targetBuilding.obj.AddComponent<Animator>();
                    }
                }
                myTarget.anim.idleAudio = EditorGUI.ObjectField(
                    new Rect(400 * rect.x, 100 * rect.y, 540 * rect.x, 25 * rect.y), "Idle Audio : ",
                    myTarget.anim.idleAudio,
                    typeof(AudioClip),
                    false) as AudioClip;
                myTarget.anim.buildUnitAudio = EditorGUI.ObjectField(
                    new Rect(400 * rect.x, 130 * rect.y, 540 * rect.x, 25 * rect.y), "Build Unit : ",
                    myTarget.anim.buildUnitAudio,
                    typeof(AudioClip),
                    false) as AudioClip;
                myTarget.anim.buildTechAudio = EditorGUI.ObjectField(
                    new Rect(400 * rect.x, 160 * rect.y, 540 * rect.x, 25 * rect.y), "Build Tech : ",
                    myTarget.anim.buildTechAudio,
                    typeof(AudioClip),
                    false) as AudioClip;
            }
        }

        bool isWithin(Rect loc)
        {
            Event e = Event.current;
            return e.button == 0 && e.isMouse && !(e.mousePosition.x < loc.x
                                                   || e.mousePosition.x > loc.x + loc.width
                                                   || e.mousePosition.y < loc.y
                                                   || e.mousePosition.y > loc.y + loc.height);
        }

        // Functions for Modifying custom class arrays

        void ModifyFactions(int nl, int ol, int curLoc)
        {
            GameObject[] copyArr = new GameObject[ol];
            for (int x = 0; x < copyArr.Length; x++)
            {
                if (target.FactionList[x])
                {
                    copyArr[x] = target.FactionList[x];
                }
            }
            target.FactionList = new GameObject[nl];
            int y = 0;
            if (nl < ol)
            {
                for (int x = 0; x < copyArr.Length; x++)
                {
                    if (x == curLoc)
                    {
                        continue;
                    }
                    if (copyArr[x])
                    {
                        target.FactionList[y] = copyArr[x];
                    }
                    y++;
                }
            }
            else
            {
                for (int x = 0; x < target.FactionList.Length; x++)
                {
                    if (x == curLoc + 1)
                    {
                        continue;
                    }
                    if (copyArr[y])
                    {
                        target.FactionList[x] = copyArr[y];
                    }
                    y++;
                }
            }
            // After a Faction gets added or removed, relations need to be updated
            InitializeFactionRelations();
        }

        Technology[] ModifyTechs(int nl, int ol, Technology[] techs)
        {
            Technology[] copyArr = new Technology[ol];
            for (int x = 0; x < copyArr.Length; x++)
            {
                copyArr[x] = techs[x];
            }
            techs = new Technology[nl];
            int y = 0;
            if (nl < ol)
            {
                for (int x = 0; x < techs.Length; x++)
                {
                    techs[y] = copyArr[x];
                    y++;
                }
            }
            else
            {
                for (int x = 0; x < copyArr.Length; x++)
                {
                    techs[x] = copyArr[x];
                }
                techs[techs.Length - 1] = new Technology();
            }
            return techs;
        }

        void ModifyFactionUnits(int nl, int ol, int curLoc)
        {
            Unit[] copyArr = new Unit[ol];
            for (int x = 0; x < copyArr.Length; x++)
            {
                copyArr[x] = new Unit();
                copyArr[x] = nTarget.UnitList[x];
            }
            nTarget.UnitList = new Unit[nl];
            int y = 0;
            if (nl < ol)
            {
                for (int x = 0; x < copyArr.Length; x++)
                {
                    if (x != curLoc)
                    {
                        nTarget.UnitList[y] = copyArr[x];
                        y++;
                    }
                }
                for (int x = 0; x < nTarget.UnitList.Length; x++)
                {
                    if (nTarget.UnitList[x] == null)
                    {
                        nTarget.UnitList[x] = new Unit();
                    }
                }
            }
            else
            {
                y = 0;
                for (int x = 0; x < nTarget.UnitList.Length; x++)
                {
                    if (x != curLoc + 1)
                    {
                        nTarget.UnitList[x] = new Unit();
                        nTarget.UnitList[x] = copyArr[y];
                        y++;
                    }
                    else
                    {
                        nTarget.UnitList[x] = new Unit();
                    }
                }
            }
        }

        void ModifyFactionTypes(int nl, int ol, int curLoc)
        {
            UnitType[] copyArr = new UnitType[ol];
            for (int x = 0; x < copyArr.Length; x++)
            {
                copyArr[x] = new UnitType();
                copyArr[x] = target.UnitTypes[x];
            }
            target.UnitTypes = new UnitType[nl];
            int y = 0;
            if (nl < ol)
            {
                for (int x = 0; x < copyArr.Length; x++)
                {
                    if (x != curLoc)
                    {
                        target.UnitTypes[y] = copyArr[x];
                        y++;
                    }
                }
            }
            else
            {
                y = 0;
                for (int x = 0; x < target.UnitTypes.Length; x++)
                {
                    if (x != curLoc + 1)
                    {
                        target.UnitTypes[x] = new UnitType();
                        target.UnitTypes[x] = copyArr[y];
                        y++;
                    }
                    else
                    {
                        target.UnitTypes[x] = new UnitType();
                    }
                }
            }
        }

        void ModifyFactionTypesStrengths(int nl, int ol, int curLoc, int loc)
        {
            Ratio[] copyArr = new Ratio[ol];
            for (int x = 0; x < copyArr.Length; x++)
            {
                copyArr[x] = new Ratio();
                copyArr[x] = target.UnitTypes[loc].strengths[x];
            }
            target.UnitTypes[loc].strengths = new Ratio[nl];
            int y = 0;
            if (nl < ol)
            {
                for (int x = 0; x < copyArr.Length; x++)
                {
                    if (x != curLoc)
                    {
                        target.UnitTypes[loc].strengths[y] = copyArr[x];
                        y++;
                    }
                }
            }
            else
            {
                y = 0;
                for (int x = 0; x < target.UnitTypes[loc].strengths.Length; x++)
                {
                    if (x != curLoc + 1)
                    {
                        target.UnitTypes[loc].strengths[x] = new Ratio();
                        target.UnitTypes[loc].strengths[x] = copyArr[y];
                        y++;
                    }
                    else
                    {
                        target.UnitTypes[loc].strengths[x] = new Ratio();
                    }
                }
            }
        }

        void ModifyFactionTypesWeaknesses(int nl, int ol, int curLoc, int loc)
        {
            Ratio[] copyArr = new Ratio[ol];
            for (int x = 0; x < copyArr.Length; x++)
            {
                copyArr[x] = new Ratio();
                copyArr[x] = target.UnitTypes[loc].weaknesses[x];
            }
            target.UnitTypes[loc].weaknesses = new Ratio[nl];
            int y = 0;
            if (nl < ol)
            {
                for (int x = 0; x < copyArr.Length; x++)
                {
                    if (x != curLoc)
                    {
                        target.UnitTypes[loc].weaknesses[y] = copyArr[x];
                        y++;
                    }
                }
            }
            else
            {
                y = 0;
                for (int x = 0; x < target.UnitTypes[loc].weaknesses.Length; x++)
                {
                    if (x != curLoc + 1)
                    {
                        target.UnitTypes[loc].weaknesses[x] = new Ratio();
                        target.UnitTypes[loc].weaknesses[x] = copyArr[y];
                        y++;
                    }
                    else
                    {
                        target.UnitTypes[loc].weaknesses[x] = new Ratio();
                    }
                }
            }
        }

        void ModifyFactionBuildings(int nl, int ol, int curLoc)
        {
            Building[] copyArr = new Building[ol];
            for (int x = 0; x < copyArr.Length; x++)
            {
                copyArr[x] = new Building();
                copyArr[x] = nTarget.BuildingList[x];
            }
            nTarget.BuildingList = new Building[nl];
            int y = 0;
            if (nl < ol)
            {
                for (int x = 0; x < copyArr.Length; x++)
                {
                    if (x != curLoc)
                    {
                        nTarget.BuildingList[y] = copyArr[x];
                        y++;
                    }
                }
                for (int x = 0; x < nTarget.BuildingList.Length; x++)
                {
                    if (nTarget.BuildingList[x] == null)
                    {
                        nTarget.BuildingList[x] = new Building();
                    }
                }
            }
            else
            {
                y = 0;
                for (int x = 0; x < nTarget.BuildingList.Length; x++)
                {
                    if (x != curLoc + 1)
                    {
                        nTarget.BuildingList[x] = new Building();
                        nTarget.BuildingList[x] = copyArr[y];
                        y++;
                    }
                    else
                    {
                        nTarget.BuildingList[x] = new Building();
                    }
                }
            }
        }

        void ModifyFactionUnitProduction(int nl, int ol, int curLoc)
        {
            BuildingController buildCont = nTarget.BuildingList[unitId].obj.GetComponent<BuildingController>();
            ProduceUnit[] copyArr = new ProduceUnit[ol];
            for (int x = 0; x < copyArr.Length; x++)
            {
                copyArr[x] = buildCont.unitProduction.units[x];
            }
            buildCont.unitProduction.units = new ProduceUnit[nl];
            int y = 0;
            if (nl < ol)
            {
                for (int x = 0; x < copyArr.Length; x++)
                {
                    if (x != curLoc)
                    {
                        buildCont.unitProduction.units[y] = copyArr[x];
                        y++;
                    }
                }
            }
            else
            {
                for (int x = 0; x < ol; x++)
                {
                    if (x != curLoc + 1)
                    {
                        buildCont.unitProduction.units[x] = copyArr[y];
                        y++;
                    }
                }
                for (int x = ol; x < buildCont.unitProduction.units.Length; x++)
                {
                    buildCont.unitProduction.units[x] = new ProduceUnit();
                    y++;
                }
            }
        }

        void ModifyFactionTechnologyProduction(int nl, int ol, int curLoc)
        {
            BuildingController buildCont = nTarget.BuildingList[unitId].obj.GetComponent<BuildingController>();
            ProduceTech[] copyArr = new ProduceTech[ol];
            for (int x = 0; x < copyArr.Length; x++)
            {
                copyArr[x] = buildCont.techProduction.techs[x];
            }
            buildCont.techProduction.techs = new ProduceTech[nl];
            int y = 0;
            if (nl < ol)
            {
                for (int x = 0; x < copyArr.Length; x++)
                {
                    if (x != curLoc)
                    {
                        buildCont.techProduction.techs[y] = copyArr[x];
                        y++;
                    }
                }
            }
            else
            {
                for (int x = 0; x < buildCont.techProduction.techs.Length; x++)
                {
                    if (x != curLoc + 1)
                    {
                        buildCont.techProduction.techs[x] = copyArr[y];
                        y++;
                    }
                }
            }
        }

        void ModifyUnitGathering(int nl, int ol, UnitController targ)
        {
            ResourceBehaviour[] copyArr = new ResourceBehaviour[ol];
            for (int x = 0; x < copyArr.Length; x++)
            {
                copyArr[x] = targ.resource.behaviour[x];
            }
            targ.resource.behaviour = new ResourceBehaviour[nl];
            if (nl > ol)
            {
                for (int x = 0; x < ol; x++)
                {
                    targ.resource.behaviour[x] = copyArr[x];
                }
                for (int x = ol; x < nl; x++)
                {
                    targ.resource.behaviour[x] = new ResourceBehaviour();
                }
            }
            else
            {
                for (int x = 0; x < nl; x++)
                {
                    targ.resource.behaviour[x] = copyArr[x];
                }
            }
        }

        void ModifyUnitTechEffects(int nl, int ol, UnitController targ)
        {
            TechEffect[] copyArr = new TechEffect[ol];
            for (int x = 0; x < copyArr.Length; x++)
            {
                copyArr[x] = targ.techEffect[x];
            }
            targ.techEffect = new TechEffect[nl];
            if (nl > ol)
            {
                for (int x = 0; x < copyArr.Length; x++)
                {
                    targ.techEffect[x] = copyArr[x];
                }
                targ.techEffect[nl - 1] = new TechEffect();
            }
            else
            {
                for (int x = 0; x < targ.techEffect.Length; x++)
                {
                    targ.techEffect[x] = copyArr[x];
                }
            }
        }

        void ModifyUnitRatios(int nl, int ol, UnitController targ)
        {
            SRatio[] copyArr = new SRatio[ol];
            for (int x = 0; x < copyArr.Length; x++)
            {
                copyArr[x] = targ.ratio[x];
            }
            targ.ratio = new SRatio[nl];
            if (nl > ol)
            {
                for (int x = 0; x < copyArr.Length; x++)
                {
                    targ.ratio[x] = copyArr[x];
                }
                targ.ratio[nl - 1] = new SRatio();
            }
            else
            {
                for (int x = 0; x < targ.ratio.Length; x++)
                {
                    targ.ratio[x] = copyArr[x];
                }
            }
        }

        void ModifyUnitBuildBehaviour(int nl, int ol, UnitController targ)
        {
            BuildBehaviour[] copyArr = new BuildBehaviour[ol];
            for (int x = 0; x < copyArr.Length; x++)
            {
                copyArr[x] = targ.build.build[x];
            }
            targ.build.build = new BuildBehaviour[nl];
            if (nl > ol)
            {
                for (int x = 0; x < copyArr.Length; x++)
                {
                    targ.build.build[x] = copyArr[x];
                }
                targ.build.build[nl - 1] = new BuildBehaviour();
            }
            else
            {
                for (int x = 0; x < targ.build.build.Length; x++)
                {
                    targ.build.build[x] = copyArr[x];
                }
            }
        }

        void ModifyUnitSelectedObjects(int nl, int ol, UnitController targ)
        {
            GameObject[] copyArr = new GameObject[ol];
            for (int x = 0; x < copyArr.Length; x++)
            {
                copyArr[x] = targ.gui.selectObjs[x];
            }
            targ.gui.selectObjs = new GameObject[nl];
            if (nl > ol)
            {
                for (int x = 0; x < copyArr.Length; x++)
                {
                    targ.gui.selectObjs[x] = copyArr[x];
                }
            }
            else
            {
                for (int x = 0; x < targ.gui.selectObjs.Length; x++)
                {
                    targ.gui.selectObjs[x] = copyArr[x];
                }
            }
        }

        void ModifyUnitTechEffectEffects(int nl, int ol, UnitController targ, int index)
        {
            Effects[] copyArr = new Effects[ol];
            for (int x = 0; x < copyArr.Length; x++)
            {
                copyArr[x] = targ.techEffect[index].effects[x];
            }
            targ.techEffect[index].effects = new Effects[nl];
            if (nl > ol)
            {
                for (int x = 0; x < copyArr.Length; x++)
                {
                    targ.techEffect[index].effects[x] = copyArr[x];
                }
                targ.techEffect[index].effects[nl - 1] = new Effects();
            }
            else
            {
                for (int x = 0; x < targ.techEffect[index].effects.Length; x++)
                {
                    targ.techEffect[index].effects[x] = copyArr[x];
                }
            }
        }

        void ModifyBuildingTechEffects(int nl, int ol, BuildingController targ)
        {
            TechEffect[] copyArr = new TechEffect[ol];
            for (int x = 0; x < copyArr.Length; x++)
            {
                copyArr[x] = targ.techEffect[x];
            }
            targ.techEffect = new TechEffect[nl];
            if (nl > ol)
            {
                for (int x = 0; x < copyArr.Length; x++)
                {
                    targ.techEffect[x] = copyArr[x];
                }
                targ.techEffect[nl - 1] = new TechEffect();
            }
            else
            {
                for (int x = 0; x < targ.techEffect.Length; x++)
                {
                    targ.techEffect[x] = copyArr[x];
                }
            }
        }

        void ModifyBuildingTechEffectEffects(int nl, int ol, BuildingController targ, int index)
        {
            Effects[] copyArr = new Effects[ol];
            for (int x = 0; x < copyArr.Length; x++)
            {
                copyArr[x] = targ.techEffect[index].effects[x];
            }
            targ.techEffect[index].effects = new Effects[nl];
            if (nl > ol)
            {
                for (int x = 0; x < copyArr.Length; x++)
                {
                    targ.techEffect[index].effects[x] = copyArr[x];
                }
            }
            else
            {
                for (int x = 0; x < targ.techEffect[index].effects.Length; x++)
                {
                    targ.techEffect[index].effects[x] = copyArr[x];
                }
            }
        }

        void ModifyBuildingSelectedObjects(int nl, int ol, BuildingController targ)
        {
            GameObject[] copyArr = new GameObject[ol];
            for (int x = 0; x < copyArr.Length; x++)
            {
                copyArr[x] = targ.gui.selectObjs[x];
            }
            targ.gui.selectObjs = new GameObject[nl];
            if (nl > ol)
            {
                for (int x = 0; x < copyArr.Length; x++)
                {
                    targ.gui.selectObjs[x] = copyArr[x];
                }
            }
            else
            {
                for (int x = 0; x < targ.gui.selectObjs.Length; x++)
                {
                    targ.gui.selectObjs[x] = copyArr[x];
                }
            }
        }

        void ModifyFactionRelations(int nl, int ol)
        {
            Relation[] copyArr = new Relation[ol];
            for (int x = 0; x < copyArr.Length; x++)
            {
                copyArr[x] = nTarget.Relations[x];
            }
            nTarget.Relations = new Relation[nl];
            if (nl > ol)
            {
                for (int x = 0; x < copyArr.Length; x++)
                {
                    nTarget.Relations[x] = copyArr[x];
                }
                nTarget.Relations[nl - 1] = new Relation();
            }
            else
            {
                for (int x = 0; x < nTarget.Relations.Length; x++)
                {
                    nTarget.Relations[x] = copyArr[x];
                }
            }
        }

        void ModifyHealthElements(int nl, int ol, Health health)
        {
            HealthElement[] copyArr = new HealthElement[ol];
            for (int x = 0; x < copyArr.Length; x++)
            {
                copyArr[x] = health.element[x];
            }
            health.element = new HealthElement[nl];
            if (nl > ol)
            {
                for (int x = 0; x < ol; x++)
                {
                    health.element[x] = copyArr[x];
                }
                for (int x = ol; x < nl; x++)
                {
                    // Default HealthElement: copies the image from a first element and first 4 will form a rectangle
                    health.element[x] = new HealthElement
                    {
                        image = x == 0 ? null : health.element[0].image
                    };
                    switch (x)
                    {
                        case 0:
                            health.element[x].loc = new Rect(0, 0, 50, 1);
                            break;
                        case 1:
                            health.element[x].loc = new Rect(0, -6, 50, 1);
                            break;
                        case 2:
                            health.element[x].loc = new Rect(0, 0, 2, 6);
                            break;
                        case 3:
                            health.element[x].loc = new Rect(48, 0, 2, 6);
                            break;
                    }
                }
            }
            else
            {
                for (int x = 0; x < health.element.Length; x++)
                {
                    health.element[x] = copyArr[x];
                }
            }
        }

        void ModifyProgressTextures(int nl, int ol, Progress progress)
        {
            Texture[] copyArr = new Texture[ol];
            for (int x = 0; x < copyArr.Length; x++)
            {
                copyArr[x] = progress.texture[x];
            }
            progress.texture = new Texture[nl];
            if (nl > ol)
            {
                for (int x = 0; x < copyArr.Length; x++)
                {
                    progress.texture[x] = copyArr[x];
                }
                progress.texture[nl - 1] = null;
            }
            else
            {
                for (int x = 0; x < progress.texture.Length; x++)
                {
                    progress.texture[x] = copyArr[x];
                }
            }
        }

        // There are a lot of them
    }
}
