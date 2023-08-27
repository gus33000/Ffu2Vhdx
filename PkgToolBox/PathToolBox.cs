using System;
using System.IO;
using System.Threading;

namespace Microsoft.Composition.ToolBox.IO
{
    public static class PathToolBox
    {
        public static string GetFullPath(string A_0)
        {
            return LongPathIO.UNCPath(A_0);
        }

        public static string Combine(string A_0, string A_1)
        {
            A_0 = A_0.TrimEnd(new char[]
            {
                '\\'
            });
            A_1 = A_1.TrimStart(new char[]
            {
                '\\'
            });
            return A_0 + "\\" + A_1;
        }

        public static string GetTemporaryPath()
        {
            return GetTempPath();
        }

        public static string GetTempPath()
        {
            DirectoryToolBox.Create(TemporaryDirectoryHandler.TempPathRoot);
            return Combine(TemporaryDirectoryHandler.TempPathRoot, Path.GetRandomFileName());
        }

        public static class TemporaryDirectoryHandler
        {
            internal static string TempPathRoot => Combine(TempPathPartialRoot, Thread.CurrentThread.ManagedThreadId.ToString());

            private static readonly Destructor Finalize = new();

            internal static string TempPathPartialRoot = Combine(Path.GetTempPath(), Path.GetRandomFileName());

            internal static bool enforcementEnabled = Environment.GetEnvironmentVariable("OFFICIAL_BUILD_MACHINE") == null;

            private sealed class Destructor
            {
                ~Destructor()
                {
                    if (enforcementEnabled && DirectoryToolBox.Exists(TempPathRoot))
                    {
                        throw new Exception("Users of temporary paths must call PathToolBox.CleanupTempDirectories() before exiting, this was leaked: " + TempPathRoot);
                    }
                }
            }
        }
    }
}
