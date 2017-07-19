using UnityEngine;
using Verse;

namespace FuckFriendlyFire
{
    public class Settings : Verse.ModSettings
    {
        public static int hit_chance   = 0;
        public static bool shooting_level_affects = false;

        protected static int simulate = 10;

        public static void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard list = new Listing_Standard();
            list.Begin(inRect);
            if (hit_chance == 0)
            {
                list.Label("Hit Chance: 0% (Disables friendly fire)", -1f);
            }
            else
            {
                list.Label("Hit Chance: " + hit_chance + "%", -1f);
            }
            hit_chance = (int)list.Slider(hit_chance, 0f, 99f);
            if (hit_chance > 0)
            {
                list.CheckboxLabeled("Affected by shooting level", ref shooting_level_affects, "When checked, the shooting level of a pawn will affect its friendly fire hit chance. Test below.");
                Log.Message(simulate.ToString());
                Log.Message(Calculations.CalculateHitChance(simulate).ToString());
                list.Label("Hit Chance After Shooter Level " + simulate + ": " + Calculations.CalculateHitChance(simulate) + "%");
                simulate = (int)list.Slider(simulate, 0f, 20f);
                list.End();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref hit_chance, "hit_chance", 0);
            Scribe_Values.Look(ref shooting_level_affects, "shooting_level_affects", false);
        }
    }
}