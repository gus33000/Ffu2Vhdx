using DiscUtils;
using DiscUtils.Streams;
using DiscUtils.Vhdx;
using FfuStream;

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

                using Stream store = ffuFile.OpenStore(i);

                StreamPump pump = new()
                {
                    InputStream = store,
                    OutputStream = outDisk.Content,
                    SparseCopy = true,
                    SparseChunkSize = (int)ffuFile.GetSectorSize(i),
                    BufferSize = (int)ffuFile.GetSectorSize(i) * 1024
                };

                long totalBytes = store.Length;

                DateTime now = DateTime.Now;
                pump.ProgressEvent += (o, e) => { ShowProgress((ulong)e.BytesRead, (ulong)totalBytes, now); };

                Console.WriteLine($"Dumping Store {i}");
                pump.Run();
                Console.WriteLine();

                store.Dispose();
            }
        }

        protected static void ShowProgress(ulong readBytes, ulong totalBytes, DateTime startTime)
        {
            DateTime now = DateTime.Now;
            TimeSpan timeSoFar = now - startTime;

            TimeSpan remaining =
                TimeSpan.FromMilliseconds(timeSoFar.TotalMilliseconds / readBytes * (totalBytes - readBytes));

            double speed = Math.Round(readBytes / 1024L / 1024L / timeSoFar.TotalSeconds);

            Console.Write(
                $"\r{GetDismLikeProgBar((int)(readBytes * 100 / totalBytes))} {speed}MB/s {remaining:hh\\:mm\\:ss\\.f}");
        }

        private static string GetDismLikeProgBar(int percentage)
        {
            int eqsLength = (int)((double)percentage / 100 * 55);
            string bases = new string('=', eqsLength) + new string(' ', 55 - eqsLength);
            bases = bases.Insert(28, percentage + "%");
            if (percentage == 100)
            {
                bases = bases[1..];
            }
            else if (percentage < 10)
            {
                bases = bases.Insert(28, " ");
            }

            return $"[{bases}]";
        }
    }
}