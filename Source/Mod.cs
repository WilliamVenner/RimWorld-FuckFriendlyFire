using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace FuckFriendlyFire
{
    public class Controller : Mod
    {
        public static Settings Settings;

        public Controller(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("venner.io.fuckfriendlyfire");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            base.GetSettings<Settings>();
        }

        public void Save()
        {
            LoadedModManager.GetMod<Controller>().GetSettings<Settings>().Write();
        }

        public override string SettingsCategory()
        {
            return "FuckFriendlyFire";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoSettingsWindowContents(inRect);
        }
    }
}
