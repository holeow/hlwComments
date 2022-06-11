#undef DEBUG
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CommentsPlus
{
    class Logger
    {
        private static int current = 0;
        static Logger()
        {
#if DEBUG
            AllocConsole();
#endif
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public static void Clear()
        {
#if DEBUG
            Console.Clear();
#endif

        }

        public static void Log(string str)
        {
#if DEBUG
            Console.WriteLine($"[{current}]:{str}");
            current++;
#endif
        }

        public static void Log(object obj)
        {
#if DEBUG
            Console.WriteLine($"[{current}]:{obj}");
            current++;
#endif
        }
    }
}
