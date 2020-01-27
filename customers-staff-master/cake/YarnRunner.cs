using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace cake
{
    public class YarnRunner
    {
        public async Task Start(string cementDirectory, TimeSpan timeout)
        {
            await KillYarnProcesses();

            var workingDirectory = Path.Combine(cementDirectory, "src", "front");
            var batPath = Path.Combine(workingDirectory, "startCustomerAndStaffDevServer.bat");

            var process = new Process
                {
                    StartInfo =
                        {
                            FileName = batPath,
                            WorkingDirectory = workingDirectory,
                            WindowStyle = ProcessWindowStyle.Minimized,
                            CreateNoWindow = true,
                            UseShellExecute = true,
                        }
                };
            process.Start();

            var httpClient = new HttpClient {Timeout = TimeSpan.FromSeconds(5)};

            var sw = Stopwatch.StartNew();
            while(true)
            {
                if(sw.Elapsed > timeout)
                    break;

                await Task.Delay(1000);
                try
                {
                    var res = await httpClient.GetAsync("http://localhost:3000");
                    if (res.StatusCode == HttpStatusCode.OK)
                        return;
                }
                catch(Exception exception)
                {
                    Console.WriteLine($"No connection {exception.GetType()}");
                }
            }

            await KillYarnProcesses();
            throw new Exception("Timeout expired");
        }

        public async Task KillYarnProcesses()
        {
            var allYarnProcessIds = GetAllYarnProcessIds().ToArray();
            foreach(int yarnProcessId in allYarnProcessIds)
            {
                await Process.GetProcessById(yarnProcessId).KillTreeAsync();
            }
        }
        
        private IEnumerable<int> GetAllYarnProcessIds()
        {
            using(ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT ProcessId, CommandLine FROM Win32_Process"))
            {
                foreach(var o in managementObjectSearcher.Get())
                {
                    var managementObject = (ManagementObject)o;
                    if(managementObject["CommandLine"] != null)
                    {
                        if(managementObject["CommandLine"].ToString().Contains("startCustomerAndStaffDevServer.bat"))
                            yield return int.Parse(managementObject["ProcessId"].ToString());
                    }
                }
            }
        }
    }
}