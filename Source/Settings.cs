using UnityEngine;
using Verse;
using System;

namespace FuckFriendlyFire
{
    public class Settings : ModSettings
    {
        public class HitChance
        {
            public static float BaseHitChance = 0;
            public static float TurretHitChance = 0;
            public static bool ScaleWithShootingLevel = false;
        }

        protected static int simulate = 10;
        private static void DrawBaseHitChance(ref Listing_Standard list)
        {
            if (HitChance.BaseHitChance == 0f)
                list.Label("Hit Chance: 0% (Disables friendly fire)", -1f);
            else
                list.Label("Hit Chance: " + Math.Round(HitChance.BaseHitChance * 100f, 2) + "%", -1f);

            HitChance.BaseHitChance = (float)list.Slider(HitChance.BaseHitChance, 0f, 1f);

            if (HitChance.BaseHitChance > 0f)
            {
                list.CheckboxLabeled("Affected by shooting level (test below)", ref HitChance.ScaleWithShootingLevel, "When checked, the shooting level of a pawn will affect its friendly fire hit chance. Test below.");
                list.Label("Hit Chance After Shooter Level " + simulate + ": " + Math.Round(FuckFriendlyFire.HitChance.Calculate(simulate) * 100f, 2) + "%");
                simulate = (int)list.Slider(simulate, 0, 20);
            }
        }

        private static void DrawTurretHitChance(ref Listing_Standard list)
        {
            if (HitChance.TurretHitChance == 0f)
                list.Label("Turret Hit Chance: 0% (Disables turret friendly fire)", -1f);
            else
                list.Label("Turret Hit Chance: " + Math.Round(HitChance.TurretHitChance * 100f, 2) + "%", -1f);

            HitChance.TurretHitChance = (float)list.Slider(HitChance.TurretHitChance, 0f, 1f);
        }

        public static void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard list = new Listing_Standard();
            list.Begin(inRect);

            DrawTurretHitChance(ref list);
            DrawBaseHitChance(ref list);

            list.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref HitChance.BaseHitChance, "BaseHitChance", 0);
            Scribe_Values.Look(ref HitChance.TurretHitChance, "TurretHitChance", 0);
            Scribe_Values.Look(ref HitChance.ScaleWithShootingLevel, "ScaleWithShootingLevel", false);
        }
    }
}
