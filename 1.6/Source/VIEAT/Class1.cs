using HarmonyLib;
using Ionic.Zlib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using LudeonTK;

namespace VIEAT
{
    [DefOf]
    public static class VIEAT_DefOf
    {
        public static PreceptDef TreeConnection;
    }

    [StaticConstructorOnStartup]
    public static class HarmonyInit
    {
        public static Harmony harmonyInstance;

        public static readonly Color Green = new ColorInt(143, 171, 156, 255).ToColor;

        public static readonly string LineTexPath = "UI/Overlays/ThingLine";

        public static readonly Material LineMatGreen;

        static HarmonyInit()
        {
            LineMatGreen = MaterialPool.MatFrom(LineTexPath, ShaderDatabase.Transparent, Green);
            harmonyInstance = new Harmony("VIEAT.Mod");
            harmonyInstance.PatchAll();
            RecolorTexture("StrengthTex", Green);
            RecolorTexture("StrengthHighlightTex", new ColorInt(173, 208, 195).ToColor);
            RecolorTexture("StrengthTargetTex", new ColorInt(102, 119, 102).ToColor);
        }

        private static void RecolorTexture(string fieldName, Color color)
        {
            // Patched draw methods may retain the original readonly texture reference.
            var texture = (Texture2D)AccessTools.Field(typeof(Gizmo_PruningConfig), fieldName).GetValue(null);
            for (int x = 0; x < texture.width; x++)
                for (int y = 0; y < texture.height; y++)
                    texture.SetPixel(x, y, color);
            texture.Apply();
        }
    }

    [HarmonyPatch(typeof(Pawn_ConnectionsTracker), "DrawConnectionLine")]
    public static class DrawConnectionLine_Patch
    {
        public static bool Prefix(Pawn ___pawn, Thing t)
        {
            if (t.Spawned && t.Map == ___pawn.Map)
            {
                GenDraw.DrawLineBetween(___pawn.TrueCenter(), t.TrueCenter(), HarmonyInit.LineMatGreen, 0.2f);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(Command_Ritual), MethodType.Constructor, new Type[] {
        typeof(Precept_Ritual), typeof(TargetInfo), typeof(RitualObligation), typeof(Dictionary<string, Pawn>)})]
    public static class Command_Ritual_Patch
    {
        public static void Postfix(Command_Ritual __instance)
        {
            if (__instance.ritual.def == VIEAT_DefOf.TreeConnection && !__instance.disabled)
            {
                __instance.defaultIconColor = HarmonyInit.Green;
            }
        }
    }
}
