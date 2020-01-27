using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Market.CustomersAndStaff.FunctionalTests.Infrastructure
{
    public static class ProcessExtensions
    {
        public static void MinimizeMainWindow(this Process process, TimeSpan timeout)
        {
            MinimizeMainWindow(() => process, timeout);
        }

        public static void MinimizeMainWindow(Func<int> getProcessId, TimeSpan timeout)
        {
            MinimizeMainWindow(() =>
                {
                    var processId = getProcessId();
                    if(processId == 0)
                        return null;
                    return Process.GetProcessById(processId);
                }, timeout);
        }

        private static void MinimizeMainWindow(Func<Process> getProcess, TimeSpan timeout)
        {
            var sw = Stopwatch.StartNew();
            var mainWindowHandle = IntPtr.Zero;
            while(true)
            {
                Thread.Sleep(1);
                var process = getProcess();
                if(sw.Elapsed > timeout)
                {
                    Console.WriteLine($"Warning! Could not minimize window for process {process?.Id}");
                    break;
                }

                if(process == null)
                    continue;

                if(process.MainWindowHandle != IntPtr.Zero)
                {
                    mainWindowHandle = process.MainWindowHandle;
                    break;
                }
            }

            const int SW_MINIMIZE = 6;
            if(mainWindowHandle != IntPtr.Zero)
            {
                ShowWindow(getProcess().MainWindowHandle, SW_MINIMIZE);
                var res = Marshal.GetLastWin32Error();
                if(res != 0)
                    Console.WriteLine($"Waring error while ShowWindow Win32ErrorCode: {res}");
            }
        }

        [DllImport("user32.dll")]
        [return : MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}