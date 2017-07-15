using Verse;
using Harmony;
using RimWorld;
using System;
using System.Reflection;

namespace NoFriendlyFire
{

    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            var harmony = HarmonyInstance.Create("venner.io.nofriendlyfire");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(Bullet), "Impact", new Type[] { typeof(Thing) })]
    public partial class NoFriendlyFireBullet_Impact
    {
        static void Prefix(Bullet __instance, ref Thing hitThing)
        {
            try
            {

                Type t = __instance.GetType().BaseType;

                Thing launcher       = (Thing)t.GetField("launcher",       BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                Thing assignedTarget = (Thing)t.GetField("assignedTarget", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

                if (hitThing.Faction.Equals(launcher.Faction) && assignedTarget != hitThing)
                    hitThing = null;

            } catch (NullReferenceException) { }
        }
    }
}