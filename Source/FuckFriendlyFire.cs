using Verse;
using HarmonyLib;
using RimWorld;
using System.Reflection;

namespace FuckFriendlyFire
{
    public class FuckFriendlyFire
    {
        const byte MaxSkillLevel = 20;

        public class HitChance
        {
            public static float Calculate(int shooterLevel)
            {
                return Settings.HitChance.BaseHitChance * (1f - (shooterLevel / (float)MaxSkillLevel));
            }

            public static float Calculate(Pawn p)
            {
                int shooterLevel = Settings.HitChance.ScaleWithShootingLevel ? p.skills.GetSkill(SkillDefOf.Shooting).Level : MaxSkillLevel;
                return Calculate(shooterLevel);
            }

            public static float Generate()
            {
                return Rand.Range(0, 10000) / 10000f;
            }

            public static bool Test(float chance)
            {
                float rng = Generate();
                return rng >= chance;
            }

            public static bool Test(Pawn pawn)
            {
                return Test(Calculate(pawn));
            }
        }

        [HarmonyPatch(typeof(Projectile))]
        [HarmonyPatch("CanHit")]
        class ProjectileCanHit
        {
            static readonly FieldInfo LauncherField = typeof(Projectile).GetField("launcher", BindingFlags.NonPublic | BindingFlags.Instance);
            static readonly FieldInfo IntendedTargetField = typeof(Projectile).GetField("intendedTarget", BindingFlags.Public | BindingFlags.Instance);

            protected static void Postfix(Projectile __instance, ref Thing thing, ref bool __result)
            {
                if (__result == false) return;

                if (thing != null && !(thing is Pawn)) return;

                Thing launcher = (Thing)LauncherField.GetValue(__instance);

                // If nobody launched this, do nothing
                if (launcher == null) return;

                if (thing != null)
                {
                    LocalTargetInfo intendedTargetInfo = (LocalTargetInfo)IntendedTargetField.GetValue(__instance);

                    // If they're specifically targeted, do nothing
                    if (intendedTargetInfo != null && intendedTargetInfo.Thing == thing) return;

                    // If the launcher of the bullet is hostile to us, do nothing
                    if (launcher.HostileTo(thing.Faction)) return;
                }

                // If this is a friendly turret...
                if (launcher is Building_TurretGun)
                {
                    if (thing != null && !launcher.HostileTo(thing.Faction))
                    {
                        if (Settings.HitChance.TurretHitChance != 100 && (Settings.HitChance.TurretHitChance == 0 || HitChance.Test(Settings.HitChance.TurretHitChance)))
                        {
                            __result = false;
                        }
                    }
                    return;
                }

                if (launcher is Pawn pawn && Settings.HitChance.BaseHitChance != 100 && (Settings.HitChance.BaseHitChance == 0 || HitChance.Test(pawn)))
                {
                    __result = false;
                }
            }
        }
    }
}
