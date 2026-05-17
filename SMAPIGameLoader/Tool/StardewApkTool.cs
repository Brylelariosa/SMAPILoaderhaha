using Android.App;
using Android.Content.PM;
using Android.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAPIGameLoader;

internal static class StardewApkTool
{
    public const string GamePlayStorePackageName = "com.chucklefish.stardewvalley";
    public const string GameGalaxyStorePackageName = "com.chucklefish.stardewvalleysamsung";
    static bool IsGameFromPlayStore = false;
    static bool IsGameFromGalaxyStore = false;
    static PackageInfo _currentPackageInfo;

    // Optional: set this to a custom APK path to bypass package detection entirely.
    // e.g. "/sdcard/stardew.apk" or any path the app has read access to.
    public static string ManualApkPath { get; set; } = null;

    //init at first SDK
    static StardewApkTool()
    {
        Console.WriteLine("Initialize Stardew Apk Tool");

        // If a manual path is set, skip package manager detection completely
        if (!string.IsNullOrEmpty(ManualApkPath))
        {
            Console.WriteLine("Game APK set manually: " + ManualApkPath);
            _manualApkPath = ManualApkPath;
            return;
        }

        var playStore = ApkTool.GetPackageInfo(GamePlayStorePackageName);
        var samsung = ApkTool.GetPackageInfo(GameGalaxyStorePackageName);

        //select samsung first, better for debug, test app
        if (samsung != null)
        {
            _currentPackageInfo = samsung;
            IsGameFromGalaxyStore = true;
            Console.WriteLine("Game Install From Galaxy Store");
        }
        else if (playStore != null)
        {
            _currentPackageInfo = playStore;
            IsGameFromPlayStore = true;
            Console.WriteLine("Game Install From Play Store");
        }
        else
        {
            // Fallback: accept any sideloaded/non-store installation with the base package name
            _currentPackageInfo = playStore ?? samsung;
            Console.WriteLine("Game Install From Unknown Source (sideloaded)");
        }
    }

    static string _manualApkPath = null;

    public static PackageInfo CurrentPackageInfo => _currentPackageInfo;

    public static bool IsInstalled
    {
        get
        {
            if (_manualApkPath != null) return true;
            return CurrentPackageInfo != null;
        }
    }

    public static Android.Content.Context GetContext => Application.Context;
    public static string? BaseApkPath => _manualApkPath ?? CurrentPackageInfo?.ApplicationInfo?.PublicSourceDir;
    public static string? Arm64ApkPath
    {
        get
        {
            try
            {
                if (_manualApkPath != null) return _manualApkPath;

                if (CurrentPackageInfo == null)
                    return null;

                if (IsGameFromPlayStore)
                {
                    var splitPath = CurrentPackageInfo.ApplicationInfo.SplitSourceDirs?.FirstOrDefault(path => path.Contains("split_config.arm64"));
                    if (splitPath != null)
                        return splitPath;
                }

                return BaseApkPath;
            }
            catch (Exception ex)
            {
                ErrorDialogTool.Show(ex, "Error try to get Arm64ApkPath");
                return null;
            }
        }
    }

    public static string? ContentApkPath
    {
        get
        {
            try
            {
                if (_manualApkPath != null) return _manualApkPath;

                if (CurrentPackageInfo == null)
                    return null;

                if (IsGameFromPlayStore)
                {
                    var splitPath = CurrentPackageInfo.ApplicationInfo.SplitSourceDirs?.FirstOrDefault(path => path.Contains("split_content"));
                    if (splitPath != null)
                        return splitPath;
                }

                return BaseApkPath;
            }
            catch (Exception ex)
            {
                ErrorDialogTool.Show(ex, "Error try to get ContentApkPath");
                return null;
            }
        }
    }

    public static Version GameVersionSupport
    {
        get
        {
            if (CurrentPackageInfo == null)
                return null;

            switch (CurrentPackageInfo.PackageName)
            {
                case GamePlayStorePackageName:
                    return new(1, 6, 15, 0);
                case GameGalaxyStorePackageName:
                    return new(1, 6, 14, 8);
                default:
                    // Sideloaded: use the Play Store minimum as a baseline
                    return new(1, 6, 15, 0);
            }
        }
    }
    public static Version CurrentGameVersion 
    {
        get
        {
            try
            {
                return new Version(CurrentPackageInfo?.VersionName);
            }
            catch(Exception ex)
            {
                return new Version(0,0,0,0);
            }
        }
    }
    public static bool IsGameVersionSupport => CurrentGameVersion >= GameVersionSupport;
}
