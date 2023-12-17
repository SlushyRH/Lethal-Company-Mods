using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace SRH.Mods.LC
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class FreeMoonsPlugin : BaseUnityPlugin
    {
        private const string GUID = "SlushyRH.LethalCompany.FreeMoons";
        private const string NAME = "FreeMoons";
        private const string VERSION = "1.0.1";

        public static FreeMoonsPlugin Instance { get; private set; }
        public ManualLogSource Log { get; private set; }

        private readonly Harmony harmony = new Harmony(GUID);

        void Awake()
        {
            // create logging source
            Log = BepInEx.Logging.Logger.CreateLogSource(GUID);

            if (Instance == null)
                Instance = this;

            harmony.PatchAll(typeof(MoonPricePatch));
        }
    }

    [HarmonyPatch(typeof(Terminal))]
    internal class MoonPricePatch
    {
        private static Terminal terminal = null;
        private static int totalCostOfItems = -5;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        static void FindTerminal()
        {
            FreeMoonsPlugin.Instance.Log.LogInfo("Finding terminal object!");
            terminal = GameObject.FindObjectOfType<Terminal>();
            
            if (terminal == null)
                FreeMoonsPlugin.Instance.Log.LogError("Failed to find terminal object!");
            else
                FreeMoonsPlugin.Instance.Log.LogInfo("Found terminal object!");

        }

        [HarmonyPatch("LoadNewNode")]
        [HarmonyPrefix]
        static void LoadNewNodePatchBefore(ref TerminalNode node)
        {
            if (terminal == null && node == null)
                return;

            if (node.buyRerouteToMoon != -2)
                return;

            Traverse totalCostOfItemsRef = Traverse.Create(terminal).Field("totalCostOfItems");
            totalCostOfItems = (int)totalCostOfItemsRef.GetValue();
            totalCostOfItemsRef.SetValue(0);
        }

        [HarmonyPatch("LoadNewNode")]
        [HarmonyPostfix]
        static void LoadNewNodePatchAfter(ref TerminalNode node)
        {
            if (terminal == null && node == null)
                return;

            if (totalCostOfItems == -5)
                return;

            Traverse totalCostOfItemsRef = Traverse.Create(terminal).Field("totalCostOfItems");
            totalCostOfItemsRef.SetValue(0);

            totalCostOfItems = -5;
        }

        [HarmonyPatch("LoadNewNodeIfAffordable")]
        [HarmonyPrefix]
        static void LoadNewNodeIfAffordablePatch(ref TerminalNode node)
        {
            if (node == null)
                return;

            // if node is moon then set cost to 0
            if (node.buyRerouteToMoon != -1)
            {
                node.itemCost = 0;
            }
        }
    }
}
