using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;

namespace InfiniteSprint
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class InfiniteSprintPlugin : BaseUnityPlugin
    {
        private const string GUID = "SlushyRH.LethalCompany.InfiniteSprint";
        private const string NAME = "Infinite Sprint";
        private const string VERSION = "1.0.0";

        private readonly Harmony harmony = new Harmony(GUID);

        void Awake()
        {
            harmony.PatchAll(typeof(PlayerControllerBPatch));
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void InfiniteSprintPatch(ref float ___sprintMeter)
        {
            ___sprintMeter = 1f;
        }
    }
}
