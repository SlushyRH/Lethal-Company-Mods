using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
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
        public ManualLogSource Log { get; private set; }

        private readonly Harmony harmony = new Harmony(GUID);

        void Awake()
        {
            // create logging source
            Log = BepInEx.Logging.Logger.CreateLogSource(GUID);

            if (Instance == null)
                Instance = this;

            harmony.PatchAll(typeof(MaskEnemyPatch));
        }
    }

    [HarmonyPatch(typeof(MaskedPlayerEnemy))]
    internal class MaskEnemyPatch
    {
        private static MaskedPlayerEnemy[] allMaskedEnemies;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        void GetMaskedEnemy()
        {
            AntiFidgetSpinnerMaskPlugin.Instance.Log.LogInfo("New Masked Enemy Spawned In!");
            allMaskedEnemies = GameObject.FindObjectsOfType<MaskedPlayerEnemy>();

            if (allMaskedEnemies.Length == 0)
                AntiFidgetSpinnerMaskPlugin.Instance.Log.LogError("Failed to get any masked enemy references!");
            else
                AntiFidgetSpinnerMaskPlugin.Instance.Log.LogInfo("Collected all masked enemies!");
        }

        [HarmonyPatch("LookAndRunRandomly")]
        [HarmonyPostfix]
        static void LookAndRunRandomlyPatch()
        {
            foreach (MaskedPlayerEnemy enemy in allMaskedEnemies)
                Traverse.Create(enemy).Field("randomLookTimer").SetValue(1f);
        }
    }
}
