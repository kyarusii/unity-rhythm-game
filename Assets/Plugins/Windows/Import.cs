using System;
using System.Runtime.InteropServices;

namespace Native.Windows
{
    public static class Import
    {
        public static class Kernel32
        {
            [DllImport("Kernel32.dll")]
            public static extern IntPtr LoadLibrary(string lpLibFileName);

            [DllImport("Kernel32.dll")]
            public static extern int FreeLibrary(IntPtr hLibModule);

            [DllImport("Kernel32.dll")]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);    
        }
    }
}