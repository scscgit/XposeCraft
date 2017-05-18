using System.Collections.Generic;
using UnityEngine;
using XposeCraft.GameInternal;

namespace XposeCraft.Game
{
    /// <summary>
    /// Game tutorial for making the jump-start into the game easier for new programmers.
    /// </summary>
    internal class Tutorial : MonoBehaviour
    {
        internal enum TutorialStep
        {
            // Preparation
            Disabled,
            Initial,
            Welcome,

            // Courses
            FindingWorker,
            Gathering,
            Events,
            Production,
            Building,
            HotSwap,
            ActionQueue,

            // Default
            Finished
        }

        private TutorialStep _state = TutorialStep.Initial;
        private TutorialStep _maxState = TutorialStep.Welcome;
        private string _text;
        private bool _hidden;
        private List<TutorialStep> _finishedStepsForSkip = new List<TutorialStep>();

        private static Tutorial _instance;

        internal static Tutorial Instance
        {
            get { return _instance ?? (_instance = GameObject.Find(GameManager.ScriptName).GetComponent<Tutorial>()); }
        }

        public void TutorialResetIfPlayer()
        {
            if (!IsGuiPlayer())
            {
                return;
            }
            _state = TutorialStep.Initial;
            _maxState = TutorialStep.Welcome;
        }

        public void TutorialStart()
        {
            if (_maxState <= TutorialStep.Welcome)
            {
                _maxState = TutorialStep.FindingWorker;
            }
            if (_state <= TutorialStep.Welcome)
            {
                _state = _maxState;
            }
        }

        public void TutorialStop()
        {
            _state = TutorialStep.Disabled;
        }

        public void GetUnitOrBuilding()
        {
            StepFinished(TutorialStep.FindingWorker);
        }

        public void SendGather()
        {
            StepFinished(TutorialStep.Gathering);
        }

        public void EventMineralsChanged()
        {
            StepFinished(TutorialStep.Events);
        }

        public void UnitProduction()
        {
            StepFinished(TutorialStep.Production);
            // This won't be triggered again if he stops the production to save resources
            if (!_finishedStepsForSkip.Contains(TutorialStep.Production))
            {
                _finishedStepsForSkip.Add(TutorialStep.Production);
            }
        }

        public void CreateBuilding()
        {
            StepFinished(TutorialStep.Building);
            // One building is enough, he would have to find a new empty spot after each placement
            if (!_finishedStepsForSkip.Contains(TutorialStep.Building))
            {
                _finishedStepsForSkip.Add(TutorialStep.Building);
            }
        }

        public void OnHotSwap()
        {
            if (!IsGuiPlayer())
            {
                return;
            }
            // This won't immediately skip all previous steps
            if (!_finishedStepsForSkip.Contains(TutorialStep.HotSwap))
            {
                _finishedStepsForSkip.Add(TutorialStep.HotSwap);
            }
            if (_maxState == TutorialStep.HotSwap)
            {
                _maxState++;
            }
        }

        public void ActionQueue()
        {
            StepFinished(TutorialStep.ActionQueue);
        }

        private void StepFinished(TutorialStep stepFinished)
        {
            if (_maxState > stepFinished || !IsGuiPlayer())
            {
                return;
            }
            _maxState = stepFinished + 1;

            // If there are actions that should be trusted to have finished, they are skipped
            while (_finishedStepsForSkip.Contains(_maxState))
            {
                _maxState++;
            }
        }

        private static bool IsGuiPlayer()
        {
            return Player.CurrentPlayer.Equals(GameManager.Instance.GuiPlayer);
        }

        private void OnGUI()
        {
            // Disabled state
            if (_text == null)
            {
                return;
            }
            var hideButtonRect = new Rect(
                Screen.width - 2.3f * Screen.width / 10f,
                Screen.height - 4.5f * Screen.height / 10f,
                Screen.width / 10f,
                0.5f * Screen.height / 10f);
            if (_hidden)
            {
                if (GUI.Button(hideButtonRect, "Návod"))
                {
                    _hidden = false;
                }
            }
            else if (GUI.Button(hideButtonRect, "Skryť návod"))
            {
                _hidden = true;
            }
            // Hidden button pressed
            if (_hidden)
            {
                return;
            }
            GUI.skin.box.fontSize = 13;
            GUI.color = new Color(1f, 0.92f, 0.16f, 1f);
            var rectangle = new Rect(
                Screen.width - 7.7f * Screen.width / 10f,
                Screen.height - 9 * Screen.height / 10f,
                5.4f * Screen.width / 10f,
                Screen.height / 2f);
            GUI.Box(rectangle, _text);
        }

        private void FixedUpdate()
        {
            switch (_state)
            {
                case TutorialStep.Initial:
                    // After the game restart, the welcome screen will not display until the second frame
                    _state = TutorialStep.Welcome;
                    return;

                case TutorialStep.Disabled:
                    _text = null;
                    return;

                case TutorialStep.Welcome:
                    _text =
                        "Vitajte v hre XposeCraft. Tento návod vám postupne ukáže základy hry.\n" +
                        "\n" +
                        "Prvou úlohou je otvorenie vášho editora, či už to je Visual Studio, Jetbrains " +
                        "Rider, alebo obyčajný textový editor. V ňom otvorte projekt a cez ten triedu " +
                        "EconomyTest, nachádzajúcu sa v adresári na ceste <b>Assets/BotScripts</b>.\n" +
                        "\n" +
                        "Následne do existujúcej metódy napíšte:\n" +
                        "<b>XposeCraft.Game.BotRunner.Tutorial = true;</b>\n" +
                        "a uložte zmeny. Potom sa vrátte naspäť a počkajte na zostavenie projektu " +
                        "(ikona sa v Unity roztočí vpravo dole), alebo hru reštartujte tlačidlom hrať.\n" +
                        "\n" +
                        "Ak chcete návod vypnúť, nastavte naopak hodnotu na false.\n" +
                        "Všetko, k čomu máte prístup, je v mennom priestore <b>XposeCraft.Game</b>.";
                    return;

                case TutorialStep.FindingWorker:
                    _text =
                        "Druhou úlohou je nájdenie vašej základnej budovy a pracovníkov " +
                        "(ktorých zatiaľ dokážete ovládať iba myšou) prostredníctvom kódu.\n" +
                        "\n" +
                        "K nájdeniu aktérov sa používajú pomocné triedy, nazývané s príponou Helper. " +
                        "Nájdite všetky budovy alebo jednotky použitím statickej metódy " +
                        "jednej z týchto tried, ktoré vracajú polia alebo listy. Počet prvkov je " +
                        "následne možné zistiť pomocou vlastnosti <b>.Length</b> alebo <b>.Count</b>.\n" +
                        "\n" +
                        "Všimnete si, že je potrebný generický typ metódy! Volanie teda vyzerá ako " +
                        "<b>GetMyUnits<IUnit>()</b>, kde IUnit hovorí, ktorý typ hľadáte.\n" +
                        "\n" +
                        "Ak nenájdete menný priestor týchto tried, sú pod <b>XposeCraft.Game.Helpers</b>.";
                    break;

                case TutorialStep.Gathering:
                    _text =
                        "Už viete nájsť aj vaše jednotky, aj budovy. Úlohou je poslať pracovníkov " +
                        "ťažiť suroviny. Trieda pracovníka sa nazýva <b>Worker</b>, k jeho metóde sa teda " +
                        "nedostanete, kým jednotku správne nepretypujete. " +
                        "Premennú tak môžete uložiť použitím výrazu <b>var worker = unit as Worker</b>;\n" +
                        "\n" +
                        "Musíte ale nájsť vhodnú <b>Helper</b> triedu aj na nájdenie surovín. V dokumentácii " +
                        "nájdete správnych viacero možností.\n" +
                        "\n" +
                        "K obsahu listov sa v jazyku C# pristupuje pomocou indexu, teda list[0] je " +
                        "prvý prvok premennej s názvom list.";
                    break;

                case TutorialStep.Events:
                    _text =
                        "Super, už dokážete zvyšovať množstvo vašich surovín. Teraz môžete začať produkovať " +
                        "nových pracovníkov pre ich rýchlejšie zbieranie.\n" +
                        "\n" +
                        "Hneď po spustení hry ale ešte nemáte dosť surovín. Je preto dôležité najprv pochopiť, " +
                        "ako v hre fungujú <b>udalosti</b>. Pomocou statickej metódy\n" +
                        "<b>XposeCraft.Game.GameEvent.Register</b>\n" +
                        "si vyskúšajte zaregistrovať vhodnú udalosť. Jej typ nájdete v enumeračnom type " +
                        "<b>XposeCraft.Game.Enums.EventType</b>, pomocou ktorého si vyberiete " +
                        "<b>MineralsChanged</b>. Už zostáva iba vytvorenie anonymnej funkcie. V jazyku C# ju " +
                        "môžete zapísať nasledovne: <b>(args) => {\n}</b>";
                    break;

                case TutorialStep.Production:
                    _text =
                        "Udalosť zaregistrovanú máte, takže do nej teraz vytvorte podmienku použitím " +
                        "vstupnej premennej, ktorú ste mohli nazvať napríklad args. Keď máte 50 minerálov, " +
                        "vo vašej základnej budove <b>BaseCenter</b> za ne môžete produkovať jednotky.\n" +
                        "\n" +
                        "Vytvorte niekoľko nových pracovníkov. Pomôže vám k tomu enumeračný typ <b>UnitType</b>. " +
                        "Udalosť môžete nálsedne skúsiť odregistrovať. Nedostanete sa k nej pomocou slovíčka " +
                        "this, ale musíte pohľadať, čo vám poskytuje typ vstupnej premennej <b>Arguments</b>.\n" +
                        "\n" +
                        "Produkcia prebieha asynchrónne, takže nová jednotka nevznikne hneď. Preto budete " +
                        "musieť použiť ďalšiu udalosť typu <b>UnitProduced</b>, ak sa k nej chcete dostať.";
                    break;

                case TutorialStep.Building:
                    _text =
                        "Ďalšiou fázou je stavba nových budov. Budova typu <b>NubianArmory</b> dokáže produkovať " +
                        "nové jednotky schopné útočiť na diaľku. Táto budova už stojí 100 minerálov.\n" +
                        "\n" +
                        "Pre lepšiu prehladnosť kódu tentokrát odporúčam písať do triedy BuildingTest.cs, " +
                        "ktorá má metódu BuildingStage. Spúšťa ju vaša akcia startNextStage(). " +
                        "Pripomínam, že budovy dokážu stavať iba pracovníci, ktorí sú typu <b>Worker</b>. " +
                        "Vhodné pozície na stavbu budov nájdete v triede <b>PlaceType</b>, alebo aj cez Helper.\n" +
                        "\n" +
                        "Vytvorte aspoň jednu takúto budovu. Predchádzajúcu udalosť asi budete musieť upraviť, " +
                        "aby vám všetky minerály nebrala produkcia pracovníkov. Viacej pracovníkov ju stavia " +
                        "rýchlejšie, ak použijú metódu FinishBuilding.";
                    break;

                case TutorialStep.HotSwap:
                    _text =
                        "Pomaly sa blížite ku koncu návodu. Vedlajšou vlastnosťou hry, ktorú pochopíte, je " +
                        "<b>výmena kódu za behu hry</b>. Využívali ste ju zatiaľ celú dobu, keď ste uložili " +
                        "zmeny pred návratom do Unity, ale môže byť hodné aj vypnúť ju, nech hra pokračuje " +
                        "bez viacnásobného spúšťania vášho kódu. Bez nej budete hru reštartovať obyčajne v Unity.\n" +
                        "\n" +
                        "Vypnúť (alebo cez true zapnúť) ju môžete cez:\n" +
                        "<b>XposeCraft.Game.BotRunner.HotSwap = false</b>";
                    break;

                case TutorialStep.ActionQueue:
                    _text =
                        "Poslednou možnosťou, ktorú si ukážeme, je <b>fronta akcií</b>. Všetky jednotky " +
                        "majú frontu, ktorá vykonáva akcie v poradí, v akom ste ich pridali. " +
                        "Použitím metód ako Worker.SendGather alebo Unit.Attack sa celá fronta nahradí " +
                        "novou o veľkosti jednej akcie. Vlastnosť jednotiek <b>ActionQueue</b> môžete využiť " +
                        "aj priamo, vytvorením novej fronty ako <b>new UnitActionQueue()</b>.\n" +
                        "\n" +
                        "Do fronty pridávate akcie metódou <b>After</b>, ktorá frontu vracia naspäť. Môžete teda " +
                        "zapísať aj .After(akcia).After(akcia).After(akcia). Vytvorte frontu o dĺžke aspoň 2.\n" +
                        "\n" +
                        "Všetky dostupné akcie vytvárate pomocou konštruktora kľúčovým slovo <b>new</b> " +
                        "a nachádzajú sa v mennom priestore <b>XposeCraft.Game.Control.GameActions</b>.";
                    break;

                default:
                    _text =
                        "Gratulujem, dokončili ste celý návod! Teraz môžete začať s vývojom vlastného robota, " +
                        "určeného na súboj proti ďalším hráčom v Aréne. Hľadajte nové stratégie a staňte sa " +
                        "najlepším hráčom. Veľa štastia! Zdrojový kód hry je voľne dostupný, takže sa môžete " +
                        "zapojiť aj do rozvíjania samotnej hry.\n" +
                        "\n" +
                        "Nezabudnite si vypnúť tento návod pomocou príkazu:\n" +
                        "<b>XposeCraft.Game.BotRunner.Tutorial = false;</b>";
                    break;
            }
            // If not any of first three states, proceed to the max state
            _state = _maxState;
        }
    }
}
