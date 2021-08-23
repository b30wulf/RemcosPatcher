using System;
using System.IO;

namespace RemcosPatcher
{
    class Program
    {
        static byte[] ConsoleCall = { 0xE8, 0xE5, 0x9B, 0x00, 0x00 };
        static byte[] TrayIcon = { 0x57, 0x57, 0x57, 0x68, 0xF9, 0x2B, 0x41, 0x00, 0x57, 0x57, 0xFF, 0xD6 };

        // https://stackoverflow.com/a/38625726/3658854
        static int Search(byte[] src, byte[] pattern)
        {
            int maxFirstCharSlot = src.Length - pattern.Length + 1;
            for (int i = 0; i < maxFirstCharSlot; i++)
            {
                if (src[i] != pattern[0]) // compare only first byte
                    continue;

                // found a match on first byte, now try to match rest of the pattern
                for (int j = pattern.Length - 1; j >= 1; j--)
                {
                    if (src[i + j] != pattern[j]) break;
                    if (j == 1) return i;
                }
            }
            return -1;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Remcos RAT Patcher for version 3.2.0\n");

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: drag and drop the Remcos Light executable onto this one, or as a command line argument");
                return;
            }

            Console.WriteLine($"Opening { args[0] }\n");
            byte[] bytes = File.ReadAllBytes(args[0]);

            int index = Search(bytes, ConsoleCall);

            if(index != -1)
                Console.WriteLine("Found call to console window\n");

            for (int i = 0; i < ConsoleCall.Length; i++)
            {
                bytes[index + i] = 0x90;
            }

            Console.WriteLine("Patched call to console window\n");


            index = Search(bytes, TrayIcon);
            if (index != -1)
                Console.WriteLine("Found call to tray icon\n");

            for (int i = 0; i < TrayIcon.Length; i++)
            {
                bytes[index + i] = 0x90;
            }

            Console.WriteLine("Patched call to tray icon\n");

            File.WriteAllBytes(args[0].Substring(0, args[0].Length - 4) + "_modified.exe", bytes);

            Console.WriteLine($"Done! Wrote to { args[0].Substring(0, args[0].Length - 4) + "_modified.exe" }\n");

            Console.ReadKey();
        }
    }
}
