using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading.Tasks;

namespace cake
{
    public static class ProcessExtensions
    {
        public static async Task KillAsync(this Process process)
        {
            try
            {
                Console.WriteLine($"Trying to kill process by id {process.Id} ({process.ProcessName})");
                var processId = process.Id;
                try
                {
                    process.Kill();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Process cannot be killed", e);
                    return;
                }
                while (true)
                {
                    try
                    {
                        if (Process.GetProcessById(processId).HasExited)
                        {
                            Console.WriteLine($"Process with id {processId} successfully killed");
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error while get killed process by id", e);
                        return;
                    }
                    await Task.Delay(100);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while kill process", e);
            }
        }

        public static async Task KillTreeAsync(this Process process)
        {
            var treeIds = GetProcessTree(process.Id).ToArray();

            foreach(var p in treeIds.Select(GetProcessByIdSafe).Where(x => x != null))
            {
                await p.KillAsync();
            }
        }

        private static Process GetProcessByIdSafe(int processId)
        {
            try
            {
                return Process.GetProcessById(processId);
            }
            catch(Exception e)
            {
                Console.WriteLine("Unable to get process by id", e);
                return null;
            }
        }

        private static IEnumerable<int> GetProcessTree(int rootId)
        {
            using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher($"SELECT ProcessId FROM Win32_Process WHERE ParentProcessID = {rootId}"))
            {
                foreach (var o in managementObjectSearcher.Get())
                {
                    var managementObject = (ManagementObject)o;
                    var childId = int.Parse(managementObject["ProcessId"].ToString());
                    foreach (var id in GetProcessTree(childId))
                    {
                        yield return id;
                    }
                }
            }

            yield return rootId;
        }
    }
}