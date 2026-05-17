using HarmonyLib;
using System;

namespace SMAPIGameLoader;

[Harmony]
internal class Log
{
    public static void It(string message)
    {
        Console.WriteLine(message);
    }
    public static void Setup()
    {
        var harmony = new Harmony("SMAPIGameLoader");
        var stardewAsm = GameAssemblyManager.LoadedStardewAssembly;
        var DefaultLogger = stardewAsm?.GetType("StardewValley.Logging.DefaultLogger");
        if (DefaultLogger == null)
        {
            Console.WriteLine("Log.Setup: DefaultLogger type not found, skipping patch");
            return;
        }
        var LogImpl = AccessTools.Method(DefaultLogger, "LogImpl");
        harmony.Patch(LogImpl, prefix: AccessTools.Method(typeof(Log), nameof(PrefixLogImpl)));
    }
    static void PrefixLogImpl(string level, string message, Exception exception = null)
    {
        Console.WriteLine($"LogImpl(level: {level}, msg: {message})");
    }
}
