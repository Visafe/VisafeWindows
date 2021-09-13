using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace VisafeService
{
    public static class Helper
    {
        public static bool ExecuteCmdCommand(string cmdArgs)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo("cmd.exe", cmdArgs)

                };
                process.Start();

                process.WaitForExit();
                return true;
            }
            catch (Exception exp)
            {
                Console.WriteLine(String.Format("Failed to execute command:'{0}' . Error:{1}", cmdArgs, exp.Message), EventLogEntryType.Error);
                return false;
            }
        }

        public static bool RunExecutable(string exeLocation)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo(exeLocation)

                };
                process.Start();

                return true;
            }
            catch (Exception exp)
            {
                Console.WriteLine(String.Format("Failed to start process:'{0}' . Error:{1}", exeLocation, exp.Message), EventLogEntryType.Error);
                return false;
            }
        }
    }
}
