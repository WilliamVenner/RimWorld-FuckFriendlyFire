using Verse;
using Harmony;
using RimWorld;
using System;
using System.Reflection;

namespace FuckFriendlyFire
{
    class Calculations
    {
        public static float CalculateHitChance(int shooterLevel)
        {
            return (Settings.hit_chance - shooterLevel * Settings.hit_chance / 20f);
        }
        public static float CalculateHitChance(Pawn p)
        {
            int shooterLevel = p.skills.GetSkill(SkillDefOf.Shooting).Level;
            return CalculateHitChance(shooterLevel);
        }
    }

    [HarmonyPatch(typeof(Bullet), "Impact", new Type[] { typeof(Thing) })]
    public static class FuckFriendlyFireBullet_Impact
    {
        static void Prefix(Bullet __instance, ref Thing hitThing)
        {
            if (hitThing.GetType() != typeof(Pawn)) { return;  }
            try
            {
                Type t = __instance.GetType().BaseType;

                Thing launcher = (Thing)t.GetField("launcher", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                Thing assignedTarget = (Thing)t.GetField("assignedTarget", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

                if (launcher.HostileTo(hitThing.Faction) || assignedTarget == hitThing) { return; }

                if (Settings.hit_chance == 0) { hitThing = null; return; }
                
                try
                {

                    decimal chance = (decimal)Calculations.CalculateHitChance((Pawn)launcher);
                    int count = BitConverter.GetBytes(decimal.GetBits(chance)[3])[2];
                    int r = Rand.Range(1, (int)Math.Pow(10,(2 + count)));

                    if (r > ((int)chance * (int)Math.Pow(10, count))) hitThing = null;

                } catch(InvalidCastException) {}
            }
            catch (NullReferenceException) {}
        }
    }
}