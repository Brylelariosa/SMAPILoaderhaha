using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SMAPIGameLoader.Game;

internal static class NativeLibManager
{
    static nint Load_libLZ4()
    {
        if (NativeLibrary.TryLoad("liblwjgl_lz4.so", out nint handle))
            return handle;

        string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        string directoryName = Path.GetDirectoryName(folderPath);
        string libname = Path.Combine(directoryName, "lib", "liblwjgl_lz4.so");
        NativeLibrary.TryLoad(libname, out handle);
        return handle;
    }

    public static void Loads()
    {
        try
        {
            Load_libLZ4();
            Console.WriteLine("done setup native libs");
        }
        catch (Exception ex)
        {
            ErrorDialogTool.Show(ex);
        }
    }
}
