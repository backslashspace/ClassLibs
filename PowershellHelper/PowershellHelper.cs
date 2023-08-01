using System;
using System.Collections.Generic;
using System.Threading;

namespace PowershellHelper
{
    ///<summary>Simple way to work with PowerShell</summary>
    public sealed class PowershellHelper
    {
        ///<summary>Execute PowerShell commands or entire scripts.</summary>
        public static String[] PowerShell(String RawPSCommand, Boolean OutPut = false, Boolean FormatCustom = false, Int32 RMTop = 0, Int32 RMBottom = 0, Boolean WaitForExit = true)
        {
            if (WaitForExit)
            {
                return LeProgg();
            }
            else
            {
                ThreadStart e = new(() =>
                {
                    LeProgg();
                });

                return null;
            }

            String[] LeProgg()
            {
                if (!OutPut)
                {
                    System.Management.Automation.PowerShell.Create().AddScript(RawPSCommand).Invoke();

                    return null;
                }

                List<String> PSOut = new();

                if (FormatCustom)
                {
                    RawPSCommand += " | Format-Custom";
                }

                foreach (var i in System.Management.Automation.PowerShell.Create().AddScript(RawPSCommand + " | Out-String").Invoke())
                {
                    if (i.ToString() is "")
                    {
                        continue;
                    }

                    PSOut.Add(i.ToString().Replace("\n", "").Replace("\r", ""));
                }

                for (Int32 i = 0; i < RMTop; i++)
                {
                    PSOut.RemoveAt(0);
                }

                for (Int32 i = 0; i < RMBottom; i++)
                {
                    PSOut.RemoveAt(PSOut.Count - 1);
                }

                return PSOut.ToArray();
            }
        }

        ///<summary>Tests if a PowerShell command can be executed.</summary>
        public static Boolean TestPSCommand(String RawPSCommand)
        {
            try
            {
                System.Management.Automation.PowerShell.Create().AddCommand(RawPSCommand).Invoke();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}