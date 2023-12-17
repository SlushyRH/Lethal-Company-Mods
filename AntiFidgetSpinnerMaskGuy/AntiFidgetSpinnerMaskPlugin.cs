using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using UnityEngine;

namespace SRH.Mods.LC
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class AntiFidgetSpinnerMaskPlugin : BaseUnityPlugin
    {
        private const string GUID = "SlushyRH.LethalCompany.AntiFidgetSpinnerMask";
        private const string NAME = "AntiFidgetSpinnerMask";
        private const string VERSION = "1.0.0";

        public static AntiFidgetSpinnerMaskPlugin Instance { get; private set; }

        private readonly Harmony harmony = new Harmony(GUID);
        public ManualLogSource log;

        void Awake()
        {
            // create logging source
            log = BepInEx.Logging.Logger.CreateLogSource(GUID);

            if (Instance == null)
                Instance = this;

            harmony.PatchAll(typeof(MaskEnemyPatch));
        }
    }

    [HarmonyPatch(typeof(MaskedPlayerEnemy))]
    internal class MaskEnemyPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        static void FineMaskedEnemy()
        {
            AntiFidgetSpinnerMaskPlugin.Instance.log.LogInfo("New Masked Enemy Spawned In");
        }

        [HarmonyPatch("LookAndRunRandomly")]
        [HarmonyPostfix]
        static void LookAndRunRandomlyPatch(ref float randomLookTimer)
        {
            randomLookTimer = 1f;
        }
    }
}
