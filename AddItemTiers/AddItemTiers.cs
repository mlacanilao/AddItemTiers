using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace AddItemTiers
{
    internal static class ModInfo
    {
        internal const string Guid = "omegaplatinum.elin.additemtiers";
        internal const string Name = "Add Item Tiers";
        internal const string Version = "1.1.0.0";
    }

    [BepInPlugin(GUID: ModInfo.Guid, Name: ModInfo.Name, Version: ModInfo.Version)]
    internal class AddItemTiers : BaseUnityPlugin
    {
        internal static AddItemTiers Instance { get; private set; }

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

    [HarmonyPatch(declaringType: typeof(ThingGen), methodName: nameof(ThingGen._Create))]
    internal static class ThingGenPatch
    {
        [HarmonyPostfix]
        public static void _CreatePostfix(Thing __result)
        {
            if (__result == null ||
                EClass.core.IsGameStarted == false)
            {
                return;
            }
            
            if (__result.source?._origin == "fish" ||
                __result.source?.category == "currency" ||
                __result.tier != 0)
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
                            a: EClass.curve(a: luck, start: 100, step: 50, rate: 70) + 50
                        )
                    )
                ) / 50,
                b: 3
            );
            
            if (tier > 0)
            {
                __result.SetTier(a: tier, setTraits: true);
            }
        }
    }
}