﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Deucalion.IP_Switcher.WMI
{
    class FormatMessage
    {
        public const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;
        public const uint FORMAT_MESSAGE_FROM_HMODULE = 0x00000800;
        public const uint FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
        public const uint FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
        public const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr LoadLibraryEx(
            [MarshalAs(UnmanagedType.LPTStr)] string lpFileName, IntPtr hFile, uint dwFlags);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int FormatMessageW(
            uint dwFormatFlags, IntPtr lpSource, int dwMessageId,
            int dwLanguageId, out IntPtr MsgBuffer, int nSize, IntPtr Arguments);

        public static string GetMessage(int id)
        {
            IntPtr hModule = IntPtr.Zero;
            IntPtr pMessageBuffer;
            int dwBufferLength;
            string sMsg = "";
            uint dwFormatFlags = FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS;

            string dllFile = @"C:\Windows\System32\wbem\wmiutils.dll";
            hModule = LoadLibraryEx(dllFile, // dll or exe file
                                    IntPtr.Zero, // null for future use
                                    LOAD_LIBRARY_AS_DATAFILE); // only to extract messages

            if (IntPtr.Zero != hModule)
            {
                dwFormatFlags |= FORMAT_MESSAGE_FROM_HMODULE;
                Console.WriteLine("\n > Found hmodule for: " + dllFile);
            }

            dwBufferLength = FormatMessageW(dwFormatFlags, // formatting options
                                                hModule,    // dll file message
                                                id,         // Message identifier
                                                0,          // Language identifier
                                                out pMessageBuffer, // Pointer to a buffer
                                                0,          // Minimum number of chars to write in pMessageBuffer
                                                IntPtr.Zero); //Pointer to an array of insertion strings
            if (0 != dwBufferLength)
            {
                sMsg = Marshal.PtrToStringUni(pMessageBuffer);
                Marshal.FreeHGlobal(pMessageBuffer);
            }
            return sMsg;
        }
    }
}
