using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace SRH.Mods.LC
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class FreeMoonsPlugin
    {
        private const string GUID = "SlushyRH.LethalCompany.FreeMoons";
        private const string NAME = "FreeMoons";
        private const string VERSION = "1.0.1";

        private readonly Harmony harmony = new Harmony(GUID);

        void Awake()
        {
            harmony.PatchAll(typeof(MoonPricePatch));
        }
    }

    [HarmonyPatch(typeof(Terminal))]
    internal class MoonPricePatch
    {
        private static Terminal? terminal;
        private static int totalCostOfItems = -5;

        [HarmonyPatch(("Awake"))]
        [HarmonyPostfix]
        static void FindTerminal()
        {
            terminal = GameObject.FindObjectOfType<Terminal>();
        }

        [HarmonyPatch("LoadNewNode")]
        [HarmonyPrefix]
        static void LoadNewNodePatchBefore(ref TerminalNode node)
        {
            if (terminal == null && node == null && node.buyRerouteToMoon != -2)
                return;

            // set the visual value of the moon to 0
            Traverse totalCostOfItemsRef = Traverse.Create(terminal).Field("totalCostOfItems");
            totalCostOfItems = (int)totalCostOfItemsRef.GetValue();
            totalCostOfItemsRef.SetValue(0);
        }

        [HarmonyPatch("LoadNewNode")]
        [HarmonyPostfix]
        static void LoadNewNodePatchAfter(ref TerminalNode node)
        {
            if (terminal == null && node == null && node.buyRerouteToMoon != -5)
                return;

            // reset the visual value
            Traverse totalCostOfItemsRef = Traverse.Create(terminal).Field("totalCostOfItems");
            totalCostOfItemsRef.SetValue(totalCostOfItems);

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
                node.itemCost = 0;
        }
    }
}
