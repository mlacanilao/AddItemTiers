using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using NPOI.SS.Formula.Functions;
using UnityEngine;

namespace AddItemTiers
{
    internal static class ModInfo
    {
        internal const string Guid = "omegaplatinum.elin.additemtiers";
        internal const string Name = "Add Item Tiers";
        internal const string Version = "1.2.0";
    }

    [BepInPlugin(GUID: ModInfo.Guid, Name: ModInfo.Name, Version: ModInfo.Version)]
    internal class AddEquipmentTiers : BaseUnityPlugin
    {
        internal static AddEquipmentTiers Instance { get; private set; } = null!;

        private void Awake()
        {
            Instance = this;
            var harmony = new Harmony(id: ModInfo.Guid);
            harmony.PatchAll();
        }

        public static void Log(object payload)
        {
            Instance.Logger.LogInfo(data: payload);
        }
    }

    [HarmonyPatch(declaringType: typeof(Card), methodName: nameof(Card.Create))]
    internal static class CardPatch
    {
        [HarmonyPostfix]
        public static void CreatePostfix(Card __instance)
        {
            if (__instance == null ||
                EClass.core.IsGameStarted == false)
            {
                return;
            }

            if (__instance.IsEquipment == false ||
                __instance.sourceCard?._origin == "fish" ||
                __instance.sourceCard?.category == "currency" ||
                __instance.tier != 0)
            {
                return;
            }

            int luck = EClass.pc?.Evalue(ele: 78) ?? 0;

            if (luck <= 0)
            {
                return;
            }

            int tier = Mathf.Min(
                a: EClass.rnd(
                    a: EClass.rnd(
                        a: EClass.rnd(
                            a: EClass.curve(_a: luck, start: 100, step: 50, rate: 70) + 50
                        )
                    )
                ) / 50,
                b: 3
            );

            if (tier > 0)
            {
                __instance.SetTier(a: tier, setTraits: true);
            }
        }
    }
}