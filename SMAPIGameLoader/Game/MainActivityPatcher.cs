using HarmonyLib;
using System;

namespace SMAPIGameLoader;

internal class MainActivityPatcher
{
    public static bool PrefixCheckStorageMigration(ref bool __result)
    {
        Console.WriteLine("bypass CheckStorageMigration");
        __result = false;
        return false;
    }

    internal static void Apply()
    {
        var harmony = new Harmony("SMAPIGameLoader");
        var prefixMethod = AccessTools.Method(
            typeof(MainActivityPatcher), nameof(MainActivityPatcher.PrefixCheckStorageMigration));

        var mainActivityType = GameAssemblyManager.LoadedStardewAssembly?
            .GetType("StardewValley.MainActivity");
        if (mainActivityType == null)
        {
            Console.WriteLine("MainActivityPatcher: MainActivity type not found, skipping patch");
            return;
        }
        var checkStorageMigration = AccessTools.Method(mainActivityType, "CheckStorageMigration");
        harmony.Patch(checkStorageMigration, prefix: prefixMethod);
        Console.WriteLine("Done MainActivityPatcher.Apply()");
    }
}
