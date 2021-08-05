using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace VIEAT
{
    [StaticConstructorOnStartup]
    public static class HarmonyInit
    {
        public static Harmony harmonyInstance;

        public static readonly Color Green;

        public static readonly string LineTexPath = "UI/Overlays/ThingLine";

        public static readonly Material LineMatGreen;

        static HarmonyInit()
        {
            Green = new ColorInt(143, 171, 156).ToColor;
            Green = GenColor.FromHex("d09b61");
            LineMatGreen = MaterialPool.MatFrom(LineTexPath, ShaderDatabase.Transparent, Green);
            harmonyInstance = new Harmony("VIEAT.Mod");
            harmonyInstance.PatchAll();
            AccessTools.Field(typeof(Gizmo_PruningConfig), "StrengthTex").SetValue(null, SolidColorMaterials.NewSolidColorTexture(Green));
            AccessTools.Field(typeof(Gizmo_PruningConfig), "StrengthHighlightTex").SetValue(null, SolidColorMaterials.NewSolidColorTexture(new ColorInt(173, 208, 195).ToColor));
            AccessTools.Field(typeof(Gizmo_PruningConfig), "StrengthTargetTex").SetValue(null, SolidColorMaterials.NewSolidColorTexture(new ColorInt(102, 119, 102).ToColor));
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
}
