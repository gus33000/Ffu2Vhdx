using Microsoft.WindowsPhone.Imaging;
using DiscUtils;
using DiscUtils.Streams;
using DiscUtils.Vhdx;
using System.Text;

namespace Ffu2Vhdx
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: Ffu2Vhdx <Path to FFU File> <Output director for LUNi.vhdx files>");
                return;
            }

            string FfuPath = args[0];
            string OutputDirectory = args[1];

            if (!File.Exists(FfuPath))
            {
                Console.WriteLine($"FfuFile does not exist: {FfuPath}");
                return;
            }

            if (!Directory.Exists(OutputDirectory))
            {
                Console.WriteLine($"Output directory does not exist: {OutputDirectory}");
                return;
            }

            ConvertFFU2VHD(FfuPath, OutputDirectory);
        }

        private static void ConvertFFU2VHD(string ffuPath, string outputDirectory)
        {
            DiscUtils.Setup.SetupHelper.RegisterAssembly(typeof(Disk).Assembly);

            using FileStream ffuStream = File.OpenRead(ffuPath);
            FfuFile ffuFile = new(ffuStream);

            for (int i = 0; i < ffuFile.StoreCount; i++)
            {
                string vhdfile = Path.Combine(outputDirectory, $"LUN{i}.vhdx");
                Console.WriteLine($"Dumping {vhdfile}...");

                long diskCapacity = (long)ffuFile.GetMinSectorCount(i) * ffuFile.GetSectorSize(i);
                using Stream fs = new FileStream(vhdfile, FileMode.CreateNew, FileAccess.ReadWrite);
                using VirtualDisk outDisk = Disk.InitializeDynamic(fs, Ownership.None, diskCapacity, logicalSectorSize: (int)ffuFile.GetSectorSize(i));

                ffuFile.WriteStoreToStream(i, outDisk.Content);
            }
        }
    }
}