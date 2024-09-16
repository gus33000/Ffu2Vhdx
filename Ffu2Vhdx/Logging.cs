/*
 * Copyright (c) The LumiaWOA and DuoWOA authors
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
namespace Ffu2Vhdx
{
    internal static class Logging
    {
        private static readonly object lockObj = new();

        private static readonly ConsoleColor originalConsoleColor;

        static Logging()
        {
            originalConsoleColor = Console.ForegroundColor;
        }

        public static void ShowProgress(long CurrentProgress,
                                        long TotalProgress,
                                        DateTime startTime,
                                        bool DisplayRed)
        {
            uint ProgressPercentage = TotalProgress == 0 ? 100 : (uint)(CurrentProgress * 100 / TotalProgress);

            DateTime now = DateTime.Now;
            TimeSpan timeSoFar = now - startTime;

            TimeSpan remaining = new(0);

            double milliSecondsRemaining;
            if ((TotalProgress - CurrentProgress) == 0)
            {
                milliSecondsRemaining = 0;
            }
            else
            {
                milliSecondsRemaining = (double)(timeSoFar.TotalMilliseconds / CurrentProgress * (TotalProgress - CurrentProgress));
            }

            try
            {
                remaining = TimeSpan.FromMilliseconds(milliSecondsRemaining);
            }
            catch { }

            LoggingLevel level;

            if (DisplayRed)
            {
                level = LoggingLevel.Warning;
            }
            else
            {
                level = LoggingLevel.Information;
            }

            Log($"{GetDISMLikeProgressBar(ProgressPercentage)} {remaining:hh\\:mm\\:ss\\.f}", severity: level, returnLine: false);
        }

        public static string GetDISMLikeProgressBar(uint percentage)
        {
            if (percentage > 100)
            {
                percentage = 100;
            }

            int eqsLength = (int)Math.Floor((double)percentage * 55u / 100u);

            string bases = $"{new string('=', eqsLength)}{new string(' ', 55 - eqsLength)}";

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

        public static void Log(string message = "", LoggingLevel severity = LoggingLevel.Information, bool returnLine = true)
        {
            lock (lockObj)
            {
                if (message?.Length == 0)
                {
                    Console.WriteLine();
                    return;
                }

                string msg = "";

                switch (severity)
                {
                    case LoggingLevel.Warning:
                        msg = "  Warning  ";
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LoggingLevel.Error:
                        msg = "   Error   ";
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case LoggingLevel.Information:
                        msg = "Information";
                        Console.ForegroundColor = originalConsoleColor;
                        break;
                }

                bool returnToLineDueToMessage = false;

                if (message?.StartsWith('\r') == true)
                {
                    message = message[1..];
                    returnToLineDueToMessage = true;
                }

                if (returnLine)
                {
                    if (returnToLineDueToMessage)
                    {
                        Console.WriteLine($"\r{DateTime.Now:'['HH':'mm':'ss']'}[{msg}] {message}");
                    }
                    else
                    {
                        Console.WriteLine($"{DateTime.Now:'['HH':'mm':'ss']'}[{msg}] {message}");
                    }
                }
                else
                {
                    Console.Write($"\r{DateTime.Now:'['HH':'mm':'ss']'}[{msg}] {message}");
                }

                Console.ForegroundColor = originalConsoleColor;
            }
        }
    }
}