using Microsoft.WindowsPhone.Imaging;
using DiscUtils;
using DiscUtils.Streams;
using DiscUtils.Vhdx;

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

            FullFlashUpdateImage ffuImage = new(ffuPath);
            using FileStream imageStream = ffuImage.GetImageStream();
            PayloadReader payloadReader = new(imageStream);
            if (payloadReader.Payloads.Count() != ffuImage.StoreCount)
            {
                throw new ImageStorageException("Store counts in metadata and store header do not match.");
            }

            for (int i = 0; i < ffuImage.StoreCount; i++)
            {
                FullFlashUpdateStore fullFlashUpdateStore = ffuImage.Stores[i];
                StorePayload storePayload = payloadReader.Payloads[i];

                string vhdfile = Path.Combine(outputDirectory, $"LUN{i}.vhdx");
                Console.WriteLine($"Dumping {vhdfile}...");

                long diskCapacity = (long)fullFlashUpdateStore.MinSectorCount * fullFlashUpdateStore.SectorSize;
                using Stream fs = new FileStream(vhdfile, FileMode.CreateNew, FileAccess.ReadWrite);
                using VirtualDisk outDisk = Disk.InitializeDynamic(fs, Ownership.None, diskCapacity, logicalSectorSize: (int)fullFlashUpdateStore.SectorSize);
                payloadReader.WriteToStream(outDisk.Content, storePayload, fullFlashUpdateStore.MinSectorCount, fullFlashUpdateStore.SectorSize);
            }
        }
    }
}